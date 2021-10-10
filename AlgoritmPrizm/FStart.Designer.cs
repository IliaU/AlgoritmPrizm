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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.TSMItemGonfig = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMItemGonfigCashiries = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMItemGonfigOther = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tSSLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.Pnl = new System.Windows.Forms.Panel();
            this.TSMItemLic = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMItemGonfig});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(521, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // TSMItemGonfig
            // 
            this.TSMItemGonfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMItemGonfigCashiries,
            this.TSMItemGonfigOther,
            this.TSMItemLic});
            this.TSMItemGonfig.Name = "TSMItemGonfig";
            this.TSMItemGonfig.Size = new System.Drawing.Size(78, 20);
            this.TSMItemGonfig.Text = "Настройка";
            // 
            // TSMItemGonfigCashiries
            // 
            this.TSMItemGonfigCashiries.Name = "TSMItemGonfigCashiries";
            this.TSMItemGonfigCashiries.Size = new System.Drawing.Size(152, 22);
            this.TSMItemGonfigCashiries.Text = "Кассиры";
            this.TSMItemGonfigCashiries.Click += new System.EventHandler(this.TSMItemGonfigCashiries_Click);
            // 
            // TSMItemGonfigOther
            // 
            this.TSMItemGonfigOther.Name = "TSMItemGonfigOther";
            this.TSMItemGonfigOther.Size = new System.Drawing.Size(152, 22);
            this.TSMItemGonfigOther.Text = "Основные";
            this.TSMItemGonfigOther.Click += new System.EventHandler(this.TSMItemGonfigOther_Click);
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
            // Pnl
            // 
            this.Pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Pnl.Location = new System.Drawing.Point(0, 24);
            this.Pnl.Name = "Pnl";
            this.Pnl.Size = new System.Drawing.Size(521, 323);
            this.Pnl.TabIndex = 2;
            // 
            // TSMItemLic
            // 
            this.TSMItemLic.Name = "TSMItemLic";
            this.TSMItemLic.Size = new System.Drawing.Size(152, 22);
            this.TSMItemLic.Text = "Лицензия";
            this.TSMItemLic.Click += new System.EventHandler(this.TSMItemLic_Click);
            // 
            // FStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 369);
            this.Controls.Add(this.Pnl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FStart";
            this.ShowInTaskbar = false;
            this.Text = "Сервис обслуживающий Prizm";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem TSMItemGonfig;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tSSLabel;
        private System.Windows.Forms.Panel Pnl;
        private System.Windows.Forms.ToolStripMenuItem TSMItemGonfigCashiries;
        private System.Windows.Forms.ToolStripMenuItem TSMItemGonfigOther;
        private System.Windows.Forms.ToolStripMenuItem TSMItemLic;
    }
}

