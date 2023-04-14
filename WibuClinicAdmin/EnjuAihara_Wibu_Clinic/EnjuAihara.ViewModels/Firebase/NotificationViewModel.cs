using System;

namespace EnjuAihara.ViewModels.Firebase
{
    public class NotificationViewModel
    {
        public Guid? NotificationId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Link { get; set; }

        public DateTime? CreateDate { get; set; }


    }
}
