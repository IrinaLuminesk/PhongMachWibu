using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.Personal
{
    public class CalendarEventViewModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public DateTime startDateTime { get; set; }

        public string start { get; set; }
    }
}
