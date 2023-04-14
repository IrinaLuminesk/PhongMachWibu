using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.CloudinaryHelper;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.Utilities.DateTimeFormat;
using EnjuAihara.Utilities.SelectListItemCustom;
using EnjuAihara.ViewModels.Datatable;
using EnjuAihara.ViewModels.MasterData;
using EnjuAihara.ViewModels.SelectList;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.MasterData.Controllers
{
    public class ArticleController : IrinaLumineskController
    {
        // GET: MasterData/Article
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public JsonResult _PaggingServerSide(DatatableViewModel model, ArticleSearchViewModel search, Guid? Author, string Title, string Summary, DateTime? FromDate, DateTime? ToDate, bool? Actived)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            if (ToDate != null)
                ToDate = ((DateTime)ToDate).AddDays(1).AddSeconds(-1);

            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.ArticleModels.
                Where(x => (x.AccountModel.AccountId == Author || (Author == null || Author == Guid.Empty))
                && (x.Title.Contains(Title) || string.IsNullOrEmpty(Title))
                && (x.Summary.Contains(Summary) || string.IsNullOrEmpty(Summary))
                && (x.Actived == Actived || Actived == null)
                && (x.CreateDate >= FromDate || FromDate == null)
                && (x.CreateDate <= ToDate || ToDate == null)
                )
                .Select(x =>
            new ArticleSearchViewModel
            {
                CreateBy = x.AccountModel.UsersModel.FirstName + " " + x.AccountModel.UsersModel.LastName,
                Status = x.Actived == true ? "Đang sử dụng" : "Đã ngưng",
                Summary = x.Summary,
                Title = x.Title,
                ArticleId = x.ArticleId,
                CreateDate = x.CreateDate,
                

            }).OrderBy(x => x.CreateDate).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<ArticleSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
            if (finalResult != null && finalResult.Count > 0)
            {
                int i = model.start;
                foreach (var item in finalResult)
                {
                    i++;
                    item.STT = i;
                    item.CreateDateString = FormatDateTime.FormatDateTimeWithString(item.CreateDate);
                }
            }
            return Json(new
            {
                draw = model.draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = finalResult
            });
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Create(string Title, string Summary, string Detail, HttpPostedFileBase Thumbnail)
        {
            try
            {
                JsonResult result = Validate(Title, Summary, Detail, Thumbnail, true);
                if (result != null)
                    return result;
                ArticleModel model = new ArticleModel()
                {
                    Actived = true,
                    CreateDate = DateTime.Now,
                    ArticleId = Guid.NewGuid(),
                    Author = CurrentUser.AccountId,
                    Summary = Summary,
                    Thumbnail = Thumbnail == null ? "" : CloudinaryUpload.Upload(Thumbnail),
                    Detail = Detail,
                    Title = Title
                };
                _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thêm thành công",
                    message = "Thêm bài viết thành công",
                    redirect = "/MasterData/Article"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Thêm thất bại",
                    message = "Đã có lỗi xảy ra " + ex.Message.ToString()
                });
            }
        }

        public ActionResult Edit(Guid Id)
        {
            var result = _context.ArticleModels.Where(x => x.ArticleId == Id).FirstOrDefault();
            return View(result);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Edit(string Title, string Summary, string Detail, HttpPostedFileBase Thumbnail, Guid ArticleId, bool Actived)
        {
            try
            {
                JsonResult result = Validate(Title, Summary, Detail, Thumbnail, false);
                if (result != null)
                    return result;
                var edit = _context.ArticleModels.Where(x => x.ArticleId == ArticleId).FirstOrDefault();
                edit.Title = Title;
                edit.Summary = Summary;
                edit.Detail = Detail;
                edit.Thumbnail = Thumbnail == null ? edit.Thumbnail : CloudinaryUpload.Upload(Thumbnail);
                edit.Author = CurrentUser.AccountId;
                edit.CreateDate = DateTime.Now;
                edit.Actived = Actived;
                _context.Entry(edit).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Sửa thành công",
                    message = "Sửa bài viết thành công",
                    redirect = "/MasterData/Article"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Thêm thất bại",
                    message = "Đã có lỗi xảy ra " + ex.Message.ToString()
                });
            }
        }
        public JsonResult Validate(string Title, string Summary, string Detail, HttpPostedFileBase Thumbnail, bool flag)
        {
            if (string.IsNullOrEmpty(Title))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Thêm thất bại",
                    message = "Vui lòng không để trống tiêu đề"
                });
            }
            if (string.IsNullOrEmpty(Summary))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Thêm thất bại",
                    message = "Vui lòng không để trống nội dung tóm tắt"
                });
            }
            if (string.IsNullOrEmpty(Detail))
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Thêm thất bại",
                    message = "Vui lòng không để trống chi tiết"
                });
            }
            if (Thumbnail != null)
            {
                if (CloudinaryUpload.CheckFileExtension(Path.GetExtension(Thumbnail.FileName)) == false)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng chọn ảnh đúng định dạng"
                    });
                }
            }
            else
            {
                if (flag == true)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng chọn ảnh thumbnail"
                    });
                }
            }
            return null;
        }

        public void CreateViewBag()
        {
            
            ViewBag.Actived = new SelectList(SelectListItemCustom.GetStatusSelectList(), "id", "name");
        }

        public JsonResult AutoComplete(string searchTerm)
        {
            var result = _context.AccountModels.Where(x =>
            (x.UsersModel.LastName.Contains(searchTerm) || x.UsersModel.FirstName.Contains(searchTerm)) && x.Actived == true && x.AccountInRoleModels.Any(y => y.RolesModel.RoleCode.Equals("SYSADMIN") || y.RolesModel.RoleCode.Equals("ADMIN"))
            ).Select(x =>
            new SelectListGuidForAutoComplete
            {
                value = x.AccountId,
                text = x.UsersModel.LastName +  " " + x.UsersModel.FirstName
            }).Take(20).ToList();

            result.Add(new SelectListGuidForAutoComplete { value = Guid.Empty, text = "-- Tất cả --" });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}