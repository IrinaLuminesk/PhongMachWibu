using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.SelectList
{
    public class SelectBoolItem
    {
        public string name { get; set; }
        public bool id { get; set; }
    }

    public class SelectGuidItem
    {
        public Guid id { get; set; }

        public string name { get; set; }
    }

}
