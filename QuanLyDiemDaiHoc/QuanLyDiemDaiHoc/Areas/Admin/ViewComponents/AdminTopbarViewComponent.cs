using Microsoft.AspNetCore.Mvc;

namespace QuanLyDiemDaiHoc.Areas.Admin.ViewComponents
{
    public class AdminTopbarViewComponent:ViewComponent
    {
        public  IViewComponentResult Invoke()
        {
            ViewBag.AdminName = HttpContext.Session.GetString("TenDangNhap") ?? "admin";
            return View();
        }
    }
}
