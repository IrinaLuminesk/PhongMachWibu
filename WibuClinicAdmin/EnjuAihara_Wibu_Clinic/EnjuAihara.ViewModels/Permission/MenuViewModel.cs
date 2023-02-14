using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.Permission
{
    public class MenuViewModel
    {
        public string MenuName { get; set; }

        public string Icon { get; set; }

        public int? OrderIndex { get; set; }

        public List<PageViewModel> Pages { get; set; }
    }
}
