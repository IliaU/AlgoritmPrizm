using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmListener
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override async void OnStart(string[] args)
        {
            // Запускаем службу чтобы работала бесконечно
            while(true)
            {
                // Пауза между циклами
                await Task.Delay(3000);
            }
        }

        protected override void OnStop()
        {
        }
    }
}
