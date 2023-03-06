using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.MasterData
{
    public class StreetSearchViewModel
    {
        //Dùng để search PagingServerSide
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        //Các tham số trả về
        public int STT { get; set; }

        public string StreetName { get; set; }

        public string InCity { get; set; }

        public string InDistrict { get; set; }

        public string Status { get; set; }

        public Guid? StreetId { get; set; }

    }
}
