using GdPicture14;
using PlatformaTrim.Models;
using System;

namespace PlatformaTrim.DigitalSignature
{
    internal class DigitalSignatureConvertor : IDisposable
    {
        public DigitalSignatureConvertor(UserData userData, string filePath)
        {
            _filePath = filePath;

            _converter = new GdPictureDocumentConverter();
            _gdpicturePDF = new GdPicturePDF();
            _templateStamp = new DigitalSignatureStampTemplate(userData.NumberCertificate, userData.Fullname, userData.DateStart, userData.DateEnd);
            _templateFooter = new DigitalSignatureFooterTemplate(userData.NumberCertificate, userData.Fullname, userData.DateStart, userData.DateEnd);
            _settings = new DigitalSignatureGeneralSettings();
        }

        
        private GdPicturePDF _gdpicturePDF;
        private GdPictureDocumentConverter _converter;
        private DigitalSignatureStampTemplate _templateStamp;
        private DigitalSignatureFooterTemplate _templateFooter;
        private DigitalSignatureGeneralSettings _settings;
        
        private string _filePath;
        private string _fontName;

        public void Accept()
        {
            Convert();
            LoadFile();

            InitSettings();

            DrawStamp();
            DrawFooter();
            
            _gdpicturePDF.SaveToFile(_filePath);
            
            ConvertToPDFA();

        }

        private void Convert()
        {
            if (IsFormatPdf()) return;

            var paths = _filePath.Split('.');
            var newName = paths[paths.Length - 2];

            var newpath = newName + ".pdf";

            using (System.IO.Stream inputStream = System.IO.File.Open(_filePath, System.IO.FileMode.Open))
            using (System.IO.Stream output = System.IO.File.Create(newpath))
                _converter.ConvertToPDF(inputStream, GetFormat(), output, PdfConformance.PDF);

            _filePath = newpath;
        }

        private bool IsFormatPdf() => GetFormat() == GdPicture14.DocumentFormat.DocumentFormatPDF;

        private GdPicture14.DocumentFormat GetFormat()
        {
            var splitters = _filePath.Split('.');
            string format = splitters[splitters.Length - 1];

            return format switch
            {
                "tiff" => GdPicture14.DocumentFormat.DocumentFormatTIFF,
                "doc" => GdPicture14.DocumentFormat.DocumentFormatDOC,
                "docx" => GdPicture14.DocumentFormat.DocumentFormatDOCX,
                "pdf" => GdPicture14.DocumentFormat.DocumentFormatPDF,
                _ => throw new Exception($"Файл формата {format} не поддерживается.")
            };
        }

       
        private void LoadFile() => _gdpicturePDF.LoadFromFile(_filePath, true);

        private void InitSettings()
        {
            _fontName = _gdpicturePDF.AddTrueTypeFontFromFileU(_settings.FontPath, _settings.FontName, false, false, true);
            _gdpicturePDF.SetOrigin(PdfOrigin.PdfOriginTopLeft);
            _gdpicturePDF.SetMeasurementUnit(PdfMeasurementUnit.PdfMeasurementUnitCentimeter);
            _gdpicturePDF.SetLanguage(_settings.LanguageCode);
        }

