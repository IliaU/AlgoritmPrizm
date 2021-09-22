using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.BLL
{
    /// <summary>
    /// Класс продукта принимаем решение нужно ли выдовать запрос матрикс кода и обязательный параметр матрикс код или нет
    /// </summary>
    public class ProdictMatrixClass
    {
        /// <summary>
        /// Идентификатор класса продуктов
        /// </summary>
        public string ProductClass { get; private set; }

        /// <summary>
        /// Необходимый
        /// </summary>
        public bool Mandatory { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ProductClass">Идентификатор класса продуктов</param>
        /// <param name="Mandatory">Необходимый</param>
        public ProdictMatrixClass(string ProductClass, bool Mandatory)
        {
            this.ProductClass = ProductClass;
            this.Mandatory = Mandatory;
        }
    }
}
