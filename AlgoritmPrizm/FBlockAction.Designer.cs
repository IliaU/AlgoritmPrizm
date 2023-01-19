
namespace AlgoritmPrizm
{
    partial class FBlockAction
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FBlockAction));
            this.lblBlockAction = new System.Windows.Forms.Label();
            this.btnBlockAction = new System.Windows.Forms.Button();
            this.txtBoxBlockAction = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblBlockAction
            // 
            this.lblBlockAction.AutoSize = true;
            this.lblBlockAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblBlockAction.ForeColor = System.Drawing.Color.Maroon;
            this.lblBlockAction.Location = new System.Drawing.Point(12, 24);
            this.lblBlockAction.Name = "lblBlockAction";
            this.lblBlockAction.Size = new System.Drawing.Size(401, 20);
            this.lblBlockAction.TabIndex = 0;
            this.lblBlockAction.Text = "Для доступа к системе необходимо ввести пароль.";
            // 
            // btnBlockAction
            // 
            this.btnBlockAction.Location = new System.Drawing.Point(219, 72);
            this.btnBlockAction.Name = "btnBlockAction";
            this.btnBlockAction.Size = new System.Drawing.Size(111, 23);
            this.btnBlockAction.TabIndex = 1;
            this.btnBlockAction.Text = "Проверить пароль";
            this.btnBlockAction.UseVisualStyleBackColor = true;
            this.btnBlockAction.Click += new System.EventHandler(this.btnBlockAction_Click);
            // 
            // txtBoxBlockAction
            // 
            this.txtBoxBlockAction.Location = new System.Drawing.Point(43, 74);
            this.txtBoxBlockAction.Name = "txtBoxBlockAction";
            this.txtBoxBlockAction.Size = new System.Drawing.Size(149, 20);
            this.txtBoxBlockAction.TabIndex = 2;
            // 
            // FBlockAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 126);
            this.Controls.Add(this.txtBoxBlockAction);
            this.Controls.Add(this.btnBlockAction);
            this.Controls.Add(this.lblBlockAction);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FBlockAction";
            this.Text = "Ввод пароля для доступа к системным настройкам";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FBlockAction_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBlockAction;
        private System.Windows.Forms.Button btnBlockAction;
        private System.Windows.Forms.TextBox txtBoxBlockAction;
    }
}