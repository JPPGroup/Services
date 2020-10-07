using Jpp.Common.Mapping;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jpp.MappingReportGenerator
{
    public abstract class DrawingTemplate
    {
        protected TileProvider _tileProvider;

        public DrawingTemplate(TileProvider provider)
        {
            _tileProvider = provider;
        }

        public abstract string GetTitle();

        public abstract SKSurface DrawKey(int MMWidth, int MMHeight);

        public abstract SKSurface DrawImage(int MMWidth, int MMHeight, WGS84 location);

        protected SKSurface Render(int MMWidth, int MMHeight, SKSurface input)
        {
            var info = new SKImageInfo(MappingReport.FromMillimeter(MMWidth), MappingReport.FromMillimeter(MMHeight));            
            var surface = SKSurface.Create(info);
            using (var canvas = surface.Canvas)
            {
                using (MemoryStream s = new MemoryStream())
                {
                    if (input != null)
                    {
                        using (var paint = new SKPaint())
                        {
                            SKRect logoRect = new SKRect(0, 0, MappingReport.FromMillimeter(MMWidth), MappingReport.FromMillimeter(MMHeight));
                            using (SKImage image = input.Snapshot())
                            {
                                canvas.DrawImage(image, logoRect, new SKPaint());
                            }
                        }
                    }
                    else
                    {
                        MissingTile(surface, MMWidth, MMHeight, GetTitle());
                    }
                }
            }

            return surface;
        }

        protected int KeyCount = 0;
        protected int KeyHeight = MappingReport.FromMillimeter(10);

        protected void AddKeyItem(SKColor color, string text, SKCanvas gfx)
        {
            SKPaint line = new SKPaint()
            {
                Color = new SKColor(0, 0, 0, 255),
                StrokeWidth = MappingReport.FromMillimeter(0.5f),
                Style = SKPaintStyle.Stroke,
                IsAntialias = false                
            };            
            gfx.DrawRect(0, (KeyHeight + MappingReport.FromMillimeter(2)) * KeyCount, KeyHeight, KeyHeight, line);

            SKPaint fill = new SKPaint()
            {
                Color = color,
                StrokeWidth = MappingReport.FromMillimeter(0.5f),
                Style = SKPaintStyle.Fill,
                IsAntialias = false
            };
            gfx.DrawRect(0, (KeyHeight + MappingReport.FromMillimeter(2)) * KeyCount, KeyHeight, KeyHeight, fill);

            SKPaint textBrush = new SKPaint()
            {
                TextSize = MappingReport.FromMillimeter(3),
                IsAntialias = false,
                Color = new SKColor(0, 0, 0),
                IsStroke = false,
                Typeface = SKTypeface.FromFamilyName("Calibri")
            };

            gfx.DrawText(text, KeyHeight + MappingReport.FromMillimeter(1), (KeyHeight + MappingReport.FromMillimeter(2)) * KeyCount + (KeyHeight + MappingReport.FromMillimeter(3)) / 2, textBrush);           
            
            KeyCount++;
        }

        protected void MissingTile(SKSurface surface, int MMWidth, int MMHeight, string tileRef)
        {
            Console.WriteLine($"Tile not available - {tileRef}");
            var canvas = surface.Canvas;

            SKPaint textBrush = new SKPaint()
            {
                TextSize = MappingReport.FromMillimeter(18),
                IsAntialias = true,
                Color = new SKColor(0, 0, 0),
                IsStroke = false,
                Typeface = SKTypeface.FromFamilyName("Calibri"),
                TextAlign = SKTextAlign.Center
            };

            canvas.DrawText($"Tile not available - {tileRef}", MappingReport.FromMillimeter(MMWidth / 2), MappingReport.FromMillimeter(MMHeight / 2), textBrush);
        }
    }
}
