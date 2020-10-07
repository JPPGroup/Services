﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Jpp.Common.Mapping;
using SkiaSharp;

namespace Jpp.MappingReportGenerator
{
    class OSLargeDrawingTemplate : DrawingTemplate
    {
        public OSLargeDrawingTemplate(TileProvider provider) : base(provider)
        { }

        public override SKSurface DrawImage(int MMWidth, int MMHeight, WGS84 location)
        {
            using (SKSurface map = _tileProvider.GetScaleImage(50000, MMWidth, MMHeight, new string[] { "OSM" }, location).Result)
            {
                return Render(MMWidth, MMHeight, map);
            }
        }

        public override SKSurface DrawKey(int MMWidth, int MMHeight)
        {
            return null;
        }

        public override string GetTitle()
        {
            return "OS Extract 1:50,000";            
        }
    }
}
