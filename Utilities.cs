using System.Net;
using System.Text;

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

    }
}
