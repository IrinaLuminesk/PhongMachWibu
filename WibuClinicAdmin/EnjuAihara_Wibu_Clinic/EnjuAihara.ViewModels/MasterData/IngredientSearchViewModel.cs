using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.MasterData
{
    public class IngredientSearchViewModel
    {
        //Phân trang
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        //Thông số search
        public string IngredientNameSearch { get; set; }
        public string IngredientCodeSearch { get; set; }
        public bool Actived { get; set; }

        public int STT { get; set; }
        public Guid IngredientId { get; set; }
        public string IngredientName { get; set; }
        public string IngredientCode { get; set; }

        public string Status { get; set; }
    }
}
