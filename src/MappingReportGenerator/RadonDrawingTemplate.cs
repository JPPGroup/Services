using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using SkiaSharp;

namespace Jpp.MappingReportGenerator
{
    class RadonDrawingTemplate : DrawingTemplate
    {
        public RadonDrawingTemplate(TileProvider provider) : base(provider)
        { }

        public override SKSurface DrawImage(int MMWidth, int MMHeight, WGS84 location)
        {
            using (SKSurface map = _tileProvider.GetScaleImage(5000, MMWidth, MMHeight, new string[] { "OSM", "radon" }, location).Result)
            {
                return Render(MMWidth, MMHeight, map);
            }           
        }

        public override SKSurface DrawKey(int MMWidth, int MMHeight)
        {
            var info = new SKImageInfo(MappingReport.FromMillimeter(MMWidth), MappingReport.FromMillimeter(MMHeight));
            var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            AddKeyItem(new SKColor(255, 229, 99, 127), "Low radon risk", canvas);

            AddKeyItem(new SKColor(250, 158, 29, 127), "Medium radon risk", canvas);

            AddKeyItem(new SKColor(255, 0, 0, 127), "High radon risk", canvas);

            return surface;
        }

        public override string GetTitle()
        {
            return "Radon Map 1:5000";            
        }
    }
}
