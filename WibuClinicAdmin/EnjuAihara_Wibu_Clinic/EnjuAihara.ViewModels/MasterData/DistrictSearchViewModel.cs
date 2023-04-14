using System;

namespace EnjuAihara.ViewModels.MasterData
{
    public class DistrictSearchViewModel
    {
        //Dùng để search PagingServerSide
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        //Các tham số trả về
        public int STT { get; set; }

        public string DistrictName { get; set; }

        public string Status { get; set; }

        public string InCity { get; set; }

        public int TotalRoad { get; set; }

        public Guid? DistrictId { get; set; }
    }
}
