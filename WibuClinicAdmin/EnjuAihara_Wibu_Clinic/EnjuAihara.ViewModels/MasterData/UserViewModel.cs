using System;
using System.Collections.Generic;
namespace EnjuAihara.ViewModels.MasterData
{
    public class UserViewModel
    {

        //Tìm kiếm

        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        public string UserCode { get; set; }

        public string UserName { get; set; }

        public DateTime? BirthdayFrom { get; set; }

        public DateTime? BirthdayTo { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public bool? Actived { get; set; }



        //Kết quả

        public int STT { get; set; }

        public Guid UserId { get; set; }

        public string UserCodeResult { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public DateTime? Birthday { get; set; }

        public string BirthdayString { get; set; }

        public string AddressResult { get; set; }

        public string EmailResult { get; set; }

        public string PhoneResult { get; set; }


        public string Status { get; set; }

        public List<string> AccountName { get; set; }

        public List<Guid> AccountId { get; set; }


    }
}
