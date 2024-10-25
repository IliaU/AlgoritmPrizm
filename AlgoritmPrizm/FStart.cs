using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Threading;
using AlgoritmPrizm.Lib;


namespace AlgoritmPrizm
{
    public partial class FStart : Form
    {
        private Color DefBaskCoclortSSLabel;
        private object LockObj = new object();

        private NotifyIcon m_notifyicon;
        private ContextMenu m_menu;

        private Size WindowSize = SystemInformation.PrimaryMonitorMaximizedWindowSize;
        private bool isClosed = false;

        private Point FLocation;
        private Size FSize;
        private Boolean DoubleClickIsShow;

        // В каком статусе настройка блокировки настроечного меню
        private bool GonfigBlock=true;
        private bool IsRunAsinGonfigBlock = false;
        private Thread ThrGonfigBlock;
        private DateTime GonfigBlockDatetime = DateTime.Now;

        // Экспортируем неуправляемую библиотеку и создаём переменную и константы для работы нашей библиотеки
        private const int NotifyForThisSession = 0;
        private const int SessionChangeMessage = 0x02B1;
        private const int SessionLockParam = 0x7;
        private const int SessionUnlockParam = 0x8;
        //
        [DllImport("wtsapi32.dll")]
        private static extern bool WTSRegisterSessionNotification(IntPtr hWnd, int dwFlags);
        //
        [DllImport("wtsapi32.dll")]
        private static extern bool WTSUnRegisterSessionNotification(IntPtr hWnd);
        //
        private bool registered = false;            // Состояние блокировки   вместо переменной в форме стал использовать переменную в классе this.com.HashLock

        /// <summary>
        /// Для того чтобы статус в нижней части работал последлвательно
        /// </summary>
        private object LockEventLog = new object();

        public FStart()
        {
            InitializeComponent();
            this.DefBaskCoclortSSLabel = this.tSSLabel.BackColor;

            m_menu = new ContextMenu();
            m_menu.MenuItems.Add(0, new MenuItem("Show", new System.EventHandler(Show_Click)));
            m_menu.MenuItems.Add(1, new MenuItem("Hide", new System.EventHandler(Hide_Click)));
            m_menu.MenuItems.Add(2, new MenuItem("Exit", new System.EventHandler(Exit_Click)));
            //m_menu.MenuItems.Add(3, new MenuItem("Тест всплывающего окна", new System.EventHandler(Test_Click)));

            m_notifyicon = new NotifyIcon();
            m_notifyicon.Text = "Щёлкните правой кнопкой мыши для вызова контекстного меню.";
            m_notifyicon.Visible = true;
            m_notifyicon.Icon = new Icon(GetType(), @"Icon.ico");
            m_notifyicon.ContextMenu = m_menu;
            m_notifyicon.DoubleClick += new EventHandler(DoubleClickIcon);

            // Подписываемся на события 
            Com.Log.onEventLog += new EventHandler<Lib.EventLog>(Log_onEventLog); //Com.Log.EventSave("Тест", this.GetType().Name, EventEn.Error);
            //Com.Lic.onRegNewKeyAfte += new EventHandler<Com.LicLib.onLicEventKey>(Lic_onRegNewKey);

            // Подключаем список доступных провайдеров для подключения ToolStripMenuItem
            // TSMItemAboutPrv
            foreach (string item in Com.ProviderFarm.ListProviderName())
            {
                this.TSMItemAboutPrv.DropDownItems.Add((new Lib.UProvider(item)).InfoToolStripMenuItem());
            }

            //
            Com.Crypto.onEventChangeHashExecuting += new EventHandler<Lib.EventCrypto>(Crypto_onEventChangeHashExecuting);

            // Асинхронный запуск процесса
            this.IsRunAsinGonfigBlock = true;
            GonfigBlockDatetime = DateTime.Now;
            ThrGonfigBlock = new Thread(AGonfigBlock); //Запуск с параметрами   
            ThrGonfigBlock.Name = "AGonfigBlock";
            ThrGonfigBlock.IsBackground = true;
            ThrGonfigBlock.Start(); // Передаём на сколько по времени разблокируется кнопака сверху
        }

