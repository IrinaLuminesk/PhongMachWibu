using System;
using System.Collections.Generic;

namespace EnjuAihara.ViewModels.MasterData
{
    public class StreetCreateViewModel
    {
        public string StreetName { get; set; }
        public Guid CityId { get; set; }

        public List<Guid> DistrictId { get; set; }
    }
}
