using System;
using System.Text;

namespace CommonLib
{
    /// <summary>
    /// Временной период
    /// </summary>
    public struct TimePeriod :
        IEquatable<TimePeriod>,
        IComparable<TimePeriod>,
        ICloneable
    {
        /// <summary>
        /// Количество часов
        /// </summary>
        public int Hours;

        /// <summary>
        /// Количество дней
        /// </summary>
        public int Days;

        /// <summary>
        /// Количество недель
        /// </summary>
        public int Weeks;

        /// <summary>
        /// Количество месяцев
        /// </summary>
        public int Months;

        /// <summary>
        /// Количество лет
        /// </summary>
        public int Years;


        /// <summary>
        /// Сравнивает с объектом
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is TimePeriod))
                return false;

            return Equals((TimePeriod)obj);
        }

        /// <summary>
        /// Получает хэш-код
        /// </summary>
        public override int GetHashCode()
        {
            return GetTotalHours();
        }

        /// <summary>
        /// Конвертирование в строку
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Years != 0)
            {
                sb.Append(Years);
                sb.Append(" ");
                sb.Append(CommStr.ABOUT);
            }

            if (Months != 0)
            {
                if (sb.Length > 0)
                    sb.Append(" ");

                sb.Append(Months);
                sb.Append(" ");
                sb.Append(CommStr.ABOUT);
            }

            if (Weeks != 0)
            {
                if (sb.Length > 0)
                    sb.Append(" ");

                sb.Append(Weeks);
                sb.Append(" ");
                sb.Append(CommStr.ABOUT);
            }

            if (Days != 0)
            {
                if (sb.Length > 0)
                    sb.Append(" ");

                sb.Append(Days);
                sb.Append(" ");
                sb.Append(CommStr.ABOUT);
            }

            if (Hours != 0)
            {
                if (sb.Length > 0)
                    sb.Append(" ");

                sb.Append(Hours);
                sb.Append(" ");
                sb.Append(CommStr.ABOUT);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Сравнивает с другим временным периодом
        /// </summary>
        public bool Equals(TimePeriod other)
        {
            return other.GetTotalHours() == GetTotalHours();
        }

        /// <summary>
        /// Копирование для интерфейса ICloneable
        /// </summary>
        public object Clone()
        {
            return new TimePeriod
            {
                Years = Years,
                Months = Months,
                Weeks = Weeks,
                Days = Days,
                Hours = Hours,
            };
        }

        /// <summary>
        /// Численно сравнивает с другим временным периодом
        /// </summary>
        public int CompareTo(TimePeriod other)
        {
            return GetTotalHours() - other.GetTotalHours();
        }

        /// <summary>
        /// Проверяет на равенство
        /// </summary>
        public static bool operator ==(TimePeriod left, TimePeriod right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Проверяет на неравенство
        /// </summary>
        public static bool operator !=(TimePeriod left, TimePeriod right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Проверяет меньше ли первое значение
        /// </summary>
        public static bool operator <(TimePeriod left, TimePeriod right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Проверяет больше ли первое значение
        /// </summary>
        public static bool operator >(TimePeriod left, TimePeriod right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Проверяет меньше или равно ли первое значение
        /// </summary>
        public static bool operator <=(TimePeriod left, TimePeriod right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Проверяет больше или равно ли первое значение
        /// </summary>
        public static bool operator >=(TimePeriod left, TimePeriod right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Получает приведенное по модулю значение
        /// </summary>
        public static TimePeriod Abs(TimePeriod period)
        {
            period.Years = Math.Abs(period.Years);
            period.Months = Math.Abs(period.Months);
            period.Weeks = Math.Abs(period.Weeks);
            period.Days = Math.Abs(period.Days);
            period.Hours = Math.Abs(period.Hours);

            return period;
        }

        /// <summary>
        /// Вычитает из даты период
        /// </summary>
        public static DateTime Substract(DateTime dateTime, TimePeriod period)
        {
            dateTime = dateTime.AddYears(-period.Years);
            dateTime = dateTime.AddMonths(-period.Months);
            dateTime = dateTime.AddDays(-period.Weeks * 7);
            dateTime = dateTime.AddDays(-period.Days);
            dateTime = dateTime.AddHours(-period.Hours);
            return dateTime;
        }

        /// <summary>
        /// Добавляет период к дате
        /// </summary>
        public static DateTime Add(DateTime dateTime, TimePeriod period)
        {
            dateTime = dateTime.AddYears(period.Years);
            dateTime = dateTime.AddMonths(period.Months);
            dateTime = dateTime.AddDays(period.Weeks * 7);
            dateTime = dateTime.AddDays(period.Days);
            dateTime = dateTime.AddHours(period.Hours);
            return dateTime;
        }

        /// <summary>
        /// Получает разницу между датами с учетом временного шага
        /// </summary>
        public static TimeSpan Modulo(DateTime start, DateTime end, TimePeriod period)
        {
            if (start > end)
                return start - end;

            while (Add(start, period) < end)
                start = Add(start, period);

            return end - start;
        }


        /// <summary>
        /// Поучает полное количество часов в периоде
        /// </summary>
        private int GetTotalHours()
        {
            return
                (Years * 12 * 30 +
                 Months * 30 +
                 Weeks * 7 +
                 Days) * 24 +
                 Hours;
        }
    }
}
