﻿using EnjuAihara.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EnjuAihara.ViewModels.MasterData
{
    public class MedicineCreateViewModel
    {
        public HttpPostedFileBase Img { get; set; }
        public List<Guid> Ingredient { get; set; }
        public Guid Provider { get; set; }

    }

    //public class MedicineCreateDetailViewModel
    //{
    //    public List<HttpPostedFileBase> Img { get; set; }
    //    public List<Guid> Ingredient { get; set; }
    //    public Guid Provider { get; set; }
    //}
}
