namespace AlgoritmPrizm
{
    partial class FConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FConfig));
            this.chkTrace = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtBoxHost = new System.Windows.Forms.TextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtBoxPort = new System.Windows.Forms.TextBox();
            this.lblFfd = new System.Windows.Forms.Label();
            this.cmbBoxFfd = new System.Windows.Forms.ComboBox();
            this.lblFrPort = new System.Windows.Forms.Label();
            this.txtBoxFrPort = new System.Windows.Forms.TextBox();
            this.lblTenderTypeCash = new System.Windows.Forms.Label();
            this.txtBoxTenderTypeCash = new System.Windows.Forms.TextBox();
            this.txtBoxTenderTypeCredit = new System.Windows.Forms.TextBox();
            this.lblTenderTypeCredit = new System.Windows.Forms.Label();
            this.txtBoxTenderTypeGiftCert = new System.Windows.Forms.TextBox();
            this.lblTenderTypeGiftCert = new System.Windows.Forms.Label();
            this.txtBoxTenderTypeGiftCard = new System.Windows.Forms.TextBox();
            this.lblTenderTypeGiftCard = new System.Windows.Forms.Label();
            this.txtBoxGiftCardCode = new System.Windows.Forms.TextBox();
            this.lblGiftCardCode = new System.Windows.Forms.Label();
            this.chkBoxGiftCardEnable = new System.Windows.Forms.CheckBox();
            this.txtBoxGiftCardTax = new System.Windows.Forms.TextBox();
            this.lblGiftCardTax = new System.Windows.Forms.Label();
            this.cmbBoxFieldItem = new System.Windows.Forms.ComboBox();
            this.lblFieldItem = new System.Windows.Forms.Label();
            this.txtBoxHostPrizmApi = new System.Windows.Forms.TextBox();
            this.lblHostPrizmApi = new System.Windows.Forms.Label();
            this.txtBoxPrizmApiSystemLogon = new System.Windows.Forms.TextBox();
            this.lblPrizmApiSystemLogon = new System.Windows.Forms.Label();
            this.txtBoxPrizmApiSystemPassord = new System.Windows.Forms.TextBox();
            this.lblPrizmApiSystemPassord = new System.Windows.Forms.Label();
            this.txtBoxPrizmApiTimeLiveTockenMinute = new System.Windows.Forms.TextBox();
            this.lblPrizmApiTimeLiveTockenMinute = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkTrace
            // 
            this.chkTrace.AutoSize = true;
            this.chkTrace.Location = new System.Drawing.Point(12, 12);
            this.chkTrace.Name = "chkTrace";
            this.chkTrace.Size = new System.Drawing.Size(87, 17);
            this.chkTrace.TabIndex = 1;
            this.chkTrace.Text = "Трасировка";
            this.chkTrace.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(407, 445);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtBoxHost
            // 
            this.txtBoxHost.Location = new System.Drawing.Point(49, 35);
            this.txtBoxHost.Name = "txtBoxHost";
            this.txtBoxHost.Size = new System.Drawing.Size(177, 20);
            this.txtBoxHost.TabIndex = 3;
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(12, 38);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(31, 13);
            this.lblHost.TabIndex = 4;
            this.lblHost.Text = "Хост";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(249, 38);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(32, 13);
            this.lblPort.TabIndex = 5;
            this.lblPort.Text = "Порт";
            // 
            // txtBoxPort
            // 
            this.txtBoxPort.Location = new System.Drawing.Point(286, 35);
            this.txtBoxPort.Name = "txtBoxPort";
            this.txtBoxPort.Size = new System.Drawing.Size(100, 20);
            this.txtBoxPort.TabIndex = 6;
            // 
            // lblFfd
            // 
            this.lblFfd.AutoSize = true;
            this.lblFfd.Location = new System.Drawing.Point(12, 64);
            this.lblFfd.Name = "lblFfd";
            this.lblFfd.Size = new System.Drawing.Size(77, 13);
            this.lblFfd.TabIndex = 7;
            this.lblFfd.Text = "ФФД версия";
            // 
            // cmbBoxFfd
            // 
            this.cmbBoxFfd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoxFfd.FormattingEnabled = true;
            this.cmbBoxFfd.Location = new System.Drawing.Point(95, 61);
            this.cmbBoxFfd.Name = "cmbBoxFfd";
            this.cmbBoxFfd.Size = new System.Drawing.Size(121, 21);
            this.cmbBoxFfd.TabIndex = 8;
            // 
            // lblFrPort
            // 
            this.lblFrPort.AutoSize = true;
            this.lblFrPort.Location = new System.Drawing.Point(249, 64);
            this.lblFrPort.Name = "lblFrPort";
            this.lblFrPort.Size = new System.Drawing.Size(103, 13);
            this.lblFrPort.TabIndex = 9;
            this.lblFrPort.Text = "Порт фискальника";
            // 
            // txtBoxFrPort
            // 
            this.txtBoxFrPort.Location = new System.Drawing.Point(358, 61);
            this.txtBoxFrPort.Name = "txtBoxFrPort";
            this.txtBoxFrPort.Size = new System.Drawing.Size(49, 20);
            this.txtBoxFrPort.TabIndex = 10;
            // 
            // lblTenderTypeCash
            // 
            this.lblTenderTypeCash.AutoSize = true;
            this.lblTenderTypeCash.Location = new System.Drawing.Point(12, 91);
            this.lblTenderTypeCash.Name = "lblTenderTypeCash";
            this.lblTenderTypeCash.Size = new System.Drawing.Size(189, 13);
            this.lblTenderTypeCash.TabIndex = 11;
            this.lblTenderTypeCash.Text = "Идентификатор типа оплаты за нал";
            // 
            // txtBoxTenderTypeCash
            // 
            this.txtBoxTenderTypeCash.Location = new System.Drawing.Point(316, 88);
            this.txtBoxTenderTypeCash.Name = "txtBoxTenderTypeCash";
            this.txtBoxTenderTypeCash.Size = new System.Drawing.Size(91, 20);
            this.txtBoxTenderTypeCash.TabIndex = 12;
            // 
            // txtBoxTenderTypeCredit
            // 
            this.txtBoxTenderTypeCredit.Location = new System.Drawing.Point(316, 114);
            this.txtBoxTenderTypeCredit.Name = "txtBoxTenderTypeCredit";
            this.txtBoxTenderTypeCredit.Size = new System.Drawing.Size(91, 20);
            this.txtBoxTenderTypeCredit.TabIndex = 14;
            // 
            // lblTenderTypeCredit
            // 
            this.lblTenderTypeCredit.AutoSize = true;
            this.lblTenderTypeCredit.Location = new System.Drawing.Point(12, 117);
            this.lblTenderTypeCredit.Name = "lblTenderTypeCredit";
            this.lblTenderTypeCredit.Size = new System.Drawing.Size(199, 13);
            this.lblTenderTypeCredit.TabIndex = 13;
            this.lblTenderTypeCredit.Text = "Идентификатор типа оплаты за карту";
            // 
            // txtBoxTenderTypeGiftCert
            // 
            this.txtBoxTenderTypeGiftCert.Location = new System.Drawing.Point(316, 140);
            this.txtBoxTenderTypeGiftCert.Name = "txtBoxTenderTypeGiftCert";
            this.txtBoxTenderTypeGiftCert.Size = new System.Drawing.Size(91, 20);
            this.txtBoxTenderTypeGiftCert.TabIndex = 16;
            // 
            // lblTenderTypeGiftCert
            // 
            this.lblTenderTypeGiftCert.AutoSize = true;
            this.lblTenderTypeGiftCert.Location = new System.Drawing.Point(12, 143);
            this.lblTenderTypeGiftCert.Name = "lblTenderTypeGiftCert";
            this.lblTenderTypeGiftCert.Size = new System.Drawing.Size(280, 13);
            this.lblTenderTypeGiftCert.TabIndex = 15;
            this.lblTenderTypeGiftCert.Text = "Идентификатор типа оплаты подарочный сертификат";
            // 
            // txtBoxTenderTypeGiftCard
            // 
            this.txtBoxTenderTypeGiftCard.Location = new System.Drawing.Point(316, 166);
            this.txtBoxTenderTypeGiftCard.Name = "txtBoxTenderTypeGiftCard";
            this.txtBoxTenderTypeGiftCard.Size = new System.Drawing.Size(91, 20);
            this.txtBoxTenderTypeGiftCard.TabIndex = 18;
            // 
            // lblTenderTypeGiftCard
            // 
            this.lblTenderTypeGiftCard.AutoSize = true;
            this.lblTenderTypeGiftCard.Location = new System.Drawing.Point(12, 169);
            this.lblTenderTypeGiftCard.Name = "lblTenderTypeGiftCard";
            this.lblTenderTypeGiftCard.Size = new System.Drawing.Size(247, 13);
            this.lblTenderTypeGiftCard.TabIndex = 17;
            this.lblTenderTypeGiftCard.Text = "Идентификатор типа оплаты подарочная карта";
            // 
            // txtBoxGiftCardCode
            // 
            this.txtBoxGiftCardCode.Location = new System.Drawing.Point(316, 192);
            this.txtBoxGiftCardCode.Name = "txtBoxGiftCardCode";
            this.txtBoxGiftCardCode.Size = new System.Drawing.Size(91, 20);
            this.txtBoxGiftCardCode.TabIndex = 20;
            // 
            // lblGiftCardCode
            // 
            this.lblGiftCardCode.AutoSize = true;
            this.lblGiftCardCode.Location = new System.Drawing.Point(12, 195);
            this.lblGiftCardCode.Name = "lblGiftCardCode";
            this.lblGiftCardCode.Size = new System.Drawing.Size(122, 13);
            this.lblGiftCardCode.TabIndex = 19;
            this.lblGiftCardCode.Text = "Код подарочной карты";
            // 
            // chkBoxGiftCardEnable
            // 
            this.chkBoxGiftCardEnable.AutoSize = true;
            this.chkBoxGiftCardEnable.Location = new System.Drawing.Point(15, 213);
            this.chkBoxGiftCardEnable.Name = "chkBoxGiftCardEnable";
            this.chkBoxGiftCardEnable.Size = new System.Drawing.Size(245, 17);
            this.chkBoxGiftCardEnable.TabIndex = 21;
            this.chkBoxGiftCardEnable.Text = "Включить использование подарочных карт";
            this.chkBoxGiftCardEnable.UseVisualStyleBackColor = true;
            // 
            // txtBoxGiftCardTax
            // 
            this.txtBoxGiftCardTax.Location = new System.Drawing.Point(316, 230);
            this.txtBoxGiftCardTax.Name = "txtBoxGiftCardTax";
            this.txtBoxGiftCardTax.Size = new System.Drawing.Size(91, 20);
            this.txtBoxGiftCardTax.TabIndex = 23;
            // 
            // lblGiftCardTax
            // 
            this.lblGiftCardTax.AutoSize = true;
            this.lblGiftCardTax.Location = new System.Drawing.Point(12, 233);
            this.lblGiftCardTax.Name = "lblGiftCardTax";
            this.lblGiftCardTax.Size = new System.Drawing.Size(277, 13);
            this.lblGiftCardTax.TabIndex = 22;
            this.lblGiftCardTax.Text = "Группа налогов при использовании подарочных карт";
            // 
            // cmbBoxFieldItem
            // 
            this.cmbBoxFieldItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoxFieldItem.FormattingEnabled = true;
            this.cmbBoxFieldItem.Location = new System.Drawing.Point(141, 252);
            this.cmbBoxFieldItem.Name = "cmbBoxFieldItem";
            this.cmbBoxFieldItem.Size = new System.Drawing.Size(121, 21);
            this.cmbBoxFieldItem.TabIndex = 25;
            // 
            // lblFieldItem
            // 
            this.lblFieldItem.AutoSize = true;
            this.lblFieldItem.Location = new System.Drawing.Point(12, 255);
            this.lblFieldItem.Name = "lblFieldItem";
            this.lblFieldItem.Size = new System.Drawing.Size(123, 13);
            this.lblFieldItem.TabIndex = 24;
            this.lblFieldItem.Text = "Поле с именем товара";
            // 
            // txtBoxHostPrizmApi
            // 
            this.txtBoxHostPrizmApi.Location = new System.Drawing.Point(316, 273);
            this.txtBoxHostPrizmApi.Name = "txtBoxHostPrizmApi";
            this.txtBoxHostPrizmApi.Size = new System.Drawing.Size(91, 20);
            this.txtBoxHostPrizmApi.TabIndex = 27;
            // 
            // lblHostPrizmApi
            // 
            this.lblHostPrizmApi.AutoSize = true;
            this.lblHostPrizmApi.Location = new System.Drawing.Point(12, 276);
            this.lblHostPrizmApi.Name = "lblHostPrizmApi";
            this.lblHostPrizmApi.Size = new System.Drawing.Size(277, 13);
            this.lblHostPrizmApi.TabIndex = 26;
            this.lblHostPrizmApi.Text = "Хост с приложением Prizm на котором развёрнут API";
            // 
            // txtBoxPrizmApiSystemLogon
            // 
            this.txtBoxPrizmApiSystemLogon.Location = new System.Drawing.Point(316, 299);
            this.txtBoxPrizmApiSystemLogon.Name = "txtBoxPrizmApiSystemLogon";
            this.txtBoxPrizmApiSystemLogon.Size = new System.Drawing.Size(91, 20);
            this.txtBoxPrizmApiSystemLogon.TabIndex = 29;
            // 
            // lblPrizmApiSystemLogon
            // 
            this.lblPrizmApiSystemLogon.AutoSize = true;
            this.lblPrizmApiSystemLogon.Location = new System.Drawing.Point(12, 302);
            this.lblPrizmApiSystemLogon.Name = "lblPrizmApiSystemLogon";
            this.lblPrizmApiSystemLogon.Size = new System.Drawing.Size(278, 13);
            this.lblPrizmApiSystemLogon.TabIndex = 28;
            this.lblPrizmApiSystemLogon.Text = "Login системного пользовательеля для доступа к api";
            // 
            // txtBoxPrizmApiSystemPassord
            // 
            this.txtBoxPrizmApiSystemPassord.Location = new System.Drawing.Point(316, 325);
            this.txtBoxPrizmApiSystemPassord.Name = "txtBoxPrizmApiSystemPassord";
            this.txtBoxPrizmApiSystemPassord.Size = new System.Drawing.Size(91, 20);
            this.txtBoxPrizmApiSystemPassord.TabIndex = 31;
            // 
            // lblPrizmApiSystemPassord
            // 
            this.lblPrizmApiSystemPassord.AutoSize = true;
            this.lblPrizmApiSystemPassord.Location = new System.Drawing.Point(12, 328);
            this.lblPrizmApiSystemPassord.Name = "lblPrizmApiSystemPassord";
            this.lblPrizmApiSystemPassord.Size = new System.Drawing.Size(298, 13);
            this.lblPrizmApiSystemPassord.TabIndex = 30;
            this.lblPrizmApiSystemPassord.Text = "Password системного пользовательеля для доступа к api";
            // 
            // txtBoxPrizmApiTimeLiveTockenMinute
            // 
            this.txtBoxPrizmApiTimeLiveTockenMinute.Location = new System.Drawing.Point(316, 351);
            this.txtBoxPrizmApiTimeLiveTockenMinute.Name = "txtBoxPrizmApiTimeLiveTockenMinute";
            this.txtBoxPrizmApiTimeLiveTockenMinute.Size = new System.Drawing.Size(91, 20);
            this.txtBoxPrizmApiTimeLiveTockenMinute.TabIndex = 33;
            // 
            // lblPrizmApiTimeLiveTockenMinute
            // 
            this.lblPrizmApiTimeLiveTockenMinute.AutoSize = true;
            this.lblPrizmApiTimeLiveTockenMinute.Location = new System.Drawing.Point(12, 354);
            this.lblPrizmApiTimeLiveTockenMinute.Name = "lblPrizmApiTimeLiveTockenMinute";
            this.lblPrizmApiTimeLiveTockenMinute.Size = new System.Drawing.Size(308, 13);
            this.lblPrizmApiTimeLiveTockenMinute.TabIndex = 32;
            this.lblPrizmApiTimeLiveTockenMinute.Text = "Тайм аут жизни токена после которого он обновится (мин)";
            // 
            // FConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 480);
            this.Controls.Add(this.txtBoxPrizmApiTimeLiveTockenMinute);
            this.Controls.Add(this.lblPrizmApiTimeLiveTockenMinute);
            this.Controls.Add(this.txtBoxPrizmApiSystemPassord);
            this.Controls.Add(this.lblPrizmApiSystemPassord);
            this.Controls.Add(this.txtBoxPrizmApiSystemLogon);
            this.Controls.Add(this.lblPrizmApiSystemLogon);
            this.Controls.Add(this.txtBoxHostPrizmApi);
            this.Controls.Add(this.lblHostPrizmApi);
            this.Controls.Add(this.cmbBoxFieldItem);
            this.Controls.Add(this.lblFieldItem);
            this.Controls.Add(this.txtBoxGiftCardTax);
            this.Controls.Add(this.lblGiftCardTax);
            this.Controls.Add(this.chkBoxGiftCardEnable);
            this.Controls.Add(this.txtBoxGiftCardCode);
            this.Controls.Add(this.lblGiftCardCode);
            this.Controls.Add(this.txtBoxTenderTypeGiftCard);
            this.Controls.Add(this.lblTenderTypeGiftCard);
            this.Controls.Add(this.txtBoxTenderTypeGiftCert);
            this.Controls.Add(this.lblTenderTypeGiftCert);
            this.Controls.Add(this.txtBoxTenderTypeCredit);
            this.Controls.Add(this.lblTenderTypeCredit);
            this.Controls.Add(this.txtBoxTenderTypeCash);
            this.Controls.Add(this.lblTenderTypeCash);
            this.Controls.Add(this.txtBoxFrPort);
            this.Controls.Add(this.lblFrPort);
            this.Controls.Add(this.cmbBoxFfd);
            this.Controls.Add(this.lblFfd);
            this.Controls.Add(this.txtBoxPort);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.lblHost);
            this.Controls.Add(this.txtBoxHost);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkTrace);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FConfig";
            this.Text = "FConfig";
            this.Load += new System.EventHandler(this.FConfig_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkTrace;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtBoxHost;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtBoxPort;
        private System.Windows.Forms.Label lblFfd;
        private System.Windows.Forms.ComboBox cmbBoxFfd;
        private System.Windows.Forms.Label lblFrPort;
        private System.Windows.Forms.TextBox txtBoxFrPort;
        private System.Windows.Forms.Label lblTenderTypeCash;
        private System.Windows.Forms.TextBox txtBoxTenderTypeCash;
        private System.Windows.Forms.TextBox txtBoxTenderTypeCredit;
        private System.Windows.Forms.Label lblTenderTypeCredit;
        private System.Windows.Forms.TextBox txtBoxTenderTypeGiftCert;
        private System.Windows.Forms.Label lblTenderTypeGiftCert;
        private System.Windows.Forms.TextBox txtBoxTenderTypeGiftCard;
        private System.Windows.Forms.Label lblTenderTypeGiftCard;
        private System.Windows.Forms.TextBox txtBoxGiftCardCode;
        private System.Windows.Forms.Label lblGiftCardCode;
        private System.Windows.Forms.CheckBox chkBoxGiftCardEnable;
        private System.Windows.Forms.TextBox txtBoxGiftCardTax;
        private System.Windows.Forms.Label lblGiftCardTax;
        private System.Windows.Forms.ComboBox cmbBoxFieldItem;
        private System.Windows.Forms.Label lblFieldItem;
        private System.Windows.Forms.TextBox txtBoxHostPrizmApi;
        private System.Windows.Forms.Label lblHostPrizmApi;
        private System.Windows.Forms.TextBox txtBoxPrizmApiSystemLogon;
        private System.Windows.Forms.Label lblPrizmApiSystemLogon;
        private System.Windows.Forms.TextBox txtBoxPrizmApiSystemPassord;
        private System.Windows.Forms.Label lblPrizmApiSystemPassord;
        private System.Windows.Forms.TextBox txtBoxPrizmApiTimeLiveTockenMinute;
        private System.Windows.Forms.Label lblPrizmApiTimeLiveTockenMinute;
    }
}