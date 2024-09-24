using System.Configuration;

namespace PlatformaTrim.DigitalSignature
{
    public class DigitalSignatureGeneralSettings
    {
        private const string _fontPathKey = "PathTrueTypeFont";
        private const string _fontNameKey = "NameTrueTypeFont";
        private const string _languageCodeKey = "LanguageCode";

        public string FontPath { get; set; }
        public string FontName { get; set; }
        public string LanguageCode { get; set; }

        public DigitalSignatureGeneralSettings()
        {
            FontPath = ConfigurationManager.AppSettings.Get(_fontPathKey);
            FontName = ConfigurationManager.AppSettings.Get(_fontNameKey);
            LanguageCode = ConfigurationManager.AppSettings.Get(_languageCodeKey);
        }
    }
}
