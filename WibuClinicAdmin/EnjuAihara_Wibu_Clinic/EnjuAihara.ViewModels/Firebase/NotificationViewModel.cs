﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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