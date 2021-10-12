namespace AlgoritmPrizm
{
    partial class FConnetSetup
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
            this.lblPrvTyp = new System.Windows.Forms.Label();
            this.cmbBoxPrvTyp = new System.Windows.Forms.ComboBox();
            this.btnConfig = new System.Windows.Forms.Button();
            this.txtBoxConnectionString = new System.Windows.Forms.TextBox();
            this.lblConnectionString = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPrvTyp
            // 
            this.lblPrvTyp.AutoSize = true;
            this.lblPrvTyp.Location = new System.Drawing.Point(12, 9);
            this.lblPrvTyp.Name = "lblPrvTyp";
            this.lblPrvTyp.Size = new System.Drawing.Size(92, 13);
            this.lblPrvTyp.TabIndex = 0;
            this.lblPrvTyp.Text = "Тип провайдера:";
            // 
            // cmbBoxPrvTyp
            // 
            this.cmbBoxPrvTyp.FormattingEnabled = true;
            this.cmbBoxPrvTyp.Location = new System.Drawing.Point(129, 6);
            this.cmbBoxPrvTyp.Name = "cmbBoxPrvTyp";
            this.cmbBoxPrvTyp.Size = new System.Drawing.Size(261, 21);
            this.cmbBoxPrvTyp.TabIndex = 1;
            // 
            // btnConfig
            // 
            this.btnConfig.Location = new System.Drawing.Point(315, 89);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(75, 23);
            this.btnConfig.TabIndex = 2;
            this.btnConfig.Text = "Изменить";
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // txtBoxConnectionString
            // 
            this.txtBoxConnectionString.Location = new System.Drawing.Point(129, 33);
            this.txtBoxConnectionString.Multiline = true;
            this.txtBoxConnectionString.Name = "txtBoxConnectionString";
            this.txtBoxConnectionString.ReadOnly = true;
            this.txtBoxConnectionString.Size = new System.Drawing.Size(261, 50);
            this.txtBoxConnectionString.TabIndex = 3;
            // 
            // lblConnectionString
            // 
            this.lblConnectionString.AutoSize = true;
            this.lblConnectionString.Location = new System.Drawing.Point(12, 36);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(116, 13);
            this.lblConnectionString.TabIndex = 4;
            this.lblConnectionString.Text = "Строка подключения:";
            // 
            // FConnetSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 124);
            this.Controls.Add(this.lblConnectionString);
            this.Controls.Add(this.txtBoxConnectionString);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.cmbBoxPrvTyp);
            this.Controls.Add(this.lblPrvTyp);
            this.Name = "FConnetSetup";
            this.Text = "Настройка подключения к базе";
            this.Load += new System.EventHandler(this.FConnetSetup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPrvTyp;
        private System.Windows.Forms.ComboBox cmbBoxPrvTyp;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.TextBox txtBoxConnectionString;
        private System.Windows.Forms.Label lblConnectionString;
    }
}