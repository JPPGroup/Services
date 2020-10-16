using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using SkiaSharp;

namespace Jpp.MappingReportGenerator
{
    class FloodZone3DrawingTemplate : DrawingTemplate
    {
        public FloodZone3DrawingTemplate(TileProvider provider) : base(provider)
        { }

        public override SKSurface DrawImage(int MMWidth, int MMHeight, WGS84 location)
        {
            using (SKSurface map = _tileProvider.GetScaleImage(5000, MMWidth, MMHeight, new string[] { "OSM", "floodzone3" }, location).Result)
            {
                return Render(MMWidth, MMHeight, map);
            }
        }

        public override SKSurface DrawKey(int MMWidth, int MMHeight)
        {
            var info = new SKImageInfo(MappingReport.FromMillimeter(MMWidth), MappingReport.FromMillimeter(MMHeight));
            var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            AddKeyItem(new SKColor(127, 42, 251, 127), "Risk of flooding", canvas);

            return surface;
        }

        public override string GetTitle()
        {
            return "Flood Zone 3 1:5000";            
        }
    }
}
