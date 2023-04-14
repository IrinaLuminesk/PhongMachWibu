using EnjuAihara.Core;
using EnjuAihara.EntityFramework;
using EnjuAihara.Utilities.Datatable;
using EnjuAihara.Utilities.SelectListItemCustom;
using EnjuAihara.ViewModels.Datatable;
using EnjuAihara.ViewModels.MasterData;
using System;
using System.Linq;
using System.Web.Mvc;

namespace EnjuAihara_Wibu_Clinic_Main.Areas.MasterData.Controllers
{
    public class MostAskQuestionsController : IrinaLumineskController
    {
        // GET: MasterData/MostAskQuestions
        public ActionResult Index()
        {
            ViewBag.Actived = new SelectList(SelectListItemCustom.GetStatusSelectList(), "id", "name");
            return View();
        }


        public JsonResult _PaggingServerSide(DatatableViewModel model, MostAskQuestionSearchViewModel search, string Title, string Content, bool? Actived)
        {
            int filteredResultsCount;
            int totalResultsCount = model.length;

            search.PageSize = model.length;
            search.PageNumber = model.start / model.length + 1;

            var query = _context.MostAskQuestionModels
                .Where(x => (x.Title.Contains(Title) || string.IsNullOrEmpty(Title)) && (x.Detail.Contains(Content) || string.IsNullOrEmpty(Content)) && ((x.Actived == Actived || Actived == null)))
                .Select(x =>
            new MostAskQuestionSearchViewModel
            {
                Title = x.Title,
                Content = x.Detail,
                Actived = x.Actived == true ? "Đang sử dụng" : "Ngừng sử dụng",
                OrderIndex = x.OrderIndex,
                MostAskQuestionId = x.MostAskQuestionId

            }).OrderBy(x => x.OrderIndex).ToList();
            var finalResult = PaggingServerSideDatatable.DatatableSearch<MostAskQuestionSearchViewModel>(model, out filteredResultsCount, out totalResultsCount, query.AsQueryable(), "STT");
            if (finalResult != null && finalResult.Count > 0)
            {
                int i = model.start;
                foreach (var item in finalResult)
                {
                    i++;
                    item.STT = i;
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


        [HttpPost]
        public JsonResult Create(string Title, string Content, int? OrderIndex)
        {
            try
            {
                if (string.IsNullOrEmpty(Title))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng nhập tiêu đề câu hỏi"
                    });
                }
                if (string.IsNullOrEmpty(Content))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng nhập nội dung câu hỏi"
                    });
                }
                if (OrderIndex == null)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng nhập thứ tự ưu tiên"
                    });
                }

                MostAskQuestionModel model = new MostAskQuestionModel()
                {
                    MostAskQuestionId = Guid.NewGuid(),
                    Title = Title,
                    OrderIndex = OrderIndex,
                    Actived = true,
                    CreateBy = CurrentUser.AccountId,
                    CreateDate = DateTime.Now,
                    Detail = Content
                };
                _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Thêm câu hỏi thành công",
                    redirect = "/MasterData/MostAskQuestions"
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = string.Format("Đã có lỗi xảy ra: {0}", ex.Message.ToString())
                });
            }
        }


        public ActionResult Edit(Guid Id)
        {
            var result = _context.MostAskQuestionModels.Where(x => x.MostAskQuestionId == Id).FirstOrDefault();
            return View(result);
        }

        [HttpPost]
        public JsonResult Edit(Guid MostAskQuestionId, string Title, string Detail, int? OrderIndex, bool? Actived)
        {
            try
            {
                if (string.IsNullOrEmpty(Title))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng không để trống tiêu đề"
                    });
                }
                if (Actived == null)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng không để trống trạng thái"
                    });
                }
                if (OrderIndex == null)
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng không để trống trạng thái"
                    });
                }
                if (string.IsNullOrEmpty(Detail))
                {
                    return Json(new
                    {
                        isSucess = false,
                        title = "Lỗi",
                        message = "Vui lòng không để trống câu hỏi"
                    });
                }
                var result = _context.MostAskQuestionModels.Where(x => x.MostAskQuestionId == MostAskQuestionId).FirstOrDefault();
                result.Title = Title;
                result.OrderIndex = OrderIndex;
                result.Detail = Detail;
                result.Actived = Actived;
                _context.Entry(result).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    isSucess = true,
                    title = "Thành công",
                    message = "Sửa thành câu hỏi thành công",
                    redirect = "/MasterData/MostAskQuestions"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSucess = false,
                    title = "Lỗi",
                    message = "Đã có lỗi xảy ra " + ex.Message.ToString()
                });
            }
        }

    }
}