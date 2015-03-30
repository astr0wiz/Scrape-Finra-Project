using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.IO;
using ScrapeFinra.Models;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace ScrapeFinra
{
    public partial class MainForm : Form
    {
        public WebBrowser browseList;
        public WebBrowser browseDetails;
        public Dictionary<string, int> States;
        public int detailPtr;
        public FinraDataSet fdset;

        public MainForm()
        {
            InitializeComponent();
            States = new Dictionary<string, int>();
            States.Add("Tennessee", 48);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetFeatureBrowserEmulation();
            browseList = new WebBrowser();
            browseDetails = new WebBrowser();
            browseList.ScriptErrorsSuppressed = true;
            browseList.DocumentCompleted += browseList_NavFetchCompleted;
            browseDetails.DocumentCompleted += browseDeatils_NavFetchCompleted;
            txtIssuer.Text = "Memphis";
            foreach (string key in States.Keys)
            {

                lstState.Items.Add(key);
            }
            lstResults.Items.Add("B");
            lstDAClass.Items.Add("4");
            lstDAClass.SelectedIndex = 0;
            lstResults.SelectedIndex = 0;
            lstState.SelectedIndex = 0;
            fdset = new FinraDataSet();
            LogIt("Initiated.");
        }

        private void browseList_NavFetchCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (browseList.DocumentText.Trim().Length > 0)
            {
                browseList.DocumentCompleted -= browseList_NavFetchCompleted;
                string oldName = string.Format("{{{0}:{{", lstResults.SelectedItem);
                string newName = string.Format("{{\"{0}\":{{", lstResults.SelectedItem);
                string jsonText = browseList.DocumentText.Replace(oldName, newName);
                fdset = JsonConvert.DeserializeObject<FinraDataSet>(jsonText);
                //System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(fdset, Formatting.Indented));
                BuildDetails(fdset.finraData.Columns);
            }
            else
            {
                LogIt("Criteria returned no information!");
            }
        }

        private void browser_DetailsFetchCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Regex rx = new Regex(@"Finra_bond_detail_url[ ]*=[ ]*'(.*)';");
            Match match = rx.Match(browseList.DocumentText);
            if (match.Success)
            {
                browseDetails.Navigate("http:" + match.Groups[1].Value);
            }
            else
            {
                LogIt("Detail not found!");
            }
        }

        private void browseDeatils_NavFetchCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.Load(browseDetails.DocumentStream);
            LogIt("Scraping...");
            htmlDoc.
        }

        private string GetHomeURL()
        {
            StringBuilder url = new StringBuilder();
            url.Append("http://finra-markets.morningstar.com/bondSearch.jsp");
            url.Append(string.Format("?count={0}", udNumReturned.Value));
            url.Append(string.Format("&searchtype={0}", lstResults.SelectedItem));
            url.Append("&sortfield=issuerName");
            url.Append("&sorttype=1");
            url.Append("&start=0");
            url.Append("&curPage=1&query=");
            url.Append(WebUtility.UrlEncode(string.Format("{{\"Keywords\":[{{\"Name\":\"debtOrAssetClass\",\"Value\":\"{0}\"}},", lstDAClass.SelectedItem)));
            url.Append(WebUtility.UrlEncode(string.Format("{{\"Name\":\"showResultsAs\",\"Value\":\"{0}\"}},", lstResults.SelectedItem)));
            url.Append(WebUtility.UrlEncode(string.Format("{{\"Name\":\"state\",\"Value\":\"{0}\"}},", States[lstState.SelectedItem.ToString()])));
            url.Append(WebUtility.UrlEncode(string.Format("{{\"Name\":\"issuerName\",\"Value\":\"{0}\"}}]}}", txtIssuer.Text)));
            return (url.ToString());
            //return ("http://finra-markets.morningstar.com/bondSearch.jsp?count=2&searchtype=B&query=%7B%22Keywords%22%3A%5B%7B%22Name%22%3A%22debtOrAssetClass%22%2C%22Value%22%3A%224%22%7D%2C%7B%22Name%22%3A%22showResultsAs%22%2C%22Value%22%3A%22B%22%7D%2C%7B%22Name%22%3A%22state%22%2C%22Value%22%3A%2248%22%7D%2C%7B%22Name%22%3A%22issuerName%22%2C%22Value%22%3A%22Memphis%22%7D%5D%7D&sortfield=issuerName&sorttype=1&start=0&curPage=1");
            //return ("http://finra-markets.morningstar.com/bondSearch.jsp?count=800&searchtype=B&sortfield=issuerName&sorttype=1&start=0&curPage=1&query%3D%7B%22Keywords%22%3A%5B%7B%22Name%22%3A%22debtOrAssetClass%22%2C%22Value%22%3A%224%22%7D%2C%7B%22Name%22%3A%22showResultsAs%22%2C%22Value%22%3A%22B%22%7D%2C%7B%22Name%22%3A%22state%22%2C%22Value%22%3A%2248%22%7D%2C%7B%22Name%22%3A%22issuerName%22%2C%22Value%22%3A%22Memphis%22%7D%5D%7D");

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (browseList.IsBusy)
            {
                browseList.Stop();
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            browseList.Navigate(GetHomeURL());
            LogIt("Retrieving list...");
        }

        private void BuildDetails(List<Bond> bonds)
        {
            browseList.DocumentCompleted += browser_DetailsFetchCompleted;
            if (bonds.Count > 0)
            {
                detailPtr = 0;
                FetchDetail(detailPtr, bonds[detailPtr]);
            }
        }

        private void FetchDetail(int ptr,Bond bond)
        {
            string url = "http://finra-markets.morningstar.com/BondCenter/BondDetail.jsp?ticker={0}&symbol={1}";
            browseList.Navigate(string.Format(url, bond.securityId, bond.symbol));
            LogIt(string.Format("Fetching detail {0} of {1}   [{2}]", ptr, fdset.finraData.Rows, fdset.finraData.Columns[ptr].securityId));
        }

        // enable HTML5 (assuming we're running IE10+)
        // more info: http://stackoverflow.com/a/18333982/1768303
        static void SetFeatureBrowserEmulation()
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
                return;
            var appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                appName, 10000, RegistryValueKind.DWord);
        }


        private void LogIt(string message)
        {
            txtLog.Text += message + Environment.NewLine;
        }

    }
}
