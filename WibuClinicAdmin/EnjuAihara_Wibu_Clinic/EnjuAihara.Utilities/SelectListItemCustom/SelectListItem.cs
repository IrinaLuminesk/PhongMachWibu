using EnjuAihara.ViewModels.SelectList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.Utilities.SelectListItemCustom
{
    public class SelectListItemCustom
    {
        public static List<SelectBoolItem> GetStatusSelectList()
        {
            List<SelectBoolItem> StatusList = new List<SelectBoolItem>()
            {
                new SelectBoolItem() { id = true, name = "Đang sử dụng"},
                new SelectBoolItem() { id = false, name = "Ngừng sử dụng"}
            };
            return StatusList;
        }
    }
}
