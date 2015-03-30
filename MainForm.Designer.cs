namespace ScrapeFinra
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lstState = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtIssuer = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lstTCB = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lstResults = new System.Windows.Forms.ListBox();
            this.lstSPDS = new System.Windows.Forms.ListBox();
            this.lstDAClass = new System.Windows.Forms.ListBox();
            this.btnGetData = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.udNumReturned = new System.Windows.Forms.NumericUpDown();
            this.txtLog = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.udNumReturned)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Debt or Asset Class";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Show Results As";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "State";
            // 
            // lstState
            // 
            this.lstState.FormattingEnabled = true;
            this.lstState.ItemHeight = 16;
            this.lstState.Location = new System.Drawing.Point(169, 103);
            this.lstState.Name = "lstState";
            this.lstState.Size = new System.Drawing.Size(120, 20);
            this.lstState.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(335, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "Issuer";
            // 
            // txtIssuer
            // 
            this.txtIssuer.Location = new System.Drawing.Point(509, 49);
            this.txtIssuer.Name = "txtIssuer";
            this.txtIssuer.Size = new System.Drawing.Size(162, 22);
            this.txtIssuer.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(335, 79);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(169, 17);
            this.label6.TabIndex = 12;
            this.label6.Text = "Trace/CUSIP/Blooomberg";
            // 
            // lstTCB
            // 
            this.lstTCB.FormattingEnabled = true;
            this.lstTCB.ItemHeight = 16;
            this.lstTCB.Location = new System.Drawing.Point(509, 77);
            this.lstTCB.Name = "lstTCB";
            this.lstTCB.Size = new System.Drawing.Size(120, 20);
            this.lstTCB.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(335, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 17);
            this.label7.TabIndex = 14;
            this.label7.Text = "SPDS Type";
            // 
            // lstResults
            // 
            this.lstResults.FormattingEnabled = true;
            this.lstResults.ItemHeight = 16;
            this.lstResults.Location = new System.Drawing.Point(169, 77);
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new System.Drawing.Size(120, 20);
            this.lstResults.TabIndex = 7;
            // 
            // lstSPDS
            // 
            this.lstSPDS.FormattingEnabled = true;
            this.lstSPDS.ItemHeight = 16;
            this.lstSPDS.Location = new System.Drawing.Point(509, 103);
            this.lstSPDS.Name = "lstSPDS";
            this.lstSPDS.Size = new System.Drawing.Size(120, 20);
            this.lstSPDS.TabIndex = 15;
            // 
            // lstDAClass
            // 
            this.lstDAClass.FormattingEnabled = true;
            this.lstDAClass.ItemHeight = 16;
            this.lstDAClass.Location = new System.Drawing.Point(169, 51);
            this.lstDAClass.Name = "lstDAClass";
            this.lstDAClass.Size = new System.Drawing.Size(120, 20);
            this.lstDAClass.TabIndex = 5;
            // 
            // btnGetData
            // 
            this.btnGetData.Location = new System.Drawing.Point(293, 153);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(123, 40);
            this.btnGetData.TabIndex = 16;
            this.btnGetData.Text = "Get Data";
            this.btnGetData.UseVisualStyleBackColor = true;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(121, 17);
            this.label8.TabIndex = 2;
            this.label8.Text = "Number Returned";
            // 
            // udNumReturned
            // 
            this.udNumReturned.Location = new System.Drawing.Point(169, 23);
            this.udNumReturned.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.udNumReturned.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udNumReturned.Name = "udNumReturned";
            this.udNumReturned.Size = new System.Drawing.Size(120, 22);
            this.udNumReturned.TabIndex = 3;
            this.udNumReturned.Value = new decimal(new int[] {
            800,
            0,
            0,
            0});
            // 
            // txtLog
            // 
            this.txtLog.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.ForeColor = System.Drawing.Color.MediumBlue;
            this.txtLog.Location = new System.Drawing.Point(12, 205);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(684, 293);
            this.txtLog.TabIndex = 17;
            this.txtLog.Text = "Activity Log";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 510);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.udNumReturned);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnGetData);
            this.Controls.Add(this.lstDAClass);
            this.Controls.Add(this.lstSPDS);
            this.Controls.Add(this.lstResults);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lstTCB);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtIssuer);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lstState);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Name = "MainForm";
            this.Text = "Get Finra Data";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.udNumReturned)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lstState;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtIssuer;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox lstTCB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox lstResults;
        private System.Windows.Forms.ListBox lstSPDS;
        private System.Windows.Forms.ListBox lstDAClass;
        private System.Windows.Forms.Button btnGetData;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown udNumReturned;
        private System.Windows.Forms.TextBox txtLog;
    }
}