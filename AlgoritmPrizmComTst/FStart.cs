using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AlgoritmPrizmCom;

namespace AlgoritmPrizmComTst
{
    public partial class FStart : Form
    {
        public FStart()
        {
            InitializeComponent();
        }

        private void btnReguestCdnForIsmpCheck_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtBoxResponceCdnForIsmpCheck.Text = CdnResponce.SerializeObject(Web.CdnForIsmpCheck(this.txtBoxReguestCdnForIsmpCheck.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCdnForIsmpConfig_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtBoxCdnForIsmpConfig.Text = CdnResponceConfig.SerializeObject(Web.CdnForIsmpConfig());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Перезагрузка Сервисов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRestartServices_Click(object sender, EventArgs e)
        {
            try
            {
                PrizmServiceController.ServiceControllerRegimeYeniseyRestart();
                MessageBox.Show("Сервисы успешно перезагружены");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
