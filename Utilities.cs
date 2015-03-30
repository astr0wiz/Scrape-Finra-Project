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

        public static string CleanJsonifiedHtml(string jsonValue)
        {
            string retval = "";
            Regex rx = new Regex(@"<!DOCTYPE.*?>");
            Regex rxBody = new Regex(@"<body.*body>", RegexOptions.Singleline);
            Regex rxStyle = new Regex(@"<style.*?style>", RegexOptions.Singleline);
            Regex rxHead = new Regex(@"<head.*?head>", RegexOptions.Singleline);
            if (rx.IsMatch(jsonValue))
            {
                retval = rx.Replace(jsonValue, "");
                retval = rxBody.Replace(retval, "<body></body>");
                retval = rxStyle.Replace(retval, "");
                retval = rxHead.Replace(retval, "");
                Trace.WriteLine("NO DOCTYPE OR BODY OR HEAD OR STYLE: " + retval);
            }
            return retval;
        }
    }
}
