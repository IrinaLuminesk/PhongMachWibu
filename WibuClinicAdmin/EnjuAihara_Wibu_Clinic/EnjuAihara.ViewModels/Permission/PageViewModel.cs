using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.Permission
{
    public class PageViewModel
    {
        public string PageUrl { get; set; }

        public string Icon { get; set; }

        public string PageName { get; set; }

        public int? OrderIndex { get; set; }

        public bool? Actived { get; set; }
    }
}
