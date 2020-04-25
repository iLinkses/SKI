using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteDBConnection;

namespace SKI
{
    class AnalysisByParameters
    {
        //public AnalysisByParameters() { }
        SQLiteDB db = new SQLiteDB();
        

        public string Analysis(double M, double Pm, double t1, double t2, double fi, double Ge, double Pl, double W)
        {
            string result = "";
            if (Pm > 0.6)//Потери массы выше нормы
            {
                if (t1 > 60)//Температура в 1- реакторе выше нормы, остальные параметры в допустимых диапазонах
                {
                    if (M > 75)//Температура в 1- реакторе выше нормы, повышение показателя вязкости по Муни
                    {
                        var SES = db.GetSES().SingleOrDefault(s => s.ID_SES == 2);
                        //if (SES == null) return;
                        result = SES.SolutionOfES;
                        return result;
                    }
                    else
                    {
                        var SES = db.GetSES().SingleOrDefault(s => s.ID_SES == 1);
                        //if (SES == null) return;
                        result = SES.SolutionOfES;
                        return result;
                    } 
                }
                
                else
                {
                    //---   !!!   ---
                    //if (t1 > 60 || M > 75)//Температура в 1- реакторе выше нормы, вязкость по Муни и ОСТАЛЬНЫЕ (какие?) параметры вышли за допустимый диапазон
                    //{
                    //}
                    var SES = db.GetSES().SingleOrDefault(s => s.ID_SES == 3);
                    //if (SES == null) return;
                    result = SES.SolutionOfES;
                    return result;
                }
            }

            if (Ge < 20 || Ge > 30)//Концентрация геля
            {
                //Соотношение ТИБА/ Тi Cl4 ниже оптимального (остальные параметры в допустимых диапазонах)

                if (Pm > 0.6)//Рост потерь масс вышел за допустимые диапазоны
                {//Странно выводит результаты
                    if (M < 55)//Рост потерь масс вышел за допустимые диапазоны и вязкость по Муни ниже нормы
                    {
                        var SES = db.GetSES().SingleOrDefault(s => s.ID_SES == 6);
                        //if (SES == null) return;
                        result = SES.SolutionOfES;
                        return result;
                    }
                    else
                    {
                        var SES = db.GetSES().SingleOrDefault(s => s.ID_SES == 5);
                        //if (SES == null) return;
                        result = SES.SolutionOfES;
                        return result;
                    } 
                }

                else
                {
                    result = "Соотношение ТИБА/ Тi Cl4 ниже оптимального. Дополнительные подробности смотреть в ручном выборе.";
                    return result;
                }

                //Соотношение ТИБА/ Тi Cl4 ниже оптимального (значение показателя вязкости по Муни вышло за допустимые диапазоны)

                //Соотношение ТИБА/ Тi Cl4 ниже оптимального (остальные параметры также вышли за пределы допустимого диапазона)

            }

            if (W > 0.0005)//Концентрация воды в шихте
            {
                result = "Концентрация воды в шихте. Смотреть возможные варианты в ручном выборе.";
                return result;
            }

            else
            {
                result = "Нештатных ситуаций не обнаружено, все показатели в норме.";
                return result;
            }
        }
        
    }
}