        /// <summary>
        /// Закрывает форму
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Exit_Click(Object sender, System.EventArgs e)
        {
            this.IsRunAsinGonfigBlock = false;
            this.ThrGonfigBlock.Join();
            isClosed = true;
            this.m_notifyicon.Dispose();
            Close();
        }

        /// <summary>
        /// Скрывает форму
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Hide_Click(Object sender, System.EventArgs e)
        {
            // Запоминаем положение и минимизируем форму
            this.FLocation = (this.Location.X > 0 && this.Location.Y > 0 ? this.Location : this.FLocation);
            this.FSize = (this.Size.Height > 50 && this.Size.Width > 200 ? this.Size : this.FSize);
            this.WindowState = FormWindowState.Minimized;
            this.DoubleClickIsShow = false;

            // Скрываем нашу форму
            Hide();

            // Переподписываемся на новое событие
            //m_notifyicon.DoubleClick -= new EventHandler(Hide_Click);
            //m_notifyicon.DoubleClick += new EventHandler(Show_Click);
        }

        /// <summary>
        /// Отображает скрытую форму
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Show_Click(Object sender, System.EventArgs e)
        {
            // Опять отображаем форму
            Show();

            // Разворачиваем форму и востанавливаем размер и положение
            this.WindowState = FormWindowState.Normal;
            this.Location = (this.FLocation.X > 0 && this.FLocation.Y > 0 ? this.FLocation : this.Location);
            this.Size = (this.FSize.Height > 50 && this.FSize.Width > 200 ? this.FSize : this.Size);
            this.DoubleClickIsShow = true;

            // Иногда исчезает подписка на двойной клик. Если вдруг исчезла, то подписываемся снова
            m_notifyicon.DoubleClick -= new EventHandler(DoubleClickIcon);
            m_notifyicon.DoubleClick += new EventHandler(DoubleClickIcon);
        }

        /// <summary>
        /// Пользователь вызвал двойной клик по иконке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoubleClickIcon(Object sender, System.EventArgs e)
        {
            lock (this)
            {
                if (!this.DoubleClickIsShow)
                {
                    this.Show_Click(sender, e);
                    this.DoubleClickIsShow = true;
                }
                else
                {
                    this.Hide_Click(sender, e);
                    this.DoubleClickIsShow = false;
                }
            }
        }

        /// <summary>
        /// Тест всплывающего уведомления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Test_Click(Object sender, System.EventArgs e)
        {   // А вот так можно вызвать стандартное уведомление
            this.m_notifyicon.ShowBalloonTip(5000, "Заголовок", "Текст\r\n gggg\r\n jjjjjjjjjjjjj", ToolTipIcon.Info);

            //F_Message frm = new F_Message(this.com);
            //frm.Show();
            //frm.Location = new System.Drawing.Point(WindowSize.Width - frm.Size.Width-5, WindowSize.Height - frm.Size.Height - 10);

            m_notifyicon.Icon = new Icon(GetType(), "IconE.ico");
        }

