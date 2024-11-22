﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atea.Scraper_01
{
    public class CountryDescription
    {
        public required string Common {  get; set; }
        public required string Official { get; set; }
        public required Dictionary<string, TranslationDescriptor> Country { get; set; }
    }
}
