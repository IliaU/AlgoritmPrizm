namespace AlgoritmPrizm
{
    partial class FEmployee
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FEmployee));
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlButtom = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.pnlFill = new System.Windows.Forms.Panel();
            this.dGView = new System.Windows.Forms.DataGridView();
            this.PrizmLogin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlButtom.SuspendLayout();
            this.pnlFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGView)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(825, 50);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Visible = false;
            // 
            // pnlButtom
            // 
            this.pnlButtom.Controls.Add(this.btnSave);
            this.pnlButtom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtom.Location = new System.Drawing.Point(0, 388);
            this.pnlButtom.Name = "pnlButtom";
            this.pnlButtom.Size = new System.Drawing.Size(825, 33);
            this.pnlButtom.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(747, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pnlFill
            // 
            this.pnlFill.Controls.Add(this.dGView);
            this.pnlFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFill.Location = new System.Drawing.Point(0, 50);
            this.pnlFill.Name = "pnlFill";
            this.pnlFill.Size = new System.Drawing.Size(825, 338);
            this.pnlFill.TabIndex = 2;
            // 
            // dGView
            // 
            this.dGView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PrizmLogin,
            this.Fio});
            this.dGView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGView.Location = new System.Drawing.Point(0, 0);
            this.dGView.Name = "dGView";
            this.dGView.Size = new System.Drawing.Size(825, 338);
            this.dGView.TabIndex = 0;
            // 
            // PrizmLogin
            // 
            this.PrizmLogin.DataPropertyName = "PrizmLogin";
            this.PrizmLogin.HeaderText = "Логин сотрудника";
            this.PrizmLogin.Name = "PrizmLogin";
            this.PrizmLogin.Width = 200;
            // 
            // Fio
            // 
            this.Fio.DataPropertyName = "Fio";
            this.Fio.HeaderText = "ФИО";
            this.Fio.Name = "Fio";
            this.Fio.Width = 400;
            // 
            // FEmployee
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 421);
            this.Controls.Add(this.pnlFill);
            this.Controls.Add(this.pnlButtom);
            this.Controls.Add(this.pnlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FEmployee";
            this.Text = "FEmployee";
            this.Load += new System.EventHandler(this.FEmployee_Load);
            this.pnlButtom.ResumeLayout(false);
            this.pnlFill.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dGView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlButtom;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel pnlFill;
        private System.Windows.Forms.DataGridView dGView;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrizmLogin;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fio;
    }
}