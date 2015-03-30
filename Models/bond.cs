using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrapeFinra.Models;
using Newtonsoft.Json;

namespace ScrapeFinra.Models
{
    class Bond
    {
        public Rating moodyRating { get; set; }
        public Rating fitchRating { get; set; }
        public Rating standardAndPoorRating { get; set; }
        public string couponType { get; set; }
        public string priceChange { get; set; }
        public string priceChangePercent { get; set; }
        public string codeDebtOrAssetClass { get; set; }
        public string debtOrAssetClass { get; set; }
        public string securityId { get; set; }
        public string issueIdentifier { get; set; }
        public string descriptionOfIssuer { get; set; }
        public string subproductType { get; set; }
        public string couponRate { get; set; }
        public string maturityDate { get; set; }
        public string price { get; set; }
        public string yield { get; set; }
        public string tradeDate { get; set; }
        public string symbol { get; set; }
        public string cusip { get; set; }
        public string callable { get; set; }
    }
}
