using Microsoft.AspNetCore.Mvc;

namespace QuanLyDiemDaiHoc.Areas.Student.ViewComponents
{
    public class StudentSidebarViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
