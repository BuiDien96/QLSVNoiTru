using QLSVNoiTru.Database;
using System.Web.Mvc;

namespace QLSVNoiTru.Controllers
{
    public class BaseController : Controller
    {
        public bool CheckLogin(int quyen)
        {
            return true;
            if (Session["user"] is null)
                return false;
            User user = (User)Session["user"];
            if (user.Quyen != quyen)
                return false;
            return true;
        }
        public bool CheckLogin()
        {
            return true;
            if (Session["user"] is null)
                return false;
            return true;
        }
    }
}