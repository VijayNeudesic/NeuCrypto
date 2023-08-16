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
            this.ServerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.lblCfgFileName.Location = new System.Drawing.Point(211, 18);
            this.lblCfgFileName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCfgFileName.Name = "lblCfgFileName";
            this.lblCfgFileName.Size = new System.Drawing.Size(69, 16);
            this.lblCfgFileName.TabIndex = 1;
            this.lblCfgFileName.Text = "File Name";
            // 
            // dgToDoList
            // 
            this.dgToDoList.AllowUserToAddRows = false;
            this.dgToDoList.AllowUserToDeleteRows = false;
            this.dgToDoList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgToDoList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgToDoList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ServerName,
            this.DBNameOrPath,
            this.TableName,
            this.Fields,
            this.WhereCls,
            this.Filters,
            this.Status});
            this.dgToDoList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dgToDoList.Location = new System.Drawing.Point(15, 70);
            this.dgToDoList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgToDoList.MultiSelect = false;
            this.dgToDoList.Name = "dgToDoList";
            this.dgToDoList.ReadOnly = true;
            this.dgToDoList.RowHeadersVisible = false;
            this.dgToDoList.RowHeadersWidth = 51;
            this.dgToDoList.Size = new System.Drawing.Size(1475, 601);
            this.dgToDoList.TabIndex = 2;
            // 
            // ServerName
            // 
            this.ServerName.HeaderText = "Database Server";
            this.ServerName.MinimumWidth = 6;
            this.ServerName.Name = "ServerName";
            this.ServerName.ReadOnly = true;
            this.ServerName.Visible = false;
            this.ServerName.Width = 125;
            // 
            // DBNameOrPath
            // 
            this.DBNameOrPath.HeaderText = "Database";
            this.DBNameOrPath.MinimumWidth = 6;
            this.DBNameOrPath.Name = "DBNameOrPath";
            this.DBNameOrPath.ReadOnly = true;
            this.DBNameOrPath.Width = 150;
            // 
            // TableName
            // 
            this.TableName.HeaderText = "Table";
            this.TableName.MinimumWidth = 6;
            this.TableName.Name = "TableName";
            this.TableName.ReadOnly = true;
            this.TableName.Width = 150;
            // 
            // Fields
            // 
            this.Fields.HeaderText = "Fields To Encrypt";
            this.Fields.MinimumWidth = 6;
            this.Fields.Name = "Fields";
            this.Fields.ReadOnly = true;
            this.Fields.Width = 200;
            // 
            // WhereCls
            // 
            this.WhereCls.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.WhereCls.HeaderText = "Where Clause Fields";
            this.WhereCls.MinimumWidth = 6;
            this.WhereCls.Name = "WhereCls";
            this.WhereCls.ReadOnly = true;
            // 
            // Filters
            // 
            this.Filters.HeaderText = "Filters";
            this.Filters.MinimumWidth = 6;
            this.Filters.Name = "Filters";
            this.Filters.ReadOnly = true;
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
            this.btnBulkEncrypt.Location = new System.Drawing.Point(576, 678);
            this.btnBulkEncrypt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.btnBulkDecryption.Location = new System.Drawing.Point(784, 678);
            this.btnBulkDecryption.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnBulkDecryption.Name = "btnBulkDecryption";
            this.btnBulkDecryption.Size = new System.Drawing.Size(200, 37);
            this.btnBulkDecryption.TabIndex = 4;
            this.btnBulkDecryption.Text = "Start Bulk Decryption";
            this.btnBulkDecryption.UseVisualStyleBackColor = true;
            this.btnBulkDecryption.Click += new System.EventHandler(this.btnBulkDecryption_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1185, 688);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Access Code:";
            // 
            // txtAccCode
            // 
            this.txtAccCode.Location = new System.Drawing.Point(1282, 685);
            this.txtAccCode.Name = "txtAccCode";
            this.txtAccCode.Size = new System.Drawing.Size(100, 22);
            this.txtAccCode.TabIndex = 6;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1505, 727);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn ServerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DBNameOrPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn TableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fields;
        private System.Windows.Forms.DataGridViewTextBoxColumn WhereCls;
        private System.Windows.Forms.DataGridViewTextBoxColumn Filters;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.Button btnBulkDecryption;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAccCode;
    }
}

