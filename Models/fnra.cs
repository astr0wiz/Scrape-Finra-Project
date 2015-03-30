using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeFinra.Models
{
    class fnra
    {
        string cusip { get; set; }
        string Description { get; set; }
        decimal Coupon { get; set; }
        DateTime MaturityDate { get; set; }
        decimal LastPrice { get; set; }
        DateTime? NextCallDate { get; set; }
        Boolean isTaxable { get; set; }
        Boolean isBankQualified { get; set; }
    }
}
