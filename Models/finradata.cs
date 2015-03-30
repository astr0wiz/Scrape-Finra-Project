﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScrapeFinra.Models

{
    class FinraData
    {
        [JsonProperty("Columns")]
        public List<Bond> Columns { get; set; }
        public int Rows { get; set; }
        public int Count { get; set; }
        public bool hasData { get; set; }
        public string errorMsg { get; set; }
    }

    class FinraDataSet
    {
        [JsonProperty("B")]
        public FinraData finraData;
    }

}