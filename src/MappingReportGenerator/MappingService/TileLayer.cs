using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BruTile;
using BruTile.Predefined;
using BruTile.Web;
using SkiaSharp;

namespace Jpp.MappingReportGenerator
{
    public class TileLayer
    {
        readonly double[] _unitsPerPixelArray =
        {
            156543.033900000, 78271.516950000, 39135.758475000, 19567.879237500, 9783.939618750,
            4891.969809375, 2445.984904688, 1222.992452344, 611.496226172, 305.748113086,
            152.874056543, 76.437028271, 38.218514136, 19.109257068, 9.554628534, 4.777314267,
            2.388657133, 1.194328567, 0.597164283
        };

        private HttpTileSource tileSource;

        public TileLayer(string endpoint)
        {
            tileSource = new HttpTileSource(new GlobalSphericalMercator(0, 18), endpoint, new[] { "a", "b", "c" }, "OSM");            
        }


        public async Task<SKSurface> GetImage(WGS84 coordinates, double Resolution, int Width, int Height)
        {
            double halfX = Width / 2 * Resolution;//_unitsPerPixelArray[ZoomLevel];
            double halfY = Height / 2 * Resolution;//_unitsPerPixelArray[ZoomLevel];
            
            // the extent of the visible map changes but lets start with the whole world
            var extent = new Extent(coordinates.WebMercatorX - halfX, coordinates.WebMercatorY - halfY, coordinates.WebMercatorX + halfX, coordinates.WebMercatorY + halfY);
            var tileInfos = tileSource.Schema.GetTileInfos(extent, Resolution);
            int zoomLevel = tileInfos.First().Index.Level;
            double scale = Resolution / _unitsPerPixelArray[zoomLevel];

            var info = new SKImageInfo(Width, Height);

            var _buffer = SKSurface.Create(info);
            using (var graphics = _buffer.Canvas)
            {
                graphics.ClipRect(new SKRect(0, 0, Width, Height));

                foreach (var tileInfo in tileInfos)
                {
                    var tile = tileSource.GetTile(tileInfo);
                    Uri uri = tileSource.GetUri(tileInfo);
                    using (SKBitmap bmap = SKBitmap.Decode(tileSource.GetTile(tileInfo)))
                    {
                        //Uncomment to write out as check
                        //SKImage.FromBitmap(bmap).Encode().SaveTo(File.Create($"{tileInfo.Index.Col}-{tileInfo.Index.Row}-{tileInfo.Index.Level}.png"));

                        //Calculate extent                
                        var point1 = WorldToScreen(tileInfo.Extent.MinX, tileInfo.Extent.MinY, Resolution, extent);
                        var point2 = WorldToScreen(tileInfo.Extent.MaxX, tileInfo.Extent.MaxY, Resolution, extent);
                        SKRect size = new SKRect(point1.X, point2.Y, point2.X, point1.Y);
                        graphics.DrawBitmap(bmap, size, new SKPaint());
                    }
                }

                return _buffer;
            }
        }

        public PointF WorldToScreen(double x, double y, double _unitsPerPixel, Extent _extent)
        {
            return new PointF((float)((x - _extent.MinX) / _unitsPerPixel), (float)((_extent.MaxY - y) / _unitsPerPixel));
        }

        private SKRect RoundToPixel(SKRect dest)
        {
            // To get seamless aligning you need to round the locations
            // not the width and height
            var result = new SKRect(
                (int)Math.Round(dest.Left),
                (int)Math.Round(dest.Top),
                (int)(Math.Round(dest.Right) - Math.Round(dest.Left)),
                (int)(Math.Round(dest.Bottom) - Math.Round(dest.Top)));
            return result;
        }
    }
}
