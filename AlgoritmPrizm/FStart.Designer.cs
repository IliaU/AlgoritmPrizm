namespace AlgoritmPrizm
{
    partial class FStart
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FStart));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tSSLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.Pnl_Top = new System.Windows.Forms.Panel();
            this.Pnl_TopFill = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.TSMItemGonfig = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMItemGonfigCashiries = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMItemGonfigEmployees = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMItemGonfigOther = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMItemLic = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMItemAboutPrv = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMItemConfigDB = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMItemConfigTerminalSD = new System.Windows.Forms.ToolStripMenuItem();
            this.Pnl_TopRight = new System.Windows.Forms.Panel();
            this.pctBoxSystemUnLock = new System.Windows.Forms.PictureBox();
            this.pctBoxSystemLock = new System.Windows.Forms.PictureBox();
            this.Pnl_Fill = new System.Windows.Forms.Panel();
            this.TSMItemConfigCrypto = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip.SuspendLayout();
            this.Pnl_Top.SuspendLayout();
            this.Pnl_TopFill.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.Pnl_TopRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctBoxSystemUnLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctBoxSystemLock)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSSLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 347);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(521, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tSSLabel
            // 
            this.tSSLabel.Name = "tSSLabel";
            this.tSSLabel.Size = new System.Drawing.Size(51, 17);
            this.tSSLabel.Text = "tSSLabel";
            // 
            // Pnl_Top
            // 
            this.Pnl_Top.Controls.Add(this.Pnl_TopFill);
            this.Pnl_Top.Controls.Add(this.Pnl_TopRight);
            this.Pnl_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.Pnl_Top.Location = new System.Drawing.Point(0, 0);
            this.Pnl_Top.Name = "Pnl_Top";
            this.Pnl_Top.Size = new System.Drawing.Size(521, 27);
            this.Pnl_Top.TabIndex = 2;
            // 
            // Pnl_TopFill
            // 
            this.Pnl_TopFill.Controls.Add(this.menuStrip1);
            this.Pnl_TopFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Pnl_TopFill.Location = new System.Drawing.Point(0, 0);
            this.Pnl_TopFill.Name = "Pnl_TopFill";
            this.Pnl_TopFill.Size = new System.Drawing.Size(462, 27);
            this.Pnl_TopFill.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMItemGonfig});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(462, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip";
            // 
            // TSMItemGonfig
            // 
            this.TSMItemGonfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMItemGonfigCashiries,
            this.TSMItemGonfigEmployees,
            this.TSMItemGonfigOther,
            this.TSMItemLic,
            this.TSMItemAboutPrv,
            this.TSMItemConfigDB,
            this.TSMItemConfigTerminalSD,
            this.TSMItemConfigCrypto});
            this.TSMItemGonfig.Name = "TSMItemGonfig";
            this.TSMItemGonfig.Size = new System.Drawing.Size(78, 20);
            this.TSMItemGonfig.Text = "Настройка";
            // 
            // TSMItemGonfigCashiries
            // 
            this.TSMItemGonfigCashiries.Name = "TSMItemGonfigCashiries";
            this.TSMItemGonfigCashiries.Size = new System.Drawing.Size(328, 22);
            this.TSMItemGonfigCashiries.Text = "Кассиры";
            this.TSMItemGonfigCashiries.Click += new System.EventHandler(this.TSMItemGonfigCashiries_Click);
            // 
            // TSMItemGonfigEmployees
            // 
            this.TSMItemGonfigEmployees.Name = "TSMItemGonfigEmployees";
            this.TSMItemGonfigEmployees.Size = new System.Drawing.Size(328, 22);
            this.TSMItemGonfigEmployees.Text = "Сотрудники";
            this.TSMItemGonfigEmployees.Click += new System.EventHandler(this.TSMItemGonfigEmployees_Click);
            // 
            // TSMItemGonfigOther
            // 
            this.TSMItemGonfigOther.Name = "TSMItemGonfigOther";
            this.TSMItemGonfigOther.Size = new System.Drawing.Size(328, 22);
            this.TSMItemGonfigOther.Text = "Основные";
            this.TSMItemGonfigOther.Click += new System.EventHandler(this.TSMItemGonfigOther_Click);
            // 
            // TSMItemLic
            // 
            this.TSMItemLic.Name = "TSMItemLic";
            this.TSMItemLic.Size = new System.Drawing.Size(328, 22);
            this.TSMItemLic.Text = "Лицензия";
            this.TSMItemLic.Click += new System.EventHandler(this.TSMItemLic_Click);
            // 
            // TSMItemAboutPrv
            // 
            this.TSMItemAboutPrv.Name = "TSMItemAboutPrv";
            this.TSMItemAboutPrv.Size = new System.Drawing.Size(328, 22);
            this.TSMItemAboutPrv.Text = "Доступные провайдеры";
            // 
            // TSMItemConfigDB
            // 
            this.TSMItemConfigDB.Name = "TSMItemConfigDB";
            this.TSMItemConfigDB.Size = new System.Drawing.Size(328, 22);
            this.TSMItemConfigDB.Text = "Подключение к базе данных";
            this.TSMItemConfigDB.Click += new System.EventHandler(this.TSMItemConfigDB_Click);
            // 
            // TSMItemConfigTerminalSD
            // 
            this.TSMItemConfigTerminalSD.Name = "TSMItemConfigTerminalSD";
            this.TSMItemConfigTerminalSD.Size = new System.Drawing.Size(328, 22);
            this.TSMItemConfigTerminalSD.Text = "Управление терминалами сбора данных";
            this.TSMItemConfigTerminalSD.Click += new System.EventHandler(this.TSMItemConfigTerminalSD_Click);
            // 
            // Pnl_TopRight
            // 
            this.Pnl_TopRight.Controls.Add(this.pctBoxSystemUnLock);
            this.Pnl_TopRight.Controls.Add(this.pctBoxSystemLock);
            this.Pnl_TopRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.Pnl_TopRight.Location = new System.Drawing.Point(462, 0);
            this.Pnl_TopRight.Name = "Pnl_TopRight";
            this.Pnl_TopRight.Size = new System.Drawing.Size(59, 27);
            this.Pnl_TopRight.TabIndex = 0;
            // 
            // pctBoxSystemUnLock
            // 
            this.pctBoxSystemUnLock.Dock = System.Windows.Forms.DockStyle.Right;
            this.pctBoxSystemUnLock.Image = ((System.Drawing.Image)(resources.GetObject("pctBoxSystemUnLock.Image")));
            this.pctBoxSystemUnLock.Location = new System.Drawing.Point(13, 0);
            this.pctBoxSystemUnLock.Name = "pctBoxSystemUnLock";
            this.pctBoxSystemUnLock.Size = new System.Drawing.Size(24, 27);
            this.pctBoxSystemUnLock.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pctBoxSystemUnLock.TabIndex = 2;
            this.pctBoxSystemUnLock.TabStop = false;
            this.pctBoxSystemUnLock.Visible = false;
            this.pctBoxSystemUnLock.Click += new System.EventHandler(this.pctBoxSystemUnLock_Click);
            // 
            // pctBoxSystemLock
            // 
            this.pctBoxSystemLock.Dock = System.Windows.Forms.DockStyle.Right;
            this.pctBoxSystemLock.Image = ((System.Drawing.Image)(resources.GetObject("pctBoxSystemLock.Image")));
            this.pctBoxSystemLock.Location = new System.Drawing.Point(37, 0);
            this.pctBoxSystemLock.Name = "pctBoxSystemLock";
            this.pctBoxSystemLock.Size = new System.Drawing.Size(22, 27);
            this.pctBoxSystemLock.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pctBoxSystemLock.TabIndex = 1;
            this.pctBoxSystemLock.TabStop = false;
            this.pctBoxSystemLock.Click += new System.EventHandler(this.pctBoxSystemLock_Click);
            // 
            // Pnl_Fill
            // 
            this.Pnl_Fill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Pnl_Fill.Location = new System.Drawing.Point(0, 27);
            this.Pnl_Fill.Name = "Pnl_Fill";
            this.Pnl_Fill.Size = new System.Drawing.Size(521, 320);
            this.Pnl_Fill.TabIndex = 3;
            // 
            // TSMItemConfigCrypto
            // 
            this.TSMItemConfigCrypto.Name = "TSMItemConfigCrypto";
            this.TSMItemConfigCrypto.Size = new System.Drawing.Size(328, 22);
            this.TSMItemConfigCrypto.Text = "Настройка подключения к криптопровайдеру";
            this.TSMItemConfigCrypto.Click += new System.EventHandler(this.TSMItemConfigCrypto_Click);
            // 
            // FStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 369);
            this.Controls.Add(this.Pnl_Fill);
            this.Controls.Add(this.Pnl_Top);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FStart";
            this.ShowInTaskbar = false;
            this.Text = "Сервис обслуживающий Prizm";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.FStart_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.Pnl_Top.ResumeLayout(false);
            this.Pnl_TopFill.ResumeLayout(false);
            this.Pnl_TopFill.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.Pnl_TopRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pctBoxSystemUnLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctBoxSystemLock)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tSSLabel;
        private System.Windows.Forms.Panel Pnl_Top;
        private System.Windows.Forms.Panel Pnl_TopFill;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem TSMItemGonfig;
        private System.Windows.Forms.ToolStripMenuItem TSMItemGonfigCashiries;
        private System.Windows.Forms.ToolStripMenuItem TSMItemGonfigEmployees;
        private System.Windows.Forms.ToolStripMenuItem TSMItemGonfigOther;
        private System.Windows.Forms.ToolStripMenuItem TSMItemLic;
        private System.Windows.Forms.ToolStripMenuItem TSMItemAboutPrv;
        private System.Windows.Forms.ToolStripMenuItem TSMItemConfigDB;
        private System.Windows.Forms.ToolStripMenuItem TSMItemConfigTerminalSD;
        private System.Windows.Forms.Panel Pnl_TopRight;
        private System.Windows.Forms.Panel Pnl_Fill;
        private System.Windows.Forms.PictureBox pctBoxSystemUnLock;
        private System.Windows.Forms.PictureBox pctBoxSystemLock;
        private System.Windows.Forms.ToolStripMenuItem TSMItemConfigCrypto;
    }
}

