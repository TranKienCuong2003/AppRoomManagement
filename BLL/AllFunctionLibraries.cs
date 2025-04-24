using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AllFunctionLibraries
    {
        public static readonly string[] units = { "", "mười", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        public static readonly string[] bigUnits = { "", "nghìn", "triệu", "tỷ" };

        public AllFunctionLibraries()
        {

        }

        public string ConvertNumberToWords(decimal number)
        {
            if (number == 0)
                return "Không đồng";

            StringBuilder result = new StringBuilder();
            int unitIndex = 0;

            while (number > 0)
            {
                int part = (int)(number % 1000);
                if (part > 0)
                {
                    string partWords = ConvertPartToWords(part);
                    result.Insert(0, partWords + " " + bigUnits[unitIndex] + " ");
                }
                number /= 1000;
                unitIndex++;
            }

            return result.ToString().Trim() + " đồng";
        }

        private string ConvertPartToWords(int part)
        {
            StringBuilder partResult = new StringBuilder();

            int hundreds = part / 100;
            int tens = (part % 100) / 10;
            int ones = part % 10;

            if (hundreds > 0)
            {
                partResult.Append(units[hundreds] + " trăm ");
            }

            if (tens > 0)
            {
                if (tens == 1 && hundreds > 0)
                    partResult.Append("mười ");
                else
                    partResult.Append(units[tens] + " mươi ");
            }
            else if (tens == 0 && hundreds > 0)
            {
                partResult.Append("lẻ ");
            }

            if (ones > 0)
            {
                if (tens > 0)
                    partResult.Append(units[ones]);
                else if (tens == 0 && ones == 5)
                    partResult.Append("năm");
                else
                    partResult.Append(units[ones]);
            }

            return partResult.ToString().Trim();
        }

        public string CalculateMonthsAndDays(DateTime date1, DateTime date2)
        {
            if (date1 > date2)
            {
                var temp = date1;
                date1 = date2;
                date2 = temp;
            }

            int months = (date2.Year - date1.Year) * 12 + date2.Month - date1.Month;
            if (date2.Day < date1.Day)
            {
                months--;
            }

            int days = (date2 - date1.AddMonths(months)).Days;

            return $"{months} tháng {days} ngày";
        }

    }
}
