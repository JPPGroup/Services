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
        public static float DPI = 72f / 25.4f * 4;//5 for demo, 8 otherwise;
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
        SKBitmap logoBitmap, titleBitmap;

        List<DrawingTemplate> drawings;
        bool _contentPage = false;

        public static MappingReport CreateStandard(TileProvider provider, string ProjectName, string Client,
            string ReferenceNumber, WGS84 location)
        {
            MappingReport mappingReport = new MappingReport(provider, ProjectName, Client, ReferenceNumber, location);

            mappingReport.AddCover();
            mappingReport.AddContents();
            mappingReport.AddDrawing(DrawingType.OSLarge);
            mappingReport.AddDrawing(DrawingType.SatelliteLarge);
            mappingReport.AddDrawing(DrawingType.HistoricLarge);
            mappingReport.AddDrawing(DrawingType.OS);
            mappingReport.AddDrawing(DrawingType.Satellite);
            mappingReport.AddDrawing(DrawingType.Historic);
            mappingReport.AddDrawing(DrawingType.OSSmall);
            mappingReport.AddDrawing(DrawingType.SatelliteSmall);
            mappingReport.AddDrawing(DrawingType.HistoricSmall);
            mappingReport.AddDrawing(DrawingType.Radon);
            mappingReport.AddDrawing(DrawingType.SUperficialGeo);
            mappingReport.AddDrawing(DrawingType.FloodZone2);
            mappingReport.AddDrawing(DrawingType.FloodZone3);
            mappingReport.Finalise();

            return mappingReport;
        }

        public MappingReport(TileProvider provider, string ProjectName, string Client, string ReferenceNumber, WGS84 location)
        {
            drawings = new List<DrawingTemplate>();

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

            using(Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Jpp.MappingReportGenerator.Resources.Title.bmp"))
            using (var stream = new SKManagedStream(s))
            {
                titleBitmap = SKBitmap.Decode(stream);
            }

            SKDocumentPdfMetadata metadata = SKDocumentPdfMetadata.Default;
            metadata.RasterDpi = DPI;
            metadata.EncodingQuality = 10;

            _document = SKDocument.CreatePdf($"{ReferenceNumber}.pdf", metadata);
        }

        public void AddDrawing(DrawingType drawingType)
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

                case DrawingType.OSSmall:
                    template = new OSSmallDrawingTemplate(_provider);
                    break;

                case DrawingType.Radon:
                    template = new RadonDrawingTemplate(_provider);
                    break;

                case DrawingType.FloodZone2:
                    template = new FloodZone2DrawingTemplate(_provider);
                    break;

                case DrawingType.FloodZone3:
                    template = new FloodZone3DrawingTemplate(_provider);
                    break;

                case DrawingType.SatelliteLarge:
                    template = new SatelliteLargeDrawingTemplate(_provider);
                    break;

                case DrawingType.Satellite:
                    template = new SatelliteDrawingTemplate(_provider);
                    break;

                case DrawingType.SatelliteSmall:
                    template = new SatelliteSmallDrawingTemplate(_provider);
                    break;

                case DrawingType.HistoricSmall:
                    template = new HistoricSmallDrawingTemplate(_provider);
                    break;

                case DrawingType.Historic:
                    template = new HistoricDrawingTemplate(_provider);
                    break;

                case DrawingType.HistoricLarge:
                    template = new HistoricLargeDrawingTemplate(_provider);
                    break;

                case DrawingType.SUperficialGeo:
                    template = new SuperficialGeoDrawingTemplate(_provider);
                    break;

                default:
                    throw new NotImplementedException();
            }

            drawings.Add(template);
        }

        private void DrawDrawing(DrawingTemplate template)
        {
            var info = new SKImageInfo(XMargin * 2 + XWidth, YMargin * 2 + YWidth);

            using (SKCanvas gfx = _document.BeginPage(XMargin * 2 + XWidth, YMargin * 2 + YWidth))
            using (var surface = SKSurface.Create(info))
            using (var compositeCanvas = surface.Canvas)
            {
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

        public void AddCover()
        {
            var info = new SKImageInfo(XMargin * 2 + XWidth, YMargin * 2 + YWidth);

            using (SKCanvas gfx = _document.BeginPage(XMargin * 2 + XWidth, YMargin * 2 + YWidth))
            using (var surface = SKSurface.Create(info))
            using (var compositeCanvas = surface.Canvas)
            {
                SKPaint fill = new SKPaint();
                fill.Color = new SKColor(0, 75, 145);
                compositeCanvas.DrawRect(new SKRect(XMargin, YMargin, XWidth + XMargin, YWidth + YMargin), fill);
                fill.Dispose();

                using (var paint = new SKPaint())
                {
                    SKRect logoRect = new SKRect(XMargin + FromMillimeter(2), YMargin + FromMillimeter(26), XMargin + FromMillimeter(222), YMargin + FromMillimeter(189) + FromMillimeter(26));
                    compositeCanvas.DrawBitmap(titleBitmap, logoRect, line);
                }

                SKPaint titleText = new SKPaint()
                {
                    TextSize = FromMillimeter(10),
                    IsAntialias = false,
                    Color = new SKColor(255, 255, 255, 255),
                    IsStroke = false,
                    Typeface = SKTypeface.FromFamilyName("Calibri"),
                    FakeBoldText = true,
                    TextAlign = SKTextAlign.Right
                };

                compositeCanvas.DrawText("JPP Consulting Location Report", FromMillimeter(375), FromMillimeter(220), titleText);
                compositeCanvas.DrawText(_projectName, FromMillimeter(375), FromMillimeter(232), titleText);
                compositeCanvas.DrawText(_referenceNumber, FromMillimeter(375), FromMillimeter(244), titleText);

                titleText.Dispose();

                surface.Draw(gfx, 0, 0, new SKPaint());
            }
            _document.EndPage();
        }

        public void AddContents()
        {
            _contentPage = true;
        }

        public void DrawContents()
        {
            var info = new SKImageInfo(XMargin * 2 + XWidth, YMargin * 2 + YWidth);

            using (SKCanvas gfx = _document.BeginPage(XMargin * 2 + XWidth, YMargin * 2 + YWidth))
            using (var surface = SKSurface.Create(info))
            using (var compositeCanvas = surface.Canvas)
            {
                compositeCanvas.DrawText("Contents", XMargin, YMargin + FromMillimeter(6), _calibriTitle);
                compositeCanvas.DrawLine(XMargin, YMargin + FromMillimeter(8), XWidth + XMargin, YMargin + FromMillimeter(8), line);

                int pageCount = 1;
                foreach (DrawingTemplate dt in drawings)
                {
                    compositeCanvas.DrawText(dt.GetTitle(), XMargin, YMargin + FromMillimeter(pageCount * 8 + 10), _calibriLargeText);
                    compositeCanvas.DrawText("......", XMargin + FromMillimeter(80), YMargin + FromMillimeter(pageCount * 8 + 10), _calibriLargeText);
                    compositeCanvas.DrawText($"{pageCount}", XMargin + FromMillimeter(140), YMargin + FromMillimeter(pageCount * 8 + 10), _calibriLargeText);
                    pageCount++;
                }

                surface.Draw(gfx, 0, 0, new SKPaint());
            }
            _document.EndPage();
        }

        public void Finalise()
        {
            if (_contentPage)
            {
                DrawContents();
            }
            foreach (DrawingTemplate dt in drawings)
            {
                DrawDrawing(dt);
            }
            _document.Close();
        }

        public static int FromMillimeter(float y)
        {
            return (int)(y * MappingReport.DPI);
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
        OSSmall,
        Satellite,
        SatelliteLarge,
        SatelliteSmall,
        HistoricLarge,
        Historic,
        HistoricSmall,
        Radon,
        MiningHazards,
        FloodZone2,
        FloodZone3,
        SUperficialGeo
    }
}