        private void DrawStamp(int page = 0)
        {
            if (page == 0) 
            {
                page = _gdpicturePDF.GetPageCount();
            }

            if (_gdpicturePDF.GetStat() == GdPictureStatus.OK)
            {
                _gdpicturePDF.SelectPage(page);

                var pageHeight = _gdpicturePDF.GetPageHeight();
                var pageWidth = _gdpicturePDF.GetPageWidth();

                var widthText = _gdpicturePDF.GetTextWidth(_fontName, _templateStamp.FontSize, _templateStamp.MaxWidthText()) + (_templateStamp.Margin * 2);
                var heightText = _gdpicturePDF.GetTextHeight(_fontName, _templateStamp.FontSize) + _templateStamp.Margin;

                var heightRectangle = (heightText * 7);

                var left = pageWidth - widthText;
                var top = pageHeight - heightRectangle;
                var right = left + widthText - _templateStamp.Margin;

                //прямоугольник
                _gdpicturePDF.SetLineColor(0, 0, 0);
                _gdpicturePDF.SetLineWidth(0.01f);
                _gdpicturePDF.DrawRoundedRectangle(left - _templateStamp.Margin,
                                                  top - _templateStamp.Margin,
                                                  widthText,
                                                  heightRectangle,
                                                  0.3f,
                                                  false,
                                                  true);

                _gdpicturePDF.SetTextSize(_templateStamp.FontSize);
                _gdpicturePDF.SetFillColor(0, 0, 0);

                //title
                _gdpicturePDF.DrawTextBox(_fontName,
                                         left + 0.2f,
                                         top,
                                         right - 0.2f,
                                         top + (heightText * 2),
                                         TextAlignment.TextAlignmentCenter,
                                         TextAlignment.TextAlignmentCenter,
                                         _templateStamp.Title
                                         );

                //черный фон + белый текст
                //gdpicturePDF.SetFillColor(0, 0, 0);
                _gdpicturePDF.DrawTextBox(_fontName,
                                         left,
                                         top + (heightText * 2),
                                         right,
                                         top + (heightText * 3),
                                         TextAlignment.TextAlignmentCenter,
                                         TextAlignment.TextAlignmentCenter,
                                         _templateStamp.Info);

                _gdpicturePDF.DrawTextBox(_fontName,
                                         left,
                                         top + (heightText * 3),
                                         right,
                                         top + (heightText * 4),
                                         TextAlignment.TextAlignmentNear,
                                         TextAlignment.TextAlignmentCenter,
                                         _templateStamp.Certificate);


                _gdpicturePDF.DrawTextBox(_fontName,
                                         left,
                                         top + heightText * 4,
                                         right,
                                         top + heightText * 5,
                                         TextAlignment.TextAlignmentNear,
                                         TextAlignment.TextAlignmentCenter,
                                         _templateStamp.Name);

                _gdpicturePDF.DrawTextBox(_fontName,
                                         left,
                                         top + heightText * 5,
                                         right,
                                         top + heightText * 6,
                                         TextAlignment.TextAlignmentNear,
                                         TextAlignment.TextAlignmentCenter,
                                         _templateStamp.Date);
            }
        }

        /// <summary>
        /// Нижний колонтитул
        /// </summary>
        private void DrawFooter()
        {
            int pageCount = _gdpicturePDF.GetPageCount();

            if (pageCount == 1) return;
            
            _gdpicturePDF.SetMeasurementUnit(PdfMeasurementUnit.PdfMeasurementUnitCentimeter);
            _gdpicturePDF.SetOrigin(PdfOrigin.PdfOriginTopLeft);
            
            for (int i = 1; i < pageCount; i++)
            {
                _gdpicturePDF.SelectPage(i);

                var pageWidth = _gdpicturePDF.GetPageWidth();
                var pageHeight = _gdpicturePDF.GetPageHeight();

                var textSize = TryResizeText(_templateFooter.FontSize, _templateFooter.Margin, pageWidth, _templateFooter.Text);

                var textHeight = _gdpicturePDF.GetTextHeight(_fontName, textSize);

                _gdpicturePDF.SetTextSize(textSize);

                _gdpicturePDF.DrawTextBox(_fontName,
                                          _templateFooter.Margin,
                                          pageHeight - textHeight - _templateFooter.Margin,
                                          pageWidth - _templateFooter.Margin,
                                          pageHeight - _templateFooter.Margin,
                                          TextAlignment.TextAlignmentNear,
                                          TextAlignment.TextAlignmentNear,
                                          _templateFooter.Text);

            }
        }

        private void ConvertToPDFA()
        {
            _gdpicturePDF.ConvertToPDFA(_filePath, PdfConversionConformance.PDF_A_1b, true, true);
            _gdpicturePDF.CloseDocument();
        }

        /// <summary>
        /// Изменяет ширину текста, если он не помещается на страницу
        /// </summary>
        /// <param name="textSize"></param>
        /// <param name="margin"></param>
        /// <param name="pageWidth"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private float TryResizeText(float textSize, float margin, float pageWidth, string text)
        {
            var textWidth = _gdpicturePDF.GetTextWidth(_fontName, textSize, text);
            while (textWidth >= (pageWidth - (margin * 2)))
            {
                textSize -= 0.1f;
                textWidth = _gdpicturePDF.GetTextWidth(_fontName, textSize, text);
            }

            return textSize;
        }

        public void Dispose()
        {
            _gdpicturePDF.Dispose();
            _converter.Dispose();
        }
    }
}
