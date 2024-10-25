using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Security.Cryptography.X509Certificates;

namespace AlgoritmPrizm
{
    public partial class FCryptoSetup : Form
    {
        /// <summary>
        /// Список доступных сертификатов
        /// </summary>
        private List<string> CrtList = null;

        /// <summary>
        /// Текущий сертификат
        /// </summary>
        private X509Certificate CurSert;

        /// <summary>
        /// Конструктор
        /// </summary>
        public FCryptoSetup()
        {
            try
            {
                InitializeComponent();

                this.txtBoxDefaultActivMinToken.Text = Com.Config.DefaultActivMinToken.ToString();
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Не можем загрузить форму FCryptoSetup произошла ошибка: ({0})", ex.Message), "FCryptoSetup.FCryptoSetup()", Lib.EventEn.Error, true, true);
            }
        }

        // Чтение данных
        private void FCryptoSetup_Load(object sender, EventArgs e)
        {
            try
            {
                // Заполняем поля значениями по умолчанию
                if (string.IsNullOrWhiteSpace(Com.Config.CrptCert)) this.lblTitle.Text = this.lblTitle.Tag.ToString().Replace("@CurSert", "сертификат не выбран");
                else this.lblTitle.Text = this.lblTitle.Tag.ToString().Replace("@CurSert", "сертификат не валиден");
                this.lblValidCert.Text = this.lblValidCert.Tag.ToString().Replace(@"@Now", "");

                // Пробегаем по списку доступных сертификатов
                foreach (string item in Com.Crypto.GetListSert())
                {
                    // Если в настройке стоит существующий сертификат
                    if ((CurSert==null && Com.Config.CrptCert == item)
                        || (CurSert != null && CurSert.Subject == item))
                    {
                        // И если он валидный то обновляем инфу в форме по выбранному сертификату
                        if (CurSert == null)
                        {
                            this.lblTitle.Text = this.lblTitle.Tag.ToString().Replace("@CurSert", Com.Crypto.curCert.Subject);
                            this.lblValidCert.Text = this.lblValidCert.Tag.ToString().Replace(@"@Now", Com.Crypto.GetExpirationDate.ToShortDateString());
                        }
                        else
                        {
                            this.lblTitle.Text = this.lblTitle.Tag.ToString().Replace("@CurSert", this.CurSert.Subject);
                            this.lblValidCert.Text = this.lblValidCert.Tag.ToString().Replace(@"@Now", this.CurSert.GetExpirationDateString());
                        }
                    }
                }

                
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Не можем загрузить форму FCryptoSetup произошла ошибка: ({0})", ex.Message), "FCryptoSetup.FCryptoSetup()", Lib.EventEn.Error, true, true);
            }
        }

        // Пользователь созраняет изменения
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.CurSert != null)
                {
                    Com.Crypto.OpenSert(this.CurSert.Subject);
                    Com.Config.CrptCert = this.CurSert.Subject;
                }


                Com.Config.DefaultActivMinToken = int.Parse(this.txtBoxDefaultActivMinToken.Text);

                this.Close();
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Не можем сохранить текущий сертификат: ({0})", ex.Message), "FCryptoSetup.btnSave_Click", Lib.EventEn.Error, true, true);
            }


        }

        // Пользователь выбирает сертификат
        private void btnChoiseSet_Click(object sender, EventArgs e)
        {
            this.CurSert = Com.Crypto.SelectedSert();
            FCryptoSetup_Load(null, null);
        }


    }
}
