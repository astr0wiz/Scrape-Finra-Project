using System.Collections.Generic;
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

    class FinraReportItem
    {
        public string CUSIP { get; set; }
        public string Description { get; set; }
        public string Coupon { get; set; }
        public string MaturityDate { get; set; }
        public string LastPrice { get; set; }
        public string NextCallDate { get; set; }
        public bool Taxable { get; set; }
        public bool BankQualified { get; set; }
        public string OriginalOffering { get; set; }

        public FinraReportItem()
        {
            CUSIP = "";
            Description = "";
            Coupon = "";
            MaturityDate = "";
            LastPrice = "";
            NextCallDate = "";
            Taxable = false;
            BankQualified = false;
            OriginalOffering = "";
        }

        public FinraReportItem(string description)
        {
            CUSIP = "";
            Description = description;
            Coupon = "";
            MaturityDate = "";
            LastPrice = "";
            NextCallDate = "";
            Taxable = false;
            BankQualified = false;
            OriginalOffering = "";
        }
    }

    class FinraStatistics
    {
        public int FederallyTaxableCount { get; set; }
        public int BankQualifiedCount { get; set; }
        public int BondCountBelow100 { get; set; }
        public int BondCountCouponOver5 { get; set; }

        public FinraStatistics()
        {
            FederallyTaxableCount = 0;
            BankQualifiedCount = 0;
            BondCountBelow100 = 0;
            BondCountCouponOver5 = 0;
        }
    }

}
