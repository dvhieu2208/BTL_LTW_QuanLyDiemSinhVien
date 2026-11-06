using Microsoft.AspNetCore.Mvc;

namespace QuanLyDiemDaiHoc.Areas.Admin.ViewComponents
{
    public class AdminSidebarViewComponent:ViewComponent
    {
        public  IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
