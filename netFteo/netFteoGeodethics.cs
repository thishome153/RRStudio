using System;

namespace netFteo.Spatial
{
    #region static Класс Geodethic - Функции Математики, Геодезии




    /// <summary>
    /// Класс Geodethic - функции Геодезии
    /// </summary>
    public static class Geodethic
    {
        /// <summary>
        /// Радиус Земли в километрах
        /// </summary>
        public const double A_E = 6371.0;				




        /// <summary>
        /// Перевод угла из формата  356.5623(RAW NiKon) в радианы 
        /// </summary>
        /// <param name="Угол">Угол в формате Nikon</param>
        /// <returns>Угол в радианах</returns>
        /// <remarks></remarks>
        public static double RawAngleToRadians(double Angle)

        {
            double Grad =0;
            double    rd20 =0;
            double DecimalGrad = 0;
            double min =0; double sec =0 ;
            Grad = Math.Truncate(Angle);
            rd20 = FracDouble(Angle) * 100; // сдвигаем точку вправо на 2 разраяд- в целой части минуты, в дробной части только секунды
            min = Math.Truncate(rd20) / 60; // минуты
            sec = FracDouble(rd20) / 36; //Секунды
            DecimalGrad = Grad + min + sec;
            return GradToRadians(DecimalGrad);
        }


        /// <summary>
        /// Перевод углов из радиан в градусы вещественного формата  356.5623 как в RAW NiKon
        /// </summary>
        /// <remarks> радианы -> градусы, 
        ///          #define Degrees(x) (x * 57.29577951308232)	// 
        ///          по следам http://gis-lab.info/qa/sphere-geodesic-direct-problem.html
        /// </remarks>
        /// <param name="Angle_In_Radians"></param>
        /// <returns></returns>
        public static double Degrees(double Angle_In_Radians)
        {
            double g ,Grad;
            Grad = Angle_In_Radians * 180 / Math.PI;

            g = Math.Truncate(Grad);
           
            return g; 
        }

        /// <summary>
        /// #define Radians(x) (x / 57.29577951308232)	// градусы -> радианы
        /// </summary>
        /// <remarks>по следам http://gis-lab.info/qa/sphere-geodesic-direct-problem.html
        /// </remarks>
        /// <param name="rd1">угол в градусы (формата  356.5623 как в RAW NiKon)</param>
        /// <returns></returns>
        public static double GradToRadians(double rd1) //Перевод в радианы
        {
            return (rd1 / 180) * Math.PI;
        }

        /// <summary>
        /// Дробная часть числа
        /// </summary>
        /// <param name="rd1"></param>
        /// <returns></returns>
        public static double Frac(float rd1)
        {
            double FracPart = rd1 - Math.Truncate(rd1);
            return FracPart;
        }
        /// <summary>
        /// //Дробная часть числа
        /// </summary>
        /// <param name="rd1"></param>
        /// <returns></returns>
        public static double FracDouble(double rd1)
        {
            double FracPart = rd1 - Math.Truncate(rd1);
            return FracPart;
        }

        /// <summary>
        /// Нормализация от 0 до 360 градусов (2*Pi)
        /// </summary>
        /// <param name="Angle">Угол</param>
        /// <returns>Тоже угол? но в пределах 0-360</returns>
        public static double AngleTo360(double Angle)
        {
            while (Angle > Math.PI * 2) Angle = Angle - Math.PI * 2;
            while (Angle < 0) Angle = Angle + Math.PI * 2;
            return Angle;
        }

        /// <summary>
        /// Перевод углов из радиан в символьную строку, формат grad° mm` ss``
        /// </summary>
        /// <param name="Angle_In_Radians">Угол в радианах</param>
        /// <returns>Строка с символами градусов минут и тп</returns>
        public static string RadiantoStr(double Angle_In_Radians)
        {
            double Grad = Angle_In_Radians * 180 / Math.PI;
            double g = Math.Truncate(Grad);
            double mm = Math.Truncate((Grad - g) * 60) / 100;
            double ss = ((Grad - g) * 60 - Math.Truncate((Grad - g) * 60)) * 0.0006;
            double res = g + mm + ss;
            if (res >= 0) res = res + 0.00000000001;
            else res = res - 0.00000000001;

            return Convert.ToString(g) + "° " + Convert.ToString(mm * 100) + "` " + Convert.ToString(Math.Round(ss * 100000,4)) + "''";
        }

        /// <summary>
        /// //Вычисление дирекционного угла (arcTangens)
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="Y2"></param>
        /// <returns></returns>
        public static double Atan(double xn1, double yn1, double xn2, double yn2)
        {
            double TanValue = 0;
            if (xn2 == xn1)
            {
                if (yn2 > yn1)
                    return Math.PI / 2;
                else
                    return 1.5 * Math.PI;
                if (xn2 > xn1)
                    return 0;
                else
                    return Math.PI;
            }

            TanValue = Math.Atan((yn2 - yn1) / (xn2 - xn1));
            if ((xn2 < xn1) & (yn2 < yn1)) TanValue = TanValue + Math.PI;
            if ((xn2 < xn1) & (yn2 > yn1)) TanValue = TanValue + Math.PI;
            AngleTo360(TanValue); // нормализуем угол
            return TanValue;
        }
        
