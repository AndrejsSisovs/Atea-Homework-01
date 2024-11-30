namespace Atea.Scraper_01
{
    public class CountryDescription
    {
        public required string Common { get; set; }
        public required string Official { get; set; }
        public required Dictionary<string, TranslationDescriptor> NativeName { get; set; }
    }
}
  
