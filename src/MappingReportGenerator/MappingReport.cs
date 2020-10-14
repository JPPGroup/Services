using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Jpp.MappingReportGenerator
{
    class MappingReport : IDisposable
    {
        SKDocument _document;
        public static float DPI = 72f / 25.4f * 6;//8;
        public int XMargin = FromMillimeter(13);       
        public int YMargin = FromMillimeter(20);
        public int XWidth = FromMillimeter(394);
        public int YWidth = FromMillimeter(257);
        public int KeyWidth = FromMillimeter(90);
        public int LogoSize = FromMillimeter(45);
        int XKeyDivide;

        SKPaint _calibri, _calibriHeading, _calibriLargeText, _calibriTitle;

        TileProvider _provider;
        WGS84 _location;
        string _projectName, _client, _referenceNumber;
        SKPaint line;
        SKBitmap logoBitmap;

        public MappingReport(TileProvider provider, string ProjectName, string Client, string ReferenceNumber, WGS84 location)
        {
            _projectName = ProjectName;
            _client = Client;
            _referenceNumber = ReferenceNumber;

            XKeyDivide = XMargin + XWidth - KeyWidth;

            _provider = provider;
            _location = location;
            _calibri = new SKPaint()
            {
                TextSize = FromMillimeter(3),
                IsAntialias = false,
                Color = new SKColor(0, 0, 0, 255),
                IsStroke = false,
                Typeface = SKTypeface.FromFamilyName("Calibri")                
            };
            _calibriLargeText = new SKPaint()
            {
                TextSize = FromMillimeter(4),
                IsAntialias = false,
                Color = new SKColor(0, 0, 0, 255),
                IsStroke = false,
                Typeface = SKTypeface.FromFamilyName("Calibri")
            };
            _calibriHeading = new SKPaint()
            {
                TextSize = FromMillimeter(4),
                IsAntialias = false,
                Color = new SKColor(0, 0, 0, 255),
                IsStroke = false,
                Typeface = SKTypeface.FromFamilyName("Calibri"),
                FakeBoldText = true
            };
            _calibriTitle = new SKPaint()
            {
                TextSize = FromMillimeter(6),
                IsAntialias = false,
                Color = new SKColor(0, 0, 0, 255),
                IsStroke = false,
                Typeface = SKTypeface.FromFamilyName("Calibri"),
                FakeBoldText = true
            };

            line = new SKPaint()
            {
                Color = new SKColor(0, 0, 0, 255),
                StrokeWidth = FromMillimeter(0.5f),
                IsAntialias = false,
                Style = SKPaintStyle.Stroke
            };

            //Logo
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Jpp.MappingReportGenerator.Resources.Navy logo.png")) 
            using (var stream = new SKManagedStream(s))
            {
                logoBitmap = SKBitmap.Decode(stream);
            }              

            _document = SKDocument.CreatePdf($"{ReferenceNumber}.pdf", DPI);            
        }

        public void AddDrawing(DrawingType drawingType)
        {
            var info = new SKImageInfo(XMargin * 2 + XWidth, YMargin * 2 + YWidth);

            using (SKCanvas gfx = _document.BeginPage(XMargin * 2 + XWidth, YMargin * 2 + YWidth))
            using (var surface = SKSurface.Create(info))
            using (var compositeCanvas = surface.Canvas)
            {
                DrawingTemplate template;
                switch (drawingType)
                {
                    case DrawingType.OS:
                        template = new OSDrawingTemplate(_provider);
                        break;

                    case DrawingType.OSLarge:
                        template = new OSLargeDrawingTemplate(_provider);
                        break;

                    case DrawingType.Radon:
                        template = new RadonDrawingTemplate(_provider);
                        break;

                    case DrawingType.FloodZone2:
                        template = new FloodZone2DrawingTemplate(_provider);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                //Draw drawing title
                compositeCanvas.DrawText(template.GetTitle(), XKeyDivide + FromMillimeter(2), YMargin + FromMillimeter(40), _calibriTitle);

                using (var tempSurface = template.DrawImage(304, 257, _location))
                {
                    tempSurface.Draw(compositeCanvas, XMargin, YMargin, new SKPaint());
                }

                //_A3template.Draw(compositeCanvas, 0, 0, new SKPaint());
                DrawTemplate(compositeCanvas);

                using (var tempSurface = template.DrawKey(86, 86))
                {
                    if (tempSurface != null)
                        tempSurface.Draw(compositeCanvas, XMargin + XWidth - KeyWidth + FromMillimeter(4), YMargin + FromMillimeter(50), new SKPaint());
                }

                //compositeCanvas.Draw(gfx, 0, 0, new SKPaint());
                surface.Draw(gfx, 0, 0, new SKPaint());
            }
            _document.EndPage();
        }

        public void DrawTemplate(SKCanvas canvas)
        {            
            canvas.DrawRect(XMargin, YMargin, XWidth, YWidth, line);
            canvas.DrawLine(XKeyDivide, YMargin, XKeyDivide, YMargin + YWidth, line);
            canvas.DrawLine(XKeyDivide, YMargin + YWidth - LogoSize, XKeyDivide + KeyWidth, YMargin + YWidth - LogoSize, line);
            canvas.DrawLine(XKeyDivide, YMargin + YWidth - LogoSize - FromMillimeter(30), XKeyDivide + KeyWidth, YMargin + YWidth - LogoSize - FromMillimeter(30), line); //Title divide
            canvas.DrawLine(XKeyDivide, YMargin + FromMillimeter(32), XKeyDivide + KeyWidth, YMargin + FromMillimeter(32), line); //Notes divide

            using (var paint = new SKPaint())
            {
                SKRect logoRect = new SKRect(XKeyDivide, YMargin + YWidth - LogoSize, XKeyDivide + LogoSize, YMargin + YWidth);
                canvas.DrawBitmap(logoBitmap, logoRect, line);
            }

            //JPP info
            /*XTextFormatter tf1 = new XTextFormatter(A3templateGfx);
            tf1.Alignment = XParagraphAlignment.Center;
            rect = new XRect(XKeyDivide + LogoSize, YMargin + YWidth - LogoSize + 20, KeyWidth - LogoSize, LogoSize - 40);
            string headerText = "JPP Consulting\n\n01908 889433\n\nsoftware@jppuk.net";
            tf1.DrawString(headerText, _calibriHeading, XBrushes.Navy, rect, XStringFormats.TopLeft);*/
                        
            canvas.DrawText("While every effort has been made to validate the accuracy of the ", XKeyDivide + FromMillimeter(2), YMargin + FromMillimeter(4), _calibri);
            canvas.DrawText("information shown, this drawing is provided as is and all information", XKeyDivide + FromMillimeter(2), YMargin + FromMillimeter(7), _calibri);
            canvas.DrawText("shown should be independantly verified before using for design", XKeyDivide + FromMillimeter(2), YMargin + FromMillimeter(10), _calibri);
            canvas.DrawText("purposes.", XKeyDivide + FromMillimeter(2), YMargin + FromMillimeter(13), _calibri);

            canvas.DrawText("All information shown is not to be reproduced, copied or redistributed", XKeyDivide + FromMillimeter(2), YMargin + FromMillimeter(19), _calibri);
            canvas.DrawText("without the permission of the original rights holder.", XKeyDivide + FromMillimeter(2), YMargin + FromMillimeter(22), _calibri);

            canvas.DrawText("Drawing is not to be scaled from.", XKeyDivide + FromMillimeter(2), YMargin + FromMillimeter(28), _calibri);

            //Project Info
            canvas.DrawText("Project Name:", XKeyDivide + FromMillimeter(2), YMargin + YWidth - LogoSize - FromMillimeter(25), _calibriHeading);
            canvas.DrawText("Client:", XKeyDivide + FromMillimeter(2), YMargin + YWidth - LogoSize - FromMillimeter(20), _calibriHeading);
            canvas.DrawText("Order Ref:", XKeyDivide + FromMillimeter(2), YMargin + YWidth - LogoSize - FromMillimeter(10), _calibriHeading);
            canvas.DrawText("Generated On:", XKeyDivide + FromMillimeter(2), YMargin + YWidth - LogoSize - FromMillimeter(5), _calibriHeading);

            canvas.DrawText(_projectName, XKeyDivide + FromMillimeter(32), YMargin + YWidth - LogoSize - FromMillimeter(25), _calibriHeading);
            canvas.DrawText(_client, XKeyDivide + FromMillimeter(32), YMargin + YWidth - LogoSize - FromMillimeter(20), _calibriHeading);
            canvas.DrawText(_referenceNumber, XKeyDivide + FromMillimeter(32), YMargin + YWidth - LogoSize - FromMillimeter(10), _calibriHeading);
            canvas.DrawText(DateTime.Now.ToShortDateString(), XKeyDivide + FromMillimeter(32), YMargin + YWidth - LogoSize - FromMillimeter(5), _calibriHeading);            
        }

        public void Finalise()
        {
            _document.Close();
        }

        public static int FromMillimeter(float y)
        {
            return (int) (y * MappingReport.DPI);
        }

        public void Dispose()
        {
            _calibri.Dispose();
            _calibriHeading.Dispose();
            _calibriLargeText.Dispose();
            _calibriTitle.Dispose();

            _document.Dispose();
        }
    }

    public enum DrawingType
    {
        OS,
        OSLarge,
        Radon,
        MiningHazards,
        FloodZone2
    }
}
