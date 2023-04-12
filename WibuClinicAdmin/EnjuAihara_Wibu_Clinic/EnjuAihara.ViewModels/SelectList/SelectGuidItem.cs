using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.SelectList
{
    public class SelectGuidItem
    {
        public Guid id { get; set; }

        public string name { get; set; }
    }


    public class SelectGuidItemWithNull
    {
        public Guid? id { get; set; }

        public string name { get; set; }
    }
}
