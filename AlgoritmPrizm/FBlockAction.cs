using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AlgoritmPrizm.Lib;

namespace AlgoritmPrizm
{
    public partial class FBlockAction : Form
    {
        private DialogResult? rez = null;

        /// <summary>
        /// Конструктор
        /// </summary>
        public FBlockAction()
        {
            InitializeComponent();
        }

        // Закрытие формы
        private void FBlockAction_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (rez != null) this.DialogResult = (DialogResult)rez;
                else this.DialogResult = DialogResult.No;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Com.Log.EventSave(ae.Message, "Fstart.btnBlockAction_Click", EventEn.Error);
                //throw ae;
            }
        }

        // Пользователь нажал на проверку пароля
        private void btnBlockAction_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.txtBoxBlockAction.Text == Com.Config.BlockActionPassword)
                {
                    this.rez = DialogResult.Yes;
                }
                else
                {
                    this.rez = DialogResult.No;
                }
                this.Close();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Com.Log.EventSave(ae.Message, "Fstart.btnBlockAction_Click", EventEn.Error);
                //throw ae;
            }
        }
    }
}
