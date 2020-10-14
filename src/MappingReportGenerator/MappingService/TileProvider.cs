using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jpp.MappingReportGenerator
{
    public class TileProvider
    {
        private Dictionary<string, TileLayer> Layers;

        public TileProvider()
        {
            Layers = new Dictionary<string, TileLayer>();
            Layers.Add("OSM", new TileLayer("http://services/mapping/osm/tile/{z}/{x}/{y}.png"));
            Layers.Add("floodzone2", new TileLayer("http://services/mapping/fz2/tile/{z}/{x}/{y}.png"));
            Layers.Add("radon", new TileLayer("http://services/mapping/radon/tile/{z}/{x}/{y}.png"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Scale">Desired drawings scale in format 1m:???m</param>
        /// <param name="Width">Width of image in mm</param>
        /// <param name="Height">Height of image in mm</param>
        /// <param name="layers">List of layers to render in order provided</param>
        /// <param name="center">Center of image</param>
        /// <returns></returns>        
        public async Task<SKSurface> GetScaleImage(int Scale, int Width, int Height, string[] layers, WGS84 center)
        {            
            int pixelWidth = (int)(97 * Width * 0.0393701);
            int pixelHeight = (int)(97 * Height * 0.0393701);

            //Calculate closest size to requested scale, and calculate scaling factor required
            double physicalDrawingWidth = Width * Scale / 1000; //Divide by 1000 to convert mm to m
            double resolution = physicalDrawingWidth / pixelWidth;

            double metersPerPx = 156543.03392 * Math.Cos(center.Latitude * Math.PI / 180) / Math.Pow(2, 18);

            //Generate images
            /*Bitmap _buffer = new Bitmap(pixelWidth, pixelHeight);
            Graphics graphics = Graphics.FromImage(_buffer);*/

            var info = new SKImageInfo(pixelWidth, pixelHeight);
            var _buffer = SKSurface.Create(info);

            using (var graphics = _buffer.Canvas)
            {
                try
                {

                    foreach (string s in layers)
                    {
                        if (Layers.ContainsKey(s))
                        {
                            using (var bitmap = await Layers[s].GetImage(center, resolution, pixelWidth, pixelHeight))
                            {
                                using (SKImage image = bitmap.Snapshot())
                                {
                                    SKRect destRect = new SKRect(0, 0, pixelWidth, pixelHeight);
                                    graphics.DrawImage(image, destRect, new SKPaint());
                                }
                            }
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("Layer not found");
                        }
                    }
                }
                catch
                {
                    _buffer = null;
                }
            }

            return _buffer;
        }
    }
}
