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
        public string AccountName { get; set; }

        public string AccountCodeSearch { get; set; }

        public Guid? RoleNameSearch { get; set; }

        public bool Actived { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        //Các tham số trả về
        public int STT { get; set; }

        public string AccountCode { get; set; }

        public string UserName { get; set; }

        public List<string> RoleName { get; set; }

        public DateTime? CreateDate { get; set; }

        public string CreateDateString { get; set; }

        public string RealName { get; set; }

        public string CreateBy { get; set; }

        public string Status { get; set; }

        public Guid? AccountId { get; set; }
    }
}
