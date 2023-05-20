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
using AlgoritmPrizm.Com;

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
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при чтении конфигурации с ошибкой: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
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
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении конфигурации с ошибкой: ({0})", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.btnBlockAction_Click", GetType().Name), EventEn.Error);
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
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении конфигурации с ошибкой: ({0})", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.btnBlockAction_Click", GetType().Name), EventEn.Error);
                //throw ae;
            }
        }

        // Пользователь нажимает любую клавишу
        private void txtBoxBlockAction_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter )
                {
                    this.btnBlockAction_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении конфигурации с ошибкой: ({0})", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.txtBoxBlockAction_KeyDown", GetType().Name), EventEn.Error);
                //throw ae;
            }
        }
    }
}
