namespace EncryptionTool
{
    partial class Main
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
            this.button1 = new System.Windows.Forms.Button();
            this.CfgFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lblCfgFileName = new System.Windows.Forms.Label();
            this.dgToDoList = new System.Windows.Forms.DataGridView();
            this.DBNameOrPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fields = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WhereCls = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Filters = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnBulkEncrypt = new System.Windows.Forms.Button();
            this.btnBulkDecryption = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAccCode = new System.Windows.Forms.TextBox();
            this.rdDBAccess = new System.Windows.Forms.RadioButton();
            this.rdDBSQL = new System.Windows.Forms.RadioButton();
            this.txtSQLServer = new System.Windows.Forms.TextBox();
            this.btnResetAll = new System.Windows.Forms.Button();
            this.chkUseDistinct = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgToDoList)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 14);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(183, 34);
            this.button1.TabIndex = 0;
            this.button1.Text = "Open Config File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CfgFileDialog
            // 
            this.CfgFileDialog.DefaultExt = "*.cfg";
            this.CfgFileDialog.FileName = "CfgFileDialog";
            this.CfgFileDialog.Filter = "Config|*.cfg";
            this.CfgFileDialog.InitialDirectory = ".";
            // 
            // lblCfgFileName
            // 
            this.lblCfgFileName.AutoSize = true;
            this.lblCfgFileName.Location = new System.Drawing.Point(205, 23);
            this.lblCfgFileName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCfgFileName.Name = "lblCfgFileName";
            this.lblCfgFileName.Size = new System.Drawing.Size(69, 16);
            this.lblCfgFileName.TabIndex = 1;
            this.lblCfgFileName.Text = "File Name";
            // 
            // dgToDoList
            // 
            this.dgToDoList.AllowDrop = true;
            this.dgToDoList.AllowUserToDeleteRows = false;
            this.dgToDoList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgToDoList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgToDoList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DBNameOrPath,
            this.TableName,
            this.Fields,
            this.WhereCls,
            this.Filters,
            this.Status});
            this.dgToDoList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.dgToDoList.Location = new System.Drawing.Point(15, 93);
            this.dgToDoList.Margin = new System.Windows.Forms.Padding(4);
            this.dgToDoList.MultiSelect = false;
            this.dgToDoList.Name = "dgToDoList";
            this.dgToDoList.RowHeadersVisible = false;
            this.dgToDoList.RowHeadersWidth = 51;
            this.dgToDoList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgToDoList.Size = new System.Drawing.Size(1165, 226);
            this.dgToDoList.TabIndex = 2;
            // 
            // DBNameOrPath
            // 
            this.DBNameOrPath.HeaderText = "Database";
            this.DBNameOrPath.MinimumWidth = 6;
            this.DBNameOrPath.Name = "DBNameOrPath";
            this.DBNameOrPath.Width = 150;
            // 
            // TableName
            // 
            this.TableName.HeaderText = "Table";
            this.TableName.MinimumWidth = 6;
            this.TableName.Name = "TableName";
            this.TableName.Width = 150;
            // 
            // Fields
            // 
            this.Fields.HeaderText = "Fields To Encrypt";
            this.Fields.MinimumWidth = 6;
            this.Fields.Name = "Fields";
            this.Fields.Width = 200;
            // 
            // WhereCls
            // 
            this.WhereCls.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.WhereCls.HeaderText = "Uniquely Identifiable Fields";
            this.WhereCls.MinimumWidth = 6;
            this.WhereCls.Name = "WhereCls";
            // 
            // Filters
            // 
            this.Filters.HeaderText = "Operators";
            this.Filters.MinimumWidth = 6;
            this.Filters.Name = "Filters";
            this.Filters.Width = 200;
            // 
            // Status
            // 
            this.Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Status.HeaderText = "Status";
            this.Status.MinimumWidth = 6;
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // btnBulkEncrypt
            // 
            this.btnBulkEncrypt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBulkEncrypt.Location = new System.Drawing.Point(266, 326);
            this.btnBulkEncrypt.Margin = new System.Windows.Forms.Padding(4);
            this.btnBulkEncrypt.Name = "btnBulkEncrypt";
            this.btnBulkEncrypt.Size = new System.Drawing.Size(200, 37);
            this.btnBulkEncrypt.TabIndex = 3;
            this.btnBulkEncrypt.Text = "Start Bulk Encryption";
            this.btnBulkEncrypt.UseVisualStyleBackColor = true;
            this.btnBulkEncrypt.Click += new System.EventHandler(this.btnBulkEncrypt_Click);
            // 
            // btnBulkDecryption
            // 
            this.btnBulkDecryption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBulkDecryption.Location = new System.Drawing.Point(474, 326);
            this.btnBulkDecryption.Margin = new System.Windows.Forms.Padding(4);
            this.btnBulkDecryption.Name = "btnBulkDecryption";
            this.btnBulkDecryption.Size = new System.Drawing.Size(200, 37);
            this.btnBulkDecryption.TabIndex = 4;
            this.btnBulkDecryption.Text = "Start Bulk Decryption";
            this.btnBulkDecryption.UseVisualStyleBackColor = true;
            this.btnBulkDecryption.Click += new System.EventHandler(this.btnBulkDecryption_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(978, 336);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Access Code:";
            // 
            // txtAccCode
            // 
            this.txtAccCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAccCode.Location = new System.Drawing.Point(1075, 333);
            this.txtAccCode.Name = "txtAccCode";
            this.txtAccCode.Size = new System.Drawing.Size(100, 22);
            this.txtAccCode.TabIndex = 6;
            // 
            // rdDBAccess
            // 
            this.rdDBAccess.AutoSize = true;
            this.rdDBAccess.Checked = true;
            this.rdDBAccess.Location = new System.Drawing.Point(15, 66);
            this.rdDBAccess.Name = "rdDBAccess";
            this.rdDBAccess.Size = new System.Drawing.Size(95, 20);
            this.rdDBAccess.TabIndex = 7;
            this.rdDBAccess.TabStop = true;
            this.rdDBAccess.Text = "Access DB";
            this.rdDBAccess.UseVisualStyleBackColor = true;
            this.rdDBAccess.CheckedChanged += new System.EventHandler(this.rdDBAccess_CheckedChanged);
            // 
            // rdDBSQL
            // 
            this.rdDBSQL.AutoSize = true;
            this.rdDBSQL.Location = new System.Drawing.Point(116, 66);
            this.rdDBSQL.Name = "rdDBSQL";
            this.rdDBSQL.Size = new System.Drawing.Size(97, 20);
            this.rdDBSQL.TabIndex = 8;
            this.rdDBSQL.TabStop = true;
            this.rdDBSQL.Text = "SQL Server";
            this.rdDBSQL.UseVisualStyleBackColor = true;
            this.rdDBSQL.CheckedChanged += new System.EventHandler(this.rdDBSQL_CheckedChanged);
            // 
            // txtSQLServer
            // 
            this.txtSQLServer.Location = new System.Drawing.Point(219, 65);
            this.txtSQLServer.Name = "txtSQLServer";
            this.txtSQLServer.Size = new System.Drawing.Size(305, 22);
            this.txtSQLServer.TabIndex = 9;
            this.txtSQLServer.Visible = false;
            // 
            // btnResetAll
            // 
            this.btnResetAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetAll.Location = new System.Drawing.Point(1061, 25);
            this.btnResetAll.Name = "btnResetAll";
            this.btnResetAll.Size = new System.Drawing.Size(114, 33);
            this.btnResetAll.TabIndex = 10;
            this.btnResetAll.Text = "Reset All";
            this.btnResetAll.UseVisualStyleBackColor = true;
            this.btnResetAll.Click += new System.EventHandler(this.btnResetAll_Click);
            // 
            // chkUseDistinct
            // 
            this.chkUseDistinct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkUseDistinct.AutoSize = true;
            this.chkUseDistinct.Location = new System.Drawing.Point(15, 336);
            this.chkUseDistinct.Name = "chkUseDistinct";
            this.chkUseDistinct.Size = new System.Drawing.Size(100, 20);
            this.chkUseDistinct.TabIndex = 11;
            this.chkUseDistinct.Text = "Use Distinct";
            this.chkUseDistinct.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1195, 375);
            this.Controls.Add(this.chkUseDistinct);
            this.Controls.Add(this.btnResetAll);
            this.Controls.Add(this.txtSQLServer);
            this.Controls.Add(this.rdDBSQL);
            this.Controls.Add(this.rdDBAccess);
            this.Controls.Add(this.txtAccCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBulkDecryption);
            this.Controls.Add(this.btnBulkEncrypt);
            this.Controls.Add(this.dgToDoList);
            this.Controls.Add(this.lblCfgFileName);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Main";
            this.Text = "Encryption Tool";
            ((System.ComponentModel.ISupportInitialize)(this.dgToDoList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog CfgFileDialog;
        private System.Windows.Forms.Label lblCfgFileName;
        private System.Windows.Forms.DataGridView dgToDoList;
        private System.Windows.Forms.Button btnBulkEncrypt;
        private System.Windows.Forms.Button btnBulkDecryption;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAccCode;
        private System.Windows.Forms.RadioButton rdDBAccess;
        private System.Windows.Forms.RadioButton rdDBSQL;
        private System.Windows.Forms.TextBox txtSQLServer;
        private System.Windows.Forms.Button btnResetAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn DBNameOrPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn TableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fields;
        private System.Windows.Forms.DataGridViewTextBoxColumn WhereCls;
        private System.Windows.Forms.DataGridViewTextBoxColumn Filters;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.CheckBox chkUseDistinct;
    }
}

