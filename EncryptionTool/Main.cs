using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NeuCrypto;

namespace EncryptionTool
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CfgFileDialog.ShowDialog();
            lblCfgFileName.Text = CfgFileDialog.FileName;   

            dgToDoList.Rows.Clear();

            if(lblCfgFileName.Text != "")
            {
                string szDBNameOrPath = "";
                string szDBServer = "";
                
                string[] szCfgTextArr = File.ReadAllLines(lblCfgFileName.Text);
                foreach(string cfgText in szCfgTextArr)
                {
                    if(cfgText.ToUpper().StartsWith("DBNAMEORPATH="))
                    {
                        szDBNameOrPath = cfgText.Substring("DBNameOrPath=".Length);
                    }
                    else if(cfgText.ToUpper().StartsWith("DBSERVER="))
                    {
                        szDBServer = cfgText.Substring("DBServer=".Length);
                    }
                    //Other lines are in format TableName|Field1,Field2|WhereClauseField1,WhereClauseField1|Filter1,Filter2
                    else if(cfgText != "")
                    {
                        string szTableName = "";
                        string szFields = "";
                        string szWhereClauseFields = "";
                        string szFilters = "";
                        string[] szTableTextArr = cfgText.Split('|');
                        if(szTableTextArr.Length == 4)
                        {
                            szTableName = szTableTextArr[0];
                            szFields = szTableTextArr[1];
                            szWhereClauseFields = szTableTextArr[2];
                            szFilters = szTableTextArr[3];
                        }
                        else
                        {
                            MessageBox.Show("Invalid configuration file format.  Please check the configuration file and try again.");
                            return;
                        }

                        /*CryptoProcess cryptoProcess = new CryptoProcess();
                        cryptoProcess.InitAll(@".\", false);
                        cryptoProcess.BulkEncryptDBTable(szDBServer, szDBNameOrPath, szTableName, szFields, szWhereClauseFields, szFilters);*/

                        dgToDoList.Rows.Add(szDBServer, szDBNameOrPath, szTableName, szFields, szWhereClauseFields, szFilters, "Not Started");
                    }
                }
            }
        }

        private async Task BulkEncrypt()
        {
            CryptoProcess cryptoProcess = new CryptoProcess();
            cryptoProcess.InitAll(@".\");

            foreach (DataGridViewRow row in dgToDoList.Rows)
            {
                if (row.Cells["Status"].Value.ToString() == "Not Started")
                {
                    row.Cells["Status"].Value = "In Progress";
                    row.Cells["Status"].Style.BackColor = Color.Yellow;

                    int rc = cryptoProcess.BulkEncryptDBTable(row.Cells["ServerName"].Value.ToString(),
                                                     row.Cells["DBNameOrPath"].Value.ToString(),
                                                     row.Cells["TableName"].Value.ToString(),
                                                     row.Cells["Fields"].Value.ToString(),
                                                     row.Cells["WhereCls"].Value.ToString(),
                                                     row.Cells["Filters"].Value.ToString());

                    if (rc == 0)
                    {
                        row.Cells["Status"].Value = cryptoProcess.StatusMsg;
                        row.Cells["Status"].Style.BackColor = Color.Navy;
                        row.Cells["Status"].Style.ForeColor = Color.White;
                    }
                    else
                    {
                        row.Cells["Status"].Value = cryptoProcess.LastError;
                        row.Cells["Status"].Style.BackColor = Color.Red;
                        row.Cells["Status"].Style.ForeColor = Color.White;
                    }
                }
            }
        }

        private async Task BulkDecrypt()
        {
            CryptoProcess cryptoProcess = new CryptoProcess();
            cryptoProcess.InitAll(@".\");

            foreach (DataGridViewRow row in dgToDoList.Rows)
            {
                if (row.Cells["Status"].Value.ToString() == "Not Started")
                {
                    row.Cells["Status"].Value = "In Progress";
                    row.Cells["Status"].Style.BackColor = Color.Yellow;

                    int rc = cryptoProcess.BulkDecryptDBTable(row.Cells["ServerName"].Value.ToString(),
                                                     row.Cells["DBNameOrPath"].Value.ToString(),
                                                     row.Cells["TableName"].Value.ToString(),
                                                     row.Cells["Fields"].Value.ToString(),
                                                     row.Cells["WhereCls"].Value.ToString(),
                                                     row.Cells["Filters"].Value.ToString());

                    if (rc == 0)
                    {
                        row.Cells["Status"].Value = cryptoProcess.StatusMsg;
                        row.Cells["Status"].Style.BackColor = Color.Navy;
                        row.Cells["Status"].Style.ForeColor = Color.White;
                    }
                    else
                    {
                        row.Cells["Status"].Value = cryptoProcess.LastError;
                        row.Cells["Status"].Style.BackColor = Color.Red;
                        row.Cells["Status"].Style.ForeColor = Color.White;
                    }
                }
            }
        }

        private void btnBulkDecryption_Click(object sender, EventArgs e)
        {
            if (txtAccCode.Text == "")
            {
                MessageBox.Show("Please enter the Access code.");
                return;
            }

            CryptoProcess cryptoProcess = new CryptoProcess();
            if(txtAccCode.Text != cryptoProcess.GenerateAccessCode())
            {
                MessageBox.Show("Invalid Access Code.");
                return;
            }

            ResetStatus();
            btnBulkDecryption.Enabled = false;
            btnBulkEncrypt.Enabled = false;

            Task.Run(async () => await BulkDecrypt());

            btnBulkDecryption.Enabled = true;
            btnBulkEncrypt.Enabled = true;
        }
        private async void btnBulkEncrypt_Click(object sender, EventArgs e)
        {
            ResetStatus();
            btnBulkEncrypt.Enabled = false;
            btnBulkDecryption.Enabled = false;

            await Task.Run(async () => await BulkEncrypt());

            btnBulkEncrypt.Enabled = true;
            btnBulkDecryption.Enabled = true;
        }

        private void ResetStatus()
        {
            /*foreach (DataGridViewRow row in dgToDoList.Rows)
            {
                row.Cells["Status"].Value = "Not Started";
                row.Cells["Status"].Style.BackColor = Color.White;
                row.Cells["Status"].Style.ForeColor = Color.Black;
            }*/

            //Is there a way to update a cell value in all the rows without a loop?  With the help of LINQ, yes.
            //Can I also set style in the same statement along with setting value in LINQ statement.
            dgToDoList.Rows.Cast<DataGridViewRow>().ToList().ForEach(r =>
                                                            {
                                                                r.Cells["Status"].Value = "Not Started";
                                                                r.Cells["Status"].Style.BackColor = Color.White;
                                                                r.Cells["Status"].Style.ForeColor = Color.Black;
                                                            });
        }
    }
}
