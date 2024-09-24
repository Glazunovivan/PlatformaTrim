using System;
using System.Configuration;

namespace PlatformaTrim.DigitalSignature
{
    /// <summary>
    /// Шаблон нижнего колонтитула цифровой подписи
    /// </summary>
    public class DigitalSignatureFooterTemplate
    {
        private const string _titleKey = "FooterTitleText";
        private const string _headerNameKey = "FooterHeaderName";
        private const string _headerNumberCertificateKey = "FooterHeaderNumberCertificate";
        private const string _headerDateKey = "FooterHeaderDate";
        private const string _marginKey = "FooterMargin";
        private const string _fontSizeKey = "FooterFontSize";

        public string Title { get; init; }
        public string Certificate { get; init; }
        public string Name { get; init; }
        public string Date { get; init; }

        public float Margin { get; init; }
        public float FontSize { get; init; }

        public string Text { get; init; }

        public DigitalSignatureFooterTemplate(string number, string fullname, DateTime dateStart, DateTime dateEnd)
        {
            Title = ConfigurationManager.AppSettings.Get(_titleKey);
            Name = $"{ConfigurationManager.AppSettings.Get(_headerNameKey)}: {fullname}";
            Certificate = $"{ConfigurationManager.AppSettings.Get(_headerNumberCertificateKey)}: {number}";
            Date = $"{ConfigurationManager.AppSettings.Get(_headerDateKey)}: с { dateStart.Date.ToString("dd/MM/yyyy")} по {dateEnd.Date.ToString("dd/MM/yyyy")}";
            Margin = float.Parse(ConfigurationManager.AppSettings.Get(_marginKey));
            FontSize = float.Parse(ConfigurationManager.AppSettings.Get(_fontSizeKey));

            Text = $"{Title}. {Certificate}. {Name}. {Date}";
        }
    }
}
