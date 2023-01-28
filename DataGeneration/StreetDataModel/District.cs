using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneration.StreetDataModel
{
    public class District
    {
        public string name { get; set; }
        public string pre { get; set; }
        public List<Ward> ward { get; set; }
        public List<string> street { get; set; }
    }
}
