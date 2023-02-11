using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.Utilities.DateTimeFormat
{
    public class FormatDateTime
    {
        public static string FormatDateTimeWithString(DateTime? dateTime)
        {
            if(dateTime == null)
                return "";
            DateTime date = (DateTime)dateTime;
            return date.ToString("dd/MM/yyyy vào lúc hh:mm");
        }
    }
}
