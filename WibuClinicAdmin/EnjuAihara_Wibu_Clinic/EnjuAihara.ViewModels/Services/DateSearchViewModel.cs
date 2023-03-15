using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.Services
{
    public class DateSearchViewModel
    {
        //Dùng để search PagingServerSide
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        public int STT { get; set; }

        public string DateCode { get; set; }

        public string Client { get; set; }

        public string Nurse { get; set; }

        public string Doctor { get; set; }

        public string DateCreateString { get; set; }

        public DateTime? DateCreate { get; set; }

        public string PayMentStatus { get; set; }

        public double? Money { get; set; }

        public Guid DateId { get; set; }
    }
}