        /// <summary>
        /// Скрывает форму при нажатии на крестик
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.isClosed)
            {
                this.Hide_Click(null, null);
                e.Cancel = true;
            }
        }

        // Отписываемся от события в внешней dll при закрытии формы
        private void MyDispose(bool disposing)      // ОСОБЕННОСТЬ Добавил вызов этого метода в конструкторе который ведёт сама студия F_Start_Designer.cs
        {
            // Надеюсь поможет от появления нескольких экземпляров этого приложения
            try
            {
                m_notifyicon.DoubleClick -= new EventHandler(DoubleClickIcon);
                //this.com.onEventStatus -= new EventHandler<Lib.EventStatus>(com_onEventStatus);
                //this.com.onEventStatusTime -= new EventHandler<Lib.EventStatus>(com_onEventStatusTime);

                WTSUnRegisterSessionNotification(Handle); // Handle -- Дескриптор текущего окна
                registered = false;
            }
            catch (Exception) { }

            if (registered)
            {
                WTSUnRegisterSessionNotification(Handle); // Handle -- Дескриптор текущего окна
                registered = false;
            }

            return;
        }

        // Переписываем метод подписания на системные события тем самым получаем событие из внешней библиотеки WndProc
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // WtsRegisterSessionNotification requires Windows XP or higher           (На старых виндах это не работает)
            bool haveXp = Environment.OSVersion.Platform == PlatformID.Win32NT &&
                                (Environment.OSVersion.Version.Major > 5 ||
                                    (Environment.OSVersion.Version.Major == 5 &&
                                     Environment.OSVersion.Version.Minor >= 1));

            if (haveXp)
                registered = WTSRegisterSessionNotification(Handle, NotifyForThisSession);      // Собственно подписываемся на событие

            return;
        }

        // Событие из внешней библиотеки
        protected override void WndProc(ref Message m)
        {
            // check for session change notifications   (Сравниваем сообщение пришедшее из внешней dll с нашей константой)
            if (m.Msg == SessionChangeMessage)
            {
                if (m.WParam.ToInt32() == SessionLockParam)
                    OnSessionLock();
                else if (m.WParam.ToInt32() == SessionUnlockParam)
                    OnSessionUnlock();
            }

            base.WndProc(ref m);                        // Отдаём управление внешней dll
            return;
        }

        // Произошла блокировка компа
        protected virtual void OnSessionLock()
        {
           // this.com.Log.EventSave("Компьютер заблокирован", this.ToString(), Lib.EventEn.Message);
           // this.com.onEventStatus -= new EventHandler<Lib.EventStatus>(com_onEventStatus);
           // this.com.onEventStatusTime -= new EventHandler<Lib.EventStatus>(com_onEventStatusTime);
          //  this.com.HashLock = true;
            return;
        }

        // Комп разблокирован
        protected virtual void OnSessionUnlock()
        {
           // this.com.Log.EventSave("Компьютер разблокирован", this.ToString(), Lib.EventEn.Message);
            // Иногда исчезает подписка на двойной клик. Если вдруг исчезла, то подписываемся снова
            m_notifyicon.DoubleClick -= new EventHandler(DoubleClickIcon);
            m_notifyicon.DoubleClick += new EventHandler(DoubleClickIcon);
           // this.com.onEventStatus -= new EventHandler<Lib.EventStatus>(com_onEventStatus);
          //  this.com.onEventStatus += new EventHandler<Lib.EventStatus>(com_onEventStatus);
           // this.com.onEventStatusTime -= new EventHandler<Lib.EventStatus>(com_onEventStatusTime);
          //  this.com.onEventStatusTime += new EventHandler<Lib.EventStatus>(com_onEventStatusTime);
           // this.com.HashLock = false;
          //  this.com.RefreshEvent = true;       // принудительно заставляем перечитать таблицу с результатами
            return;
        }

        // Произошло событие системное правим текущий статус
        delegate void delig_Log_onEventLog(object sender, Lib.EventLog e);
        void Log_onEventLog(object sender, Lib.EventLog e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    lock (this.LockEventLog)
                    {
                        delig_Log_onEventLog dl = new delig_Log_onEventLog(Log_onEventLog);
                        this.Invoke(dl, new object[] { sender, e });
                    }
                }
                else
                {
                    lock (this.LockObj)
                    {
                        if (e != null)
                        {
                            switch (e.Evn)
                            {
                                case Lib.EventEn.Empty:
                                case Lib.EventEn.Dump:
                                    break;
                                case Lib.EventEn.Warning:
                                    this.tSSLabel.BackColor = Color.Khaki;
                                    this.tSSLabel.Text = e.Message;
                                    break;
                                case Lib.EventEn.Error:
                                case Lib.EventEn.FatalError:
                                    this.tSSLabel.BackColor = Color.Tomato;
                                    this.tSSLabel.Text = e.Message;
                                    break;
                                default:
                                    this.tSSLabel.BackColor = this.DefBaskCoclortSSLabel;
                                    this.tSSLabel.Text = e.Message;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "Log_onEventLog", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
            }
        }

        delegate void delig_Token(object sender, Lib.EventCrypto e);
        /// <summary>
        /// Отрисовка событий связанных с изменением статуса наличия заданий по токену
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Crypto_onEventChangeHashExecuting(object sender, Lib.EventCrypto e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    delig_Token dl = new delig_Token(Crypto_onEventChangeHashExecuting);
                    this.Invoke(dl, new object[] { sender, e });
                }
                else
                {
                    // Отрисовываем агрегированный статус
                    //SetStatpnlTopRight();
                }
            }
            catch (Exception ex)
            {
                // если завершение работы то ошибку не фиксируем
                //if (this.IsRunAUpdateStatusCon) Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "Crypto_onEventChangeHashExecuting", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
            }
        }

        // Список касиров
        private void TSMItemGonfigCashiries_Click(object sender, EventArgs e)
        {
            try
            {
                if (SystemBlockAction(false))
                {
                    using (FCashiries Frm = new FCashiries())
                    {
                        Frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "TSMItemGonfigCashiries_Click", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
            }
        }

        // Список сотрудников
        private void TSMItemGonfigEmployees_Click(object sender, EventArgs e)
        {
            try
            {
                if (SystemBlockAction(false))
                {
                    using (FEmployee Frm = new FEmployee())
                    {
                        Frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "TSMItemGonfigEmployees_Click", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
            }
        }

        // Основные настройки
        private void TSMItemGonfigOther_Click(object sender, EventArgs e)
        {
            try
            {
                if (SystemBlockAction(false))
                {
                    FConfig Frm = new FConfig();
                    Frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "TSMItemGonfigOther_Click", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
            }
        }

        // Пользователь вызывает форму с лицензиями
        private void TSMItemLic_Click(object sender, EventArgs e)
        {
            try
            {
                if (SystemBlockAction(false))
                {
                    using (FLic Frm = new FLic())
                    {
                        Frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "TSMItemLic_Click", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
            }
        }

        private void FStart_Load(object sender, EventArgs e)
        {
            try
            {
                // Если есть подключение
                if (Com.ProviderFarm.HashConnect())
                {
                    if (Com.ProviderFarm.CurrentPrv == null || !Com.ProviderFarm.CurrentPrv.HashConnect) throw new ApplicationException("Не установлено подключение с базой данных.");
                    else
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(ex.Message, GetType().Name + ".FStart_Load", Lib.EventEn.Error, true, true);
            }
        }
       
        // Пользователь решил поправить подключение
        private void TSMItemConfigDB_Click(object sender, EventArgs e)
        {
            if (SystemBlockAction(false))
            {
                using (FConnetSetup Frm = new FConnetSetup())
                {
                    Frm.ShowDialog();
                }
            }
        }

        // Пользователь решил править инфорамцию про терминал сбора данных
        private void TSMItemConfigTerminalSD_Click(object sender, EventArgs e)
        {
            if (SystemBlockAction(false))
            {
                using (FTerminalSD Frm = new FTerminalSD())
                {
                    Frm.ShowDialog();
                }
            }
        }

        // Пользователь настраивает подключение к криптопровайдеру
        private void TSMItemConfigCrypto_Click(object sender, EventArgs e)
        {
            try
            {
                if (SystemBlockAction(false))
                {
                    using (FCryptoSetup Frm = new FCryptoSetup())
                    {
                        Frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "TSMItemConfigCrypto_Click", ex.Message), this.GetType().FullName, EventEn.Error, true, true);
            }
        }




        delegate bool dl_SystemBlockAction(bool HashRender);
        /// <summary>
        /// Проверка доступа и возвращает результат проверки предоставлн ли доступ к системному меню
        /// </summary>
        /// <param name="HashRender">Флаг заставляющий только переключить переключатель без ввода логина и пароля для того чтобы асинхронный процесс не попал на ввод пароля</param>
        /// <returns>Возвражает true если пользователю разрешено редактировать в конфиге</returns>
        private bool SystemBlockAction(bool HashRender)
        {
            bool RezLock = false;

            try
            {
                if (this.InvokeRequired)
                {
                    dl_SystemBlockAction dl = new dl_SystemBlockAction(SystemBlockAction);
                    return (bool) this.Invoke(dl, new object[] { HashRender });
                }
                else
                {
                    // Если запрашивает пользователь то он просит вывести интерфейс если нет то это парралельны процесс и интерфейс выводить не стои
                    if (HashRender) this.GonfigBlock = false;   // если без ввывода интерфейса значит это поток который только может блокировать соответсвенно выставляем что текущее состояние не заблокировано чтобы дальше система переключила в другое состояние
                    else
                    {
                        // Если сейчас админская консоль разблокирована то надо просто обновить время чтобы она не закрылась
                        if (!this.GonfigBlock)
                        {
                            this.GonfigBlockDatetime = DateTime.Now;
                            return true;
                        }
                        else
                        {
                            // Теперь можно спросить у пользователя пароль
                            DialogResult RrFBlockAction = DialogResult.No;
                            using (FBlockAction Frm = new FBlockAction())
                            {
                                RrFBlockAction = Frm.ShowDialog();
                            }
                            
                            // Проверяем результат проверки пароля если пользователь ввёл его успешно то выставляем что форма заблокирована аэтот влаг поменяется при последующей обработке
                            if (RrFBlockAction == DialogResult.Yes)
                            {
                                Com.Log.EventSave("Пароль введён верный к настройке системных параметров.", "Fstart.SystemBlockAction", EventEn.Message);
                                this.GonfigBlock = true;
                                RezLock = true;
                            }
                            else // Если пользователь не правильно ввёл пароль то увы говорим ему об этом и блокируем доступ
                            {
                                Com.Log.EventSave("Введён не верный пароль к настройке системных параметров.", "Fstart.SystemBlockAction", EventEn.Error, true, true);
                                this.GonfigBlock = false;
                                RezLock = false;
                            }

                            // Если пользователь вёл правильный пароль то ничего не делаем
                            //а вот еслли не правильный то поступаем как с асинхронным потоком просто присваиваем жёстко переменной значение
                            //
                        }
                    }


                    // Проверяем текущий статус
                    if (this.GonfigBlock)
                    {
                        // Меняем видимость картинок
                        this.pctBoxSystemLock.Visible = false;
                        this.pctBoxSystemUnLock.Visible = true;

                        // Ставим текужее время чтобы было что сравнивать механизму блокировки
                        this.GonfigBlockDatetime = DateTime.Now;

                        // Было заблокировано теперь отображаем элемент
                        this.GonfigBlock = false;
                    }
                    else
                    {
                        // Меняем видимость картинок
                        this.pctBoxSystemUnLock.Visible = false;
                        this.pctBoxSystemLock.Visible = true;

                        // Ставим текужее время чтобы было что сравнивать механизму блокировки
                        this.GonfigBlockDatetime = DateTime.Now;

                        // Было разблокировано теперь скрываем элемент элемент
                        this.GonfigBlock = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Com.Log.EventSave(ae.Message, "Fstart.SystemBlockAction", EventEn.Error);
                //throw ae;
            }

            return RezLock;
        }

      

        // Отдельный поток который будет блокировать вход через время
        private void AGonfigBlock (/*object TimeOutLock*/)
        {
            try
            {
                // Цыкл для риёма запросов от пользователя
                while (IsRunAsinGonfigBlock)
                {
                    // Проверяем текущий статус если не даблокировано и таймаут исчерпался то надо заблокировать путём нажатия на кнопку блокировки
                    if (!this.GonfigBlock && this.GonfigBlockDatetime.AddSeconds(Com.Config.BlockActionTimeOut) <DateTime.Now)
                    {
                        this.SystemBlockAction(true);
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Com.Log.EventSave(ae.Message, "Fstart.AGonfigBlock", EventEn.Error);
                //throw ae;
            }
        }

        // Пользователь разблокировать хочет через нажатие на закрытый замок
        private void pctBoxSystemLock_Click(object sender, EventArgs e)
        {
            try
            {
                this.SystemBlockAction(false);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Com.Log.EventSave(ae.Message, "Fstart.pctBoxSystemLock_Click", EventEn.Error);
                //throw ae;
            }
        }

        // Пользователь хочет заблокировать через нажатие на открытый замок
        private void pctBoxSystemUnLock_Click(object sender, EventArgs e)
        {
            try
            {
                this.SystemBlockAction(true);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Com.Log.EventSave(ae.Message, "Fstart.pctBoxSystemUnLock_Click", EventEn.Error);
                //throw ae;
            }
        }

        
    }
}
