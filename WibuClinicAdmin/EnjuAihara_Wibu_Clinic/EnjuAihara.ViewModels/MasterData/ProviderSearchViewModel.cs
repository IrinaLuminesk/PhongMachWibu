using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.MasterData
{
    public class ProviderSearchViewModel
    {
        //Phân trang
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        //Thông số search
        public string ProviderNameSearch { get; set; }
        public string ProviderCodeSearch { get; set; }
        public string AddressSearch { get; set; }
        public bool Actived { get; set; }

        public int STT { get; set; }
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string ProviderCode { get; set; }
        public string Address {get; set; }
        public string Status { get; set; }
        public float Latidude { get; set; }
        public float Longitude { get; set; }

    }
}
