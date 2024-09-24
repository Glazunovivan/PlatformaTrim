using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace PlatformaTrim.DigitalSignature
{
    /// <summary>
    /// Шаблон штампа цифровой подписи
    /// </summary>
    public class DigitalSignatureStampTemplate
    {
        private const string _titleKey = "StampTitleText";
        private const string _titleInfoKey = "StampTitleInfo";
        private const string _headerNameKey = "StampHeaderName";
        private const string _headerNumberCertificateKey = "StampHeaderNumberCertificate";
        private const string _headerDateKey = "StampHeaderDate";
        private const string _marginKey = "StampMargin";
        private const string _fontSizeKey = "StampFontSize";

        public string Title { get; init; }
        public string Info { get; init; }
        public string Certificate { get; init; }
        public string Name { get; init; }
        public string Date { get; init; }

        public float Margin { get; init; }
        public float FontSize { get; init; }

        public DigitalSignatureStampTemplate(string number, string fullname, DateTime dateStart, DateTime dateEnd)
        {
            Title = ConfigurationManager.AppSettings.Get(_titleKey);
            Info = ConfigurationManager.AppSettings.Get(_titleInfoKey);
            Certificate = $"{ConfigurationManager.AppSettings.Get(_headerNumberCertificateKey)}: {number}";
            Name = $"{ConfigurationManager.AppSettings.Get(_headerNameKey)}: {fullname}";
            Date = $"{ConfigurationManager.AppSettings.Get(_headerDateKey)}: с { dateStart.Date.ToString("dd/MM/yyyy")} по {dateEnd.Date.ToString("dd/MM/yyyy")}";
            Margin = float.Parse(ConfigurationManager.AppSettings.Get(_marginKey));
            FontSize = float.Parse(ConfigurationManager.AppSettings.Get(_fontSizeKey));
        
        }

        public string MaxWidthText()
        {
            var list = new List<string>
            {
                Name,
                Certificate,
                Date
            };
            return list.OrderByDescending(x => x.Length).First();
        }
    }
}