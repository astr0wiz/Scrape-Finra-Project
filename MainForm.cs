using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ScrapeFinra.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace ScrapeFinra
{
    public partial class MainForm : Form
    {
        private WebBrowser c_browser;
        private WebRequest c_requestAPIs;
        private Dictionary<string, int> c_States;
        private int c_detailPtr;
        private FinraDataSet c_fdset;
        private string c_currentState;
        private MatchCollection c_apiMatches;
        private List<FinraReportItem> c_reportItems;
        private FinraReportItem c_rptItem;

        public MainForm()
        {
            InitializeComponent();
            c_States = new Dictionary<string, int>();
            c_States.Add("Tennessee", 48);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            c_browser = new WebBrowser();
            c_browser.ScriptErrorsSuppressed = true;
            txtIssuer.Text = "Memphis";
            foreach (string key in c_States.Keys)
            {

                lstState.Items.Add(key);
            }
            lstResults.Items.Add("B");
            lstDAClass.Items.Add("4");
            lstDAClass.SelectedIndex = 0;
            lstResults.SelectedIndex = 0;
            lstState.SelectedIndex = 0;
            c_fdset = new FinraDataSet();
            c_currentState = common.STATE_IDLE;
            LogIt("Initiated.");
        }

        private void browser_NavCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Regex rx;
            c_browser.DocumentCompleted -= browser_NavCompleted;
            switch (c_currentState)
            {
                case common.STATE_LINKSET:
                    if (c_browser.DocumentText.Trim().Length > 0)
                    {
                        //
                        // The set of links (all bonds) comes back as JSON data.
                        // Root object is code for the requested results ("B" = bonds)
                        //
                        string oldName = string.Format("{{{0}:{{", lstResults.SelectedItem);
                        string newName = string.Format("{{\"{0}\":{{", lstResults.SelectedItem);
                        string jsonText = c_browser.DocumentText.Replace(oldName, newName);
                        try
                        {
                            c_fdset = JsonConvert.DeserializeObject<FinraDataSet>(jsonText);
                            c_reportItems = new List<FinraReportItem>();
                            c_detailPtr = 0;
                            if (c_fdset.finraData.Columns.Count > 0)
                            {
                                ProcessState(common.STATE_ITEM);
                            }
                        }
                        catch (Newtonsoft.Json.JsonReaderException ex)
                        {
                            Trace.WriteLine(string.Format("-!Json Reader Error! : {0}", ex.Message));
                        }
                    }
                    else
                    {
                        LogIt("Criteria returned no information!");
                    }
                    break;
                case common.STATE_ITEM:
                    rx = new Regex(@"Finra_bond_detail_url[ ]*=[ ]*'(.*)';");
                    Match match = rx.Match(c_browser.DocumentText);
                    if (match.Success)
                    {
                        c_rptItem = new FinraReportItem(c_fdset.finraData.Columns[c_detailPtr].descriptionOfIssuer);
                        ProcessState(common.STATE_BASEAPI, new object[] { "http:" + match.Groups[1].Value });
                    }
                    else
                    {
                        LogIt("Item base not found!");
                    }
                    break;
                case common.STATE_BASEAPI:
                    rx = new Regex(@"\s*tempReqUrl\s*=\s*""(?<api>.*?quote/c-(banner|tax).*?callback=).*", RegexOptions.ExplicitCapture);
                    if (rx.IsMatch(c_browser.DocumentText))
                    {
                        c_apiMatches = rx.Matches(c_browser.DocumentText);
                        ProcessState(common.STATE_APIBANNER, new object[] { c_apiMatches[0].Groups["api"].Value });
                    }
                    break;
                default:
                    LogIt(string.Format(" ** Invalid state: {0}! **", c_currentState));
                    break;
            }
        }

        private string GetAPIData(string uri)
        {
            c_requestAPIs = WebRequest.Create("http:" + uri);
            c_requestAPIs.Method = "GET";
            WebResponse _response = c_requestAPIs.GetResponse();
            if (((HttpWebResponse)_response).StatusCode == HttpStatusCode.OK)
            {
                using (Stream _datastream = _response.GetResponseStream())
                {
                    using (StreamReader _datareader = new StreamReader(_datastream))
                    {
                        APIData _apiData = JsonConvert.DeserializeObject<APIData>(_datareader.ReadToEnd());
                        return _apiData.html;
                    }
                }
            }
            else
            {
                LogIt("-! Bad response from API call: " + ((HttpWebResponse)_response).StatusCode.ToString());
                return "";
            }
        }

        private void ScrapeDetails(string documentText)
        {
            HtmlAgilityPack.HtmlNodeCollection nodes;
            if (documentText != null)
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(Utilities.WrapPartialHtml(documentText));

                Trace.WriteLine(Utilities.WrapPartialHtml(documentText));

                switch (c_currentState)
                {
                    case common.STATE_APIBANNER:
                        string reportlink = htmlDoc.DocumentNode.SelectSingleNode("//a[@id='report_link']").Attributes["href"].Value;
                        if (!Utilities.ScrapeCUSIP(reportlink, c_rptItem))
                        {
                            LogIt("-! Warning: CUSIP not found");
                        }
                        nodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='market_wrapper']/div/div[@class='gr_text_bigprice']");
                        if (!Utilities.ScrapeCouponRate(nodes[0].InnerHtml.Trim(), c_rptItem))
                        {
                            LogIt("-! Warning: Coupon Rate not found");
                        }
                        if (!Utilities.ScrapeMaturityDate(nodes[1].InnerHtml.Trim(), c_rptItem))
                        {
                            LogIt("-! Warning: Maturity Date not found");
                        }
                        nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='gr_colm_a2b gr_text1']/table/tbody/tr/td/span");
                        if (!Utilities.ScrapeNextCallDate(nodes[1].InnerHtml.Trim(), c_rptItem))
                        {
                            LogIt("-! Warning: Next Call Date not found");
                        }
                        if (!Utilities.ScrapeLastTradePrice(nodes[3].InnerHtml.Trim(), c_rptItem))
                        {
                            LogIt("-! Warning: Next Call Date not found");
                        }

                        ProcessState(common.STATE_APITAX, new object[] { c_apiMatches[1].Groups["api"].Value });
                        break;
                    case common.STATE_APITAX:
                        // get nodes
                        // scrape taxable
                        // scrape bankqualified
                        // process state STATE_STORE (here we store in the list and add to the totals)
                        break;
                    default:
                        break;
                }
            }
            else
            {
                LogIt(string.Format("-! No details fetched for {0}", c_currentState));
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            ProcessState(common.STATE_LINKSET);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (c_browser.IsBusy)
            {
                c_browser.Stop();
            }
        }

        private void ProcessState(string appState, object[] args = null)
        {
            c_currentState = appState;
            switch (appState)
            {
                case common.STATE_LINKSET:
                    c_browser.DocumentCompleted += browser_NavCompleted;
                    c_browser.Navigate(Utilities.GetListURL(new object[]{
                        udNumReturned.Value,
                        lstResults.SelectedItem,
                        lstDAClass.SelectedItem,
                        c_States[lstState.SelectedItem.ToString()],
                        txtIssuer.Text
                    }));
                    LogIt("Retrieving list...");
                    break;
                case common.STATE_ITEM:
                    c_browser.DocumentCompleted += browser_NavCompleted;
                    string url = "http://finra-markets.morningstar.com/BondCenter/BondDetail.jsp?ticker={0}&symbol={1}";
                    c_browser.Navigate(string.Format(url, c_fdset.finraData.Columns[c_detailPtr].securityId, c_fdset.finraData.Columns[c_detailPtr].symbol));
                    LogIt(string.Format("Fetching detail {0} of {1}   [{2}]", c_detailPtr, c_fdset.finraData.Rows, c_fdset.finraData.Columns[c_detailPtr].securityId));
                    break;
                case common.STATE_BASEAPI:
                    c_browser.DocumentCompleted += browser_NavCompleted;
                    c_browser.Navigate(args[0].ToString());
                    LogIt("Getting data...", false);
                    break;
                case common.STATE_APIBANNER:
                    LogIt("...banner...", false);
                    ScrapeDetails(GetAPIData(args[0].ToString()));
                    break;
                case common.STATE_APITAX:
                    LogIt("...taxes...", true);
                    ScrapeDetails(GetAPIData(args[0].ToString()));
                    break;
                case common.STATE_STORE:
                    // put the rptItem into reportList
                    // call incrementer for requested summaries
                    // add 1 to c_detailPtr
                    // call reset func to drop and create WebBrowser control, and call process state [gotta figure out how to do that async -- maybe Invoke?]
                    break;
                default:
                    // STATE_IDLE
                    break;
            }
        }


        private void LogIt(string message, bool hasNewLine = true)
        {
            txtLog.Text += message + (hasNewLine ? Environment.NewLine : "");
        }

    }
}
