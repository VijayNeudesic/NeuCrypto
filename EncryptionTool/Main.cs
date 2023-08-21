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

        public void ResetAllControls()
        {
            lblCfgFileName.Text = "";
            dgToDoList.Rows.Clear();
            txtSQLServer.Text = "";
            txtSQLServer.Visible = false;
            rdDBAccess.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dlgrslt = CfgFileDialog.ShowDialog();

            if (dlgrslt != DialogResult.OK)
                return;

            lblCfgFileName.Text = CfgFileDialog.FileName;   

            dgToDoList.Rows.Clear();

            if(lblCfgFileName.Text != "")
            {
                string szDBNameOrPath = "";
                string szDBServer = "";
                txtSQLServer.Visible = false;
                txtSQLServer.Text = "";
                rdDBAccess.Checked = true;  

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
                        if (szDBServer != "")
                        {
                            txtSQLServer.Text = szDBServer;
                            txtSQLServer.Visible = true;
                            rdDBSQL.Checked = true;
                        }
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

                        dgToDoList.Rows.Add(szDBNameOrPath, szTableName, szFields, szWhereClauseFields, szFilters, "Not Started");
                    }
                }
            }
        }

        private async Task BulkEncrypt()
        {
            Encryptor encryptor = new Encryptor();

            if(encryptor.Init(@".\") < 0)
            {
                MessageBox.Show("Error initializing encryption process.  Error: " + encryptor.LastError);
                return;
            }

            foreach (DataGridViewRow row in dgToDoList.Rows)
            {
                if(row.Index == dgToDoList.Rows.Count - 1)
                    continue;

                if (row.Cells["Status"].Value.ToString() == "Not Started")
                {
                    row.Cells["Status"].Value = "In Progress";
                    row.Cells["Status"].Style.BackColor = Color.Yellow;

                    string filters = row.Cells["Filters"].Value == null ? "" : row.Cells["Filters"].Value.ToString();

                    int rc = encryptor.BulkEncryptDBTable(txtSQLServer.Text,
                                                     row.Cells["DBNameOrPath"].Value.ToString(),
                                                     row.Cells["TableName"].Value.ToString(),
                                                     row.Cells["Fields"].Value.ToString(),
                                                     row.Cells["WhereCls"].Value.ToString(),
                                                     filters);

                    if (rc == 0)
                    {
                        row.Cells["Status"].Value = encryptor.StatusMsg;
                        row.Cells["Status"].Style.BackColor = Color.Navy;
                        row.Cells["Status"].Style.ForeColor = Color.White;
                    }
                    else
                    {
                        row.Cells["Status"].Value = encryptor.LastError;
                        row.Cells["Status"].Style.BackColor = Color.Red;
                        row.Cells["Status"].Style.ForeColor = Color.White;
                    }
                }
            }
        }

        private async Task BulkDecrypt(string szAccessCode)
        {
            if(szAccessCode == "")
            {
                MessageBox.Show("Please enter an access code.");
                return;
            }

            Encryptor encryptor = new Encryptor();
            if (encryptor.Init(@".\") < 0)
            {
                MessageBox.Show("Error initializing encryption process.  Error: " + encryptor.LastError);
                return;
            }

            foreach (DataGridViewRow row in dgToDoList.Rows)
            {
                if(row.Index == dgToDoList.Rows.Count - 1)
                    continue;   

                if (row.Cells["Status"].Value.ToString() == "Not Started")
                {
                    row.Cells["Status"].Value = "In Progress";
                    row.Cells["Status"].Style.BackColor = Color.Yellow;

                    string filters = row.Cells["Filters"].Value == null ? "" : row.Cells["Filters"].Value.ToString();

                    int rc = encryptor.BulkDecryptDBTable(txtSQLServer.Text,
                                                     row.Cells["DBNameOrPath"].Value.ToString(),
                                                     row.Cells["TableName"].Value.ToString(),
                                                     row.Cells["Fields"].Value.ToString(),
                                                     row.Cells["WhereCls"].Value.ToString(),
                                                     filters, szAccessCode);

                    if (rc == 0)
                    {
                        row.Cells["Status"].Value = encryptor.StatusMsg;
                        row.Cells["Status"].Style.BackColor = Color.Navy;
                        row.Cells["Status"].Style.ForeColor = Color.White;
                    }
                    else if(rc == -2)
                    {
                        row.Cells["Status"].Value = encryptor.LastError;
                        row.Cells["Status"].Style.BackColor = Color.Red;
                        row.Cells["Status"].Style.ForeColor = Color.White;
                    }
                    else
                    {
                        row.Cells["Status"].Value = encryptor.LastError;
                        row.Cells["Status"].Style.BackColor = Color.Red;
                        row.Cells["Status"].Style.ForeColor = Color.White;
                    }
                }
            }
        }

        //ValidateData method needs to check for SQL Server, and Access Code
        //It should check the DataGrid for valid data in all the rows except for last row and then enable the Encrypt and Decrypt buttons
        private bool ValidateData()
        {
            if (rdDBSQL.Checked)
            {
                if (txtSQLServer.Text == "")
                {
                    MessageBox.Show("Please enter the SQL Server name.");
                    return false;
                }
            }

            foreach (DataGridViewRow row in dgToDoList.Rows)
            {
                if (row.Index == dgToDoList.Rows.Count - 1)
                    continue;

                if (row.Cells["DBNameOrPath"].Value.ToString() == "")
                {
                    MessageBox.Show("Please enter the Database name or path.");
                    return false;
                }

                if (row.Cells["TableName"].Value.ToString() == "")
                {
                    MessageBox.Show("Please enter the Table name.");
                    return false;
                }

                if (row.Cells["Fields"].Value.ToString() == "")
                {
                    MessageBox.Show("Please enter the Fields.");
                    return false;
                }

                if (row.Cells["WhereCls"].Value.ToString() == "")
                {
                    MessageBox.Show("Please enter the uniquely identifiable fields.");
                    return false;
                }
            }

            return true;
        }

        private async void btnBulkDecryption_Click(object sender, EventArgs e)
        {
            if (!ValidateData())
                return;

            if (txtAccCode.Text == "")
            {
                MessageBox.Show("Please enter the Access code.");
                return;
            }

            string szAccessCode = txtAccCode.Text;
            txtAccCode.Text = "";

            ResetStatus();

            SetControlsStatus(true);

            await Task.Run(async () => await BulkDecrypt(szAccessCode));

            SetControlsStatus(false);
        }
        private async void btnBulkEncrypt_Click(object sender, EventArgs e)
        {
            if(!ValidateData())
                return;

            ResetStatus();

            SetControlsStatus(true);

            await Task.Run(async () => await BulkEncrypt());

            SetControlsStatus(false);
        }

        private void SetControlsStatus(bool Inprogress)
        {
            btnBulkEncrypt.Enabled = !Inprogress;
            btnBulkDecryption.Enabled = !Inprogress;
            btnResetAll.Enabled = !Inprogress;
            button1.Enabled = !Inprogress;
            rdDBAccess.Enabled = !Inprogress;
            rdDBSQL.Enabled = !Inprogress;
            txtAccCode.Enabled = !Inprogress;
            txtSQLServer.Enabled = !Inprogress;
            dgToDoList.ReadOnly = Inprogress;
            dgToDoList.Columns["Status"].ReadOnly = true;
        }

        private void ResetStatus()
        {
            foreach (DataGridViewRow row in dgToDoList.Rows)
            {
                if(row.Index == dgToDoList.Rows.Count - 1)
                    continue;

                row.Cells["Status"].Value = "Not Started";
                row.Cells["Status"].Style.BackColor = Color.White;
                row.Cells["Status"].Style.ForeColor = Color.Black;
            }

        }

        private void rdDBSQL_CheckedChanged(object sender, EventArgs e)
        {
            if(rdDBSQL.Checked == true)
                txtSQLServer.Visible = true;
        }

        private void rdDBAccess_CheckedChanged(object sender, EventArgs e)
        {
            if (rdDBAccess.Checked == true)
            {
                txtSQLServer.Text = "";
                txtSQLServer.Visible = false;
            }
        }

        private void btnResetAll_Click(object sender, EventArgs e)
        {
            ResetAllControls();
        }
    }
}
