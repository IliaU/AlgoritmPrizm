using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Lib
{
    /// <summary>
    /// Результат процедуры Com.FR GetSummCheck
    /// </summary>
    public class FrGetSummCheckRez
    {
        /// <summary>
        /// Общий итог по документу сколько всего денег
        /// </summary>
        public decimal itog;

        /// <summary>
        /// Тпы оплат например нал безнал без детализации
        /// </summary>
        public List<FrGetSummCheckRezType> TenderTyp = new List<FrGetSummCheckRezType>();

        /// <summary>
        /// Добавление суммыв текущий объект
        /// </summary>
        /// <param name="TenderType">Тип оплаты нал безнал</param>
        /// <param name="TenderName">Имя безналичного расчёта например виза или мастеркард</param>
        /// <param name="value">Сумма которую нужно обработать</param>
        public void SetSumm(FrGetSummCheckRezTypeEn TenderType, string TenderName, decimal TenderValue)
        {
            try
            {
                itog += TenderValue;

                switch (TenderType)
                {
                    case FrGetSummCheckRezTypeEn.Nal:
                        int flagAddNal = -1;
                        for (int i = 0; i < TenderTyp.Count; i++)
                        {
                            if (TenderTyp[i].TenderTyp == FrGetSummCheckRezTypeEn.Nal)
                            {
                                flagAddNal = i;
                                break;
                            }
                        }
                        if (flagAddNal == -1) TenderTyp.Add(new FrGetSummCheckRezType(FrGetSummCheckRezTypeEn.Nal, TenderValue));
                        else TenderTyp[flagAddNal].PromItog+= TenderValue;
                        break;
                    case FrGetSummCheckRezTypeEn.BezNal:
                        int flagAddBezNal = -1;
                        for (int i = 0; i < TenderTyp.Count; i++)
                        {
                            if (TenderTyp[i].TenderTyp == FrGetSummCheckRezTypeEn.BezNal)
                            {
                                flagAddBezNal = i;
                                break;
                            }
                        }
                        if (flagAddBezNal == -1)
                        {
                            TenderTyp.Add(new FrGetSummCheckRezType(FrGetSummCheckRezTypeEn.BezNal, TenderValue));
                            TenderTyp[TenderTyp.Count-1].TenderName.Add(new FrGetSummCheckRezTypeTenderName(TenderName, TenderValue));
                        }
                        else
                        {
                            TenderTyp[flagAddBezNal].PromItog += TenderValue;

                            int flagAddBezNal2 = -1;
                            for (int ii = 0; ii < TenderTyp[flagAddBezNal].TenderName.Count; ii++)
                            {
                                if (TenderTyp[flagAddBezNal].TenderName[ii].Name == TenderName)
                                {
                                    flagAddBezNal2 = ii;
                                    break;
                                }
                            }

                            if (flagAddBezNal2 == -1) TenderTyp[flagAddBezNal].TenderName.Add(new FrGetSummCheckRezTypeTenderName(TenderName, TenderValue));
                            else TenderTyp[flagAddBezNal].TenderName[flagAddBezNal2].Value+= TenderValue;
                        }
                        break;
                    case FrGetSummCheckRezTypeEn.Credit:
                        int flagAddCredit = -1;
                        for (int i = 0; i < TenderTyp.Count; i++)
                        {
                            if (TenderTyp[i].TenderTyp == FrGetSummCheckRezTypeEn.Credit)
                            {
                                flagAddCredit = i;
                                break;
                            }
                        }
                        if (flagAddCredit == -1) TenderTyp.Add(new FrGetSummCheckRezType(FrGetSummCheckRezTypeEn.Credit, TenderValue));
                        else TenderTyp[flagAddCredit].PromItog += TenderValue;
                        break;
                    case FrGetSummCheckRezTypeEn.Predoplata:
                        int flagAddPred = -1;
                        for (int i = 0; i < TenderTyp.Count; i++)
                        {
                            if (TenderTyp[i].TenderTyp == FrGetSummCheckRezTypeEn.Predoplata)
                            {
                                flagAddPred = i;
                                break;
                            }
                        }
                        if (flagAddPred == -1) TenderTyp.Add(new FrGetSummCheckRezType(FrGetSummCheckRezTypeEn.Predoplata, TenderValue));
                        else TenderTyp[flagAddPred].PromItog += TenderValue;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
