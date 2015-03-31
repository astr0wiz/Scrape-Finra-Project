using ScrapeFinra.Models;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ScrapeFinra
{
    static class Utilities
    {
        public static string GetListURL(object[] args)
        {
            StringBuilder url = new StringBuilder();
            url.Append("http://finra-markets.morningstar.com/bondSearch.jsp");
            url.Append(string.Format("?count={0}", args[0]));                     // udNumReturned.Value
            url.Append(string.Format("&searchtype={0}", args[1]));                // lstResults.SelectedItem (used twice here)
            url.Append("&sortfield=issuerName");
            url.Append("&sorttype=1");
            url.Append("&start=0");
            url.Append("&curPage=1&query=");
            url.Append(WebUtility.UrlEncode(string.Format("{{\"Keywords\":[{{\"Name\":\"debtOrAssetClass\",\"Value\":\"{0}\"}},", args[2])));  // lstDAClass.SelectedItem
            url.Append(WebUtility.UrlEncode(string.Format("{{\"Name\":\"showResultsAs\",\"Value\":\"{0}\"}},", args[1])));                     // lstResults.SelectedItem
            url.Append(WebUtility.UrlEncode(string.Format("{{\"Name\":\"state\",\"Value\":\"{0}\"}},", args[3])));                             // States[lstState.SelectedItem.ToString()]
            url.Append(WebUtility.UrlEncode(string.Format("{{\"Name\":\"issuerName\",\"Value\":\"{0}\"}}]}}", args[4])));                      // txtIssuer.Text
            return (url.ToString());

        }

        public static string WrapPartialHtml(string partialHtml)
        {
            StringBuilder newHtml = new StringBuilder();
            newHtml.Append(partialHtml);
            if (!partialHtml.ToLower().Contains("<body"))
            {
                newHtml.Insert(0, "<body>");
                newHtml.Append("</body>");
            }
            if (!partialHtml.ToLower().Contains("<html"))
            {
                newHtml.Insert(0, "<html>");
                newHtml.Append("</html>");
            }
            return newHtml.ToString();
        }

        public static bool ScrapeCUSIP(string reportlink, FinraReportItem rptItem)
        {
            Regex rx = new Regex(@".*cusip=(.{9})");
            if (rx.IsMatch(reportlink))
            {
                Match match = rx.Match(reportlink);
                rptItem.CUSIP = match.Groups[1].Value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ScrapeCouponRate(string coupon, FinraReportItem rptItem)
        {
            rptItem.Coupon = coupon;
            return true;
        }

        public static bool ScrapeMaturityDate(string maturityDate, FinraReportItem rptItem)
        {
            rptItem.MaturityDate = maturityDate.ToLower().Equals("&mdash;") ? "" : maturityDate;
            return true;
        }

        public static bool ScrapeNextCallDate(string calldate, FinraReportItem rptItem)
        {
            rptItem.NextCallDate = calldate.ToLower().Equals("&mdash;") ? "" : calldate;
            return true;
        }

        public static bool ScrapeLastTradePrice(string tradeprice, FinraReportItem rptItem)
        {
            rptItem.LastPrice = tradeprice;
            return true;
        }

        public static bool ScrapeOriginalOffering(string originaloffering, FinraReportItem rptItem)
        {
            rptItem.OriginalOffering = originaloffering;
            return true;
        }

        public static bool ScrapeBankQualified(string bankqualified, FinraReportItem rptItem)
        {
            rptItem.BankQualified = bankqualified.Equals("yes", System.StringComparison.CurrentCultureIgnoreCase) ? true : false;
            return true;
        }

        public static bool ScrapeTaxableFederal(string taxablefederal, FinraReportItem rptItem)
        {
            rptItem.Taxable = taxablefederal.Equals("yes", System.StringComparison.CurrentCultureIgnoreCase) ? true : false;
            return true;
        }
    }
}
