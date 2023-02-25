using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.MasterData
{
    public class MedicineSearchViewModel
    {
        //Dùng để search PagingServerSide
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }


        //Các tham số search
        public string MedicineNameSearch { get; set; }

        public string MedicineCodeSearch { get; set; }
        public string IngredientNameSearch { get; set; }
        public string ProviderNameSearch { get; set; }
        
        public bool Actived { get; set; }

        //Các tham số trả về
        public int STT { get; set; }

        public string MedicineName { get; set; }

        public string Unit { get; set; }

        public List<string> IngredientName { get; set; }
        public string ProviderName { get; set; }

        public decimal? MedicineOnHandQuantity { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? Expiry { get; set; }

        public string ExpiryString { get; set; }
        public Guid? MedicineId { get; set; }
        public string Status { get; set; }
        public string Status2 { get; set; }

        public Guid? MapId { get; set; }
    }
}
