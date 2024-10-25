
namespace AlgoritmPrizm
{
    partial class FCryptoSetup
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblValidCert = new System.Windows.Forms.Label();
            this.btnChoiseSet = new System.Windows.Forms.Button();
            this.lblDefaultActivMinToken = new System.Windows.Forms.Label();
            this.txtBoxDefaultActivMinToken = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(174, 13);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Tag = "Текущий сертификат @CurSert";
            this.lblTitle.Text = "Текущий сертификат  не выбран";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(676, 182);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblValidCert
            // 
            this.lblValidCert.AutoSize = true;
            this.lblValidCert.Location = new System.Drawing.Point(12, 69);
            this.lblValidCert.Name = "lblValidCert";
            this.lblValidCert.Size = new System.Drawing.Size(164, 13);
            this.lblValidCert.TabIndex = 3;
            this.lblValidCert.Tag = "Сертификат валиден до @Now";
            this.lblValidCert.Text = "Информации о валидности нет";
            // 
            // btnChoiseSet
            // 
            this.btnChoiseSet.Location = new System.Drawing.Point(12, 25);
            this.btnChoiseSet.Name = "btnChoiseSet";
            this.btnChoiseSet.Size = new System.Drawing.Size(75, 23);
            this.btnChoiseSet.TabIndex = 4;
            this.btnChoiseSet.Text = "Выбрать сертификат";
            this.btnChoiseSet.UseVisualStyleBackColor = true;
            this.btnChoiseSet.Click += new System.EventHandler(this.btnChoiseSet_Click);
            // 
            // lblDefaultActivMinToken
            // 
            this.lblDefaultActivMinToken.AutoSize = true;
            this.lblDefaultActivMinToken.Location = new System.Drawing.Point(12, 114);
            this.lblDefaultActivMinToken.Name = "lblDefaultActivMinToken";
            this.lblDefaultActivMinToken.Size = new System.Drawing.Size(365, 13);
            this.lblDefaultActivMinToken.TabIndex = 5;
            this.lblDefaultActivMinToken.Text = "Через сколько минут считать токен протухшим и получить его заново";
            // 
            // txtBoxDefaultActivMinToken
            // 
            this.txtBoxDefaultActivMinToken.Location = new System.Drawing.Point(396, 111);
            this.txtBoxDefaultActivMinToken.Name = "txtBoxDefaultActivMinToken";
            this.txtBoxDefaultActivMinToken.Size = new System.Drawing.Size(55, 20);
            this.txtBoxDefaultActivMinToken.TabIndex = 6;
            // 
            // FCryptoSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 217);
            this.Controls.Add(this.txtBoxDefaultActivMinToken);
            this.Controls.Add(this.lblDefaultActivMinToken);
            this.Controls.Add(this.btnChoiseSet);
            this.Controls.Add(this.lblValidCert);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblTitle);
            this.Name = "FCryptoSetup";
            this.Text = "Настройка сертификата";
            this.Load += new System.EventHandler(this.FCryptoSetup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblValidCert;
        private System.Windows.Forms.Button btnChoiseSet;
        private System.Windows.Forms.Label lblDefaultActivMinToken;
        private System.Windows.Forms.TextBox txtBoxDefaultActivMinToken;
    }
}