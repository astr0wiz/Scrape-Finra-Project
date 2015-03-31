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
        private FinraStatistics c_Stats;
        private decimal c_weightedTotal;
        private decimal c_weightedCount;
        public delegate void StateProcessor(string s, object[] o);
        public StateProcessor stateMachine;
        public delegate void NextItemProcessor();
        public NextItemProcessor nextBond;
        private string c_currentBaseAPIUri;

        public MainForm()
        {
            InitializeComponent();
            c_States = new Dictionary<string, int>();
            c_States.Add("Tennessee", 48);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            stateMachine = new StateProcessor(ProcessState);
            nextBond = new NextItemProcessor(ProcessNextBond);
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
                            if (c_fdset.finraData.Columns.Count > 0)
                            {
                                ProcessState(common.STATE_ITEM);
                            }
                        }
                        catch (Newtonsoft.Json.JsonReaderException ex)
                        {
                            LogIt(string.Format("-! Json Reader Error! : {0}", ex.Message));
                        }
                    }
                    else
                    {
                        LogIt("-! Criteria returned no information !-");
                    }
                    break;
                case common.STATE_ITEM:
                    rx = new Regex(@"Finra_bond_detail_url[ ]*=[ ]*'(.*)';");
                    Match match = rx.Match(c_browser.DocumentText);
                    if (match.Success)
                    {
                        c_rptItem = new FinraReportItem(c_fdset.finraData.Columns[c_detailPtr].descriptionOfIssuer);
                        c_currentBaseAPIUri = "http:" + match.Groups[1].Value;
                        ProcessState(common.STATE_BASEAPI, new object[] { c_currentBaseAPIUri });
                    }
                    else
                    {
                        LogIt("-! Item base not found !-");
                    }
                    break;
                case common.STATE_BASEAPI:
                    rx = new Regex(@"\s*tempReqUrl\s*=\s*""(?<api>.*?quote/c-(banner|tax|issue).*?callback=).*", RegexOptions.ExplicitCapture);
                    if (rx.IsMatch(c_browser.DocumentText))
                    {
                        c_apiMatches = rx.Matches(c_browser.DocumentText);
                        ProcessState(common.STATE_APIBANNER, new object[] { c_apiMatches[0].Groups["api"].Value });
                    }
                    else
                    {
                        LogIt("-! Base API data not found (retrying) !-");
                        ProcessState(common.STATE_BASEAPI, new object[] { c_currentBaseAPIUri });
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
            try
            {
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
            catch (Exception ex)
            {
                LogIt("-! ERROR requesting API data  !-");
                LogIt(ex.Message);
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

                        ProcessState(common.STATE_APITAX, new object[] { c_apiMatches[2].Groups["api"].Value });
                        break;
                    case common.STATE_APITAX:
                        nodes = htmlDoc.DocumentNode.SelectNodes("//table/tbody/tr/td[@align='right']");
                        if (!Utilities.ScrapeTaxableFederal(nodes[0].InnerHtml.Trim(), c_rptItem))
                        {
                            LogIt("-! Warning: Federal Tax indicator not found");
                        }
                        if (!Utilities.ScrapeBankQualified(nodes[2].InnerHtml.Trim(), c_rptItem))
                        {
                            LogIt("-! Warning: Bank Qualified indicator not found");
                        }
                        ProcessState(common.STATE_ISSUE, new object[] { c_apiMatches[1].Groups["api"].Value });
                        break;
                    case common.STATE_ISSUE:
                        nodes = htmlDoc.DocumentNode.SelectNodes("//table/tbody/tr/td[@align='right']");
                        if (nodes != null)
                        {
                            if (!Utilities.ScrapeOriginalOffering(nodes[3].InnerHtml.Trim(), c_rptItem))
                            {
                                LogIt("-! Warning: Original Offering not found");
                            }
                        }
                        else
                        {
                            LogIt("-! Warning: Issue Response empty (retrying after 1 second) !-");
                            System.Threading.Thread.Sleep(1000);
                            ProcessState(common.STATE_ISSUE, new object[] { c_apiMatches[1].Groups["api"].Value });
                        }
                        ProcessState(common.STATE_STORE);
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
            if (txtOutputFileName.Text.EndsWith(".csv"))
            {
                txtLog.Clear();
                LogIt(string.Format("Initiated : {0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                btnGetData.Enabled = false;
                ProcessState(common.STATE_LINKSET);
            }
            else
            {
                MessageBox.Show("Please specify an output file before processing.", "Output File Needed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                txtOutputFileName.Focus();
            }
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
                    c_detailPtr = -1;
                    c_weightedCount = 0;
                    c_weightedTotal = 0.0M;
                    c_Stats = new FinraStatistics();
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
                    c_detailPtr++;
                    while (c_browser.IsBusy) { }
                    c_browser.DocumentCompleted += browser_NavCompleted;
                    string url = "http://finra-markets.morningstar.com/BondCenter/BondDetail.jsp?ticker={0}&symbol={1}";
                    c_browser.Navigate(string.Format(url, c_fdset.finraData.Columns[c_detailPtr].securityId, c_fdset.finraData.Columns[c_detailPtr].symbol));
                    LogIt(string.Format("Fetching detail {0} of {1}   [{2}]", c_detailPtr + 1, c_fdset.finraData.Rows, c_fdset.finraData.Columns[c_detailPtr].securityId));
                    lblProgress.Text = string.Format("{0} / {1}", c_detailPtr + 1, c_fdset.finraData.Rows);
                    break;
                case common.STATE_BASEAPI:
                    while (c_browser.IsBusy) { }
                    c_browser.DocumentCompleted += browser_NavCompleted;
                    c_browser.Navigate(args[0].ToString());
                    LogIt("Getting data...", false);
                    break;
                case common.STATE_APIBANNER:
                    LogIt("...banner...", false);
                    ScrapeDetails(GetAPIData(args[0].ToString()));
                    break;
                case common.STATE_APITAX:
                    LogIt("...taxes...", false);
                    ScrapeDetails(GetAPIData(args[0].ToString()));
                    break;
                case common.STATE_ISSUE:
                    LogIt("...issue...", true);
                    ScrapeDetails(GetAPIData(args[0].ToString()));
                    break;
                case common.STATE_STORE:
                    c_reportItems.Add(c_rptItem);
                    Invoke(nextBond);
                    break;
                case common.STATE_REPORT:
                    LogIt("Building CSV file...");
                    BuildReportOutput();
                    break;
                case common.STATE_IDLE:
                    btnGetData.Enabled = true;
                    break;
                default:
                    // STATE_UNKNOWN
                    break;
            }
        }

        private void ProcessNextBond()
        {
            decimal lastPrice = 0;
            decimal couponValue = 0;
            decimal offering = 0;
            c_Stats.FederallyTaxableCount += c_rptItem.Taxable == true ? 1 : 0;
            c_Stats.BankQualifiedCount += c_rptItem.BankQualified == true ? 1 : 0;
            if (decimal.TryParse(c_rptItem.LastPrice, System.Globalization.NumberStyles.Currency, null, out lastPrice))
            {
                c_Stats.BondCountBelow100 += lastPrice < 100.00M ? 1 : 0;
            }
            if (decimal.TryParse(c_rptItem.Coupon, System.Globalization.NumberStyles.Float, null, out couponValue))
            {
                c_Stats.BondCountCouponOver5 += couponValue < 5.00M ? 1 : 0;
            }
            if (decimal.TryParse(c_rptItem.OriginalOffering, System.Globalization.NumberStyles.Currency, null, out offering))
            {
                if (offering > 0.0M && couponValue > 0.0M)
                {
                    c_weightedTotal += offering * couponValue;
                    c_weightedCount += offering;
                }
            }
            if (c_detailPtr < c_fdset.finraData.Rows - 1)
            {
                // do we need to drop/create the WebBrowser?
                ProcessState(common.STATE_ITEM);
            }
            else
            {
                ProcessState(common.STATE_REPORT);
            }
        }

        private void BuildReportOutput()
        {
            if (c_reportItems.Count > 0)
            {
                using (StreamWriter writer = new StreamWriter(txtOutputFileName.Text, false))
                {
                    writer.WriteLine("CUSIP,Description,Coupon,Maturity Date,Last Price,Next Call Date,Taxable,Bank Qualified");
                    for (int i = 0; i < c_reportItems.Count; i++)
                    {
                        writer.WriteLine(string.Format("\"{0}\",\"{1}\",{2},{3},{4},{5},{6},{7}",
                            c_reportItems[i].CUSIP,
                            c_reportItems[i].Description,
                            c_reportItems[i].Coupon,
                            c_reportItems[i].MaturityDate,
                            c_reportItems[i].LastPrice,
                            c_reportItems[i].NextCallDate,
                            c_reportItems[i].Taxable,
                            c_reportItems[i].BankQualified
                            ));
                    }
                    writer.WriteLine(string.Format("\"Total # of Federally taxable bonds\",{0},,,,,,", c_Stats.FederallyTaxableCount));
                    writer.WriteLine(string.Format("\"Total # of Bank Qualified bonds\",{0},,,,,,", c_Stats.BankQualifiedCount));
                    writer.WriteLine(string.Format("\"Total # of bonds with a last price below $100\",{0},,,,,,", c_Stats.BondCountBelow100));
                    writer.WriteLine(string.Format("\"Total # of bonds with a coupon above 5%\",{0},,,,,,", c_Stats.BondCountCouponOver5));
                    writer.WriteLine(string.Format("\"Weighted Avg Coupon\",{0:0.000},,,,,,", c_weightedTotal / c_weightedCount));
                }
                LogIt(string.Format("Done : {0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                ProcessState(common.STATE_IDLE);
            }
        }

        private void LogIt(string message, bool hasNewLine = true)
        {
            txtLog.AppendText(message + (hasNewLine ? Environment.NewLine : ""));
        }

        private void btnFileDialog_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.OverwritePrompt = true;
            dlg.DefaultExt = "csv";
            dlg.Filter = "CSV Files (*.csv)|*.csv";
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtOutputFileName.Text = dlg.FileName;
            }
        }
    }
}