        /// <summary>
        /// Древнее произведение вычисление горизонтального проложения (GPR...)
        /// </summary>
        /// <param name="xn1"></param>
        /// <param name="yn1"></param>
        /// <param name="xn2"></param>
        /// <param name="yn2"></param>
        /// <returns></returns>
        public static double lent(double xn1,double yn1,double xn2, double yn2)
        {
             return Math.Sqrt(Math.Pow(xn2-xn1,2)+Math.Pow(yn2-yn1,2));
        }

        /// <summary>
        /// 26.07.2011 расчет высоты пикета
        /// </summary>
        /// <param name="hi">высота инструмента</param>
        /// <param name="ht">высота цели</param>
        /// <param name="va">верт. угол</param>
        /// <param name="SlopeDist">Накл. расстояние</param>
        /// <returns></returns>
        public static double Elevation(double hi, double ht, double va, double SlopeDist)
        {
            return hi - ht + Math.Sin(va) * SlopeDist;
        }

        /// <summary>
        /// Разделить (разбить) прямую на точки
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="PointCount"></param>
        /// <returns></returns>
        public static PointList DivideLine(Point A, Point B, int PointCount)
        {
            PointList res = new PointList();
            double u = Atan(A.x,A.y,B.x,B.y); 
            double interval = lent(A.x,A.y,B.x,B.y)/( PointCount+1);

    if (interval > 0  )
      for (int n = 0; n <=  PointCount-1;n++)
       {
       res.AddPoint(A.NumGeopointA+"-"+B.NumGeopointA+"."+n.ToString(),
                       A.x +Math.Cos(u)*(interval*(n+1)),
                       A.y +Math.Sin(u)*(interval*(n+1)),
                       "Разбивка прямой на точки. Количество "+ PointCount.ToString());
                       
      }
 
 
            return null;
        }
        
        /// <summary>
        /// Проверка пересечения двух прямых a1a2 и b1b2
        /// </summary>
        /// <param name="ax1"></param>
        /// <param name="ay1"></param>
        /// <param name="ax2"></param>
        /// <param name="ay2"></param>
        /// <param name="bx1"></param>
        /// <param name="by1"></param>
        /// <param name="bx2"></param>
        /// <param name="by2"></param>
        /// <returns></returns>
       public static bool LinesIntersect(double ax1, double ay1, double ax2, double ay2, double bx1, double by1, double bx2, double by2)
        {
           double v1 = 0; double v2 = 0; double v3 = 0; double v4 = 0;
           v1 = (bx2-bx1)*(ay1-by1)-(by2-by1)*(ax1-bx1);
           v2 = (bx2-bx1)*(ay2-by1)-(by2-by1)*(ax2-bx1);
           v3 = (ax2-ax1)*(by1-ay1)-(ay2-ay1)*(bx1-ax1);
           v4 = (ax2-ax1)*(by2-ay1)-(ay2-ay1)*(bx2-ax1);

        return (v1*v2<0) & (v3*v4<0);
        }

       public static bool LinesIntersect(Point A1, Point A2, Point B1, Point B2)
       {
           return LinesIntersect(A1.x, A1.y, A2.x, A2.y, B1.x, B1.y, B2.x, B2.y);
       }

        /// <summary>
        /// Определение точки пересечения прямых a1a2 и b1b2
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
       public static Point FindIntersect(Point a1, Point a2, Point b1, Point b2)
       {
           double Denominator1 = 0; double k1; double k2;
           double Denominator2 = 0; double q1; double q2;
           double Denominator3 = 0;

           if (! LinesIntersect(a1,a2, b1, b2))
           {
               return null; //не пересекаются
           }

           //проверим, что она не параллельна оси и знаменатели не будут  равны 0
           if (a2.x - a1.x == 0) { Denominator1 = MathExt.eps; } else Denominator1 = a2.x - a1.x;
           if (b2.x - b1.x == 0) { Denominator2 = MathExt.eps; } else Denominator2 = b2.x - b1.x;
           k1 = (a2.y - a1.y) / (Denominator1); k2 = (b2.y - b1.y) / (Denominator2);
           if (k1 == 0 & k2 == 0) return null; //параллелльные
           q1 = a1.y - a1.x * k1; q2 = b1.y - b1.x * k2;
           //еще проверим, что деления на ноль не будет:
           if (k2 - k1 == 0) Denominator3 = MathExt.eps; else Denominator3 = k2 - k1;

           Point FP = new Point();
           //FP.id = Gen_id.newId;
           FP.NumGeopointA = "pt" + FP.id.ToString();
           FP.x = (q1 - q2) / (Denominator3);
           FP.y = (k2 * q1 - k1 * q2) / (Denominator3);
           FP.z = 0;
           FP.Status = 0;
           FP.Place = a1.NumGeopointA + a2.NumGeopointA + " x " + b1.NumGeopointA + b2.NumGeopointA;
           return FP;
       }

    }


}

namespace netFteo
{
    /// <summary>
    /// Класс Geodethic - функции Математики
    /// </summary>
    public static class MathExt
    {

        /// <summary>
        /// Очено малое число, но не Zero (Infinity, div by zero)
        /// </summary>
        /// 
        public const double eps = 0.0000000000000000000000000000000000000000000000001;

        /// <summary>
        /// Mod (modulus) divides two numbers and returns only the remainder (остаток)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Mod(double a, double b)
        {
            return a - b * Math.Round(a / b);
        }

        public static int ModInt(double a, double b)
        {
            return (int) Math.Round(Mod(a,b));
        }
    }
}
#endregion
