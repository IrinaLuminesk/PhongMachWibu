using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.MasterData
{
    public class StreetCreateViewModel
    {
        public string StreetName { get; set; }
        public Guid CityId { get; set; }

        public List<Guid> DistrictId { get; set; }
    }
}
