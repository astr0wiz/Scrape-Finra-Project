using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ScrapeFinra.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace ScrapeFinra
{
    public partial class MainForm : Form
    {
        private WebBrowser c_browser;
        private WebBrowser c_browserAPIs;
        private Dictionary<string, int> c_States;
        private int c_detailPtr;
        private FinraDataSet c_fdset;
        private string c_currentState;
        private MatchCollection apiMatches;

        public MainForm()
        {
            InitializeComponent();
            c_States = new Dictionary<string, int>();
            c_States.Add("Tennessee", 48);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            c_browser = new WebBrowser();
            c_browserAPIs = new WebBrowser();
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
                    Regex rx = new Regex(@"Finra_bond_detail_url[ ]*=[ ]*'(.*)';");
                    Match match = rx.Match(c_browser.DocumentText);
                    if (match.Success)
                    {
                        ProcessState(common.STATE_BASEAPI, new object[] { "http:" + match.Groups[1].Value });
                    }
                    else
                    {
                        LogIt("Item base not found!");
                    }
                    break;
                default:
                    LogIt(string.Format(" ** Invalid state: {0}! **", c_currentState));
                    break;
            }
        }

        private void browserAPIs_NavCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            //htmlDoc.Load(browserAPIs.DocumentStream);
            c_browserAPIs.DocumentCompleted -= browserAPIs_NavCompleted;
            switch (c_currentState)
            {
                case common.STATE_BASEAPI:
                    Regex rx = new Regex(@"\s*tempReqUrl\s*=\s*""(?<api>.*?quote/c-(banner|tax).*?callback=).*", RegexOptions.ExplicitCapture);
                    if (rx.IsMatch(c_browserAPIs.DocumentText))
                    {
                        apiMatches = rx.Matches(c_browserAPIs.DocumentText);
                        ProcessState(common.STATE_APIBANNER, new object[] { apiMatches[0].Groups["api"].Value });
                    }
                    break;
                case common.STATE_APIBANNER:
                    break;
                case common.STATE_APITAX:
                    break;
                default:
                    break;
            }

        }

        private void ScheduleDetailScrapers(MatchCollection APIcalls)
        {
            throw new NotImplementedException();
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
                    c_browserAPIs.DocumentCompleted += browserAPIs_NavCompleted;
                    c_browserAPIs.Navigate(args[0].ToString());
                    LogIt("Getting data...", false);
                    break;
                case common.STATE_APIBANNER:
                    c_browserAPIs.DocumentCompleted += browserAPIs_NavCompleted;
                    c_browserAPIs.Navigate("http:" + args[0].ToString());
                    LogIt("...banner...", false);
                    break;
                case common.STATE_APITAX:
                    c_browserAPIs.DocumentCompleted += browserAPIs_NavCompleted;
                    c_browserAPIs.Navigate("http:" + args[0].ToString());
                    LogIt("...taxes...", true);
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
