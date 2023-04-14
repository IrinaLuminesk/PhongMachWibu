using EnjuAihara.ViewModels.SelectList;
using System.Collections.Generic;

namespace EnjuAihara.Utilities.SelectListItemCustom
{
    public class SelectListItemCustom
    {
        public static List<SelectBoolItem> GetStatusSelectList()
        {
            List<SelectBoolItem> StatusList = new List<SelectBoolItem>()
            {
                new SelectBoolItem() { id = true, name = "Đang sử dụng"},
                new SelectBoolItem() { id = false, name = "Đã ngừng sử dụng"}
            };
            return StatusList;
        }
    }
}
