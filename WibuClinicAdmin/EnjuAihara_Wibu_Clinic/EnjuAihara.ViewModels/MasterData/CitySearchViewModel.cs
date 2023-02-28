using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.MasterData
{
    public class CitySearchViewModel
    {
        //Dùng để search PagingServerSide
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        //Các tham số trả về
        public int STT { get; set; }

        public string CityName { get; set; }

        public string Status { get; set; }

        public Guid? CityId { get; set; }
    }
}
