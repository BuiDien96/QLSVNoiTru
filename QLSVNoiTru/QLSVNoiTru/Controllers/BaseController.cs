using QLSVNoiTru.Database;
using System.Web.Mvc;

namespace QLSVNoiTru.Controllers
{
    public class BaseController : Controller
    {
        public bool CheckLogin(int quyen)
        {
            if (Session["user"] is null)
                return false;
            User user = (User)Session["user"];
            if (user.Quyen != quyen)
                return false;
            return true;
        }
        public bool CheckLogin()
        {
            if (Session["user"] is null)
                return false;
            return true;
        }

        public string GetLoginFullName()
        {
            if (Session["user"] is null)
            {
                return null;
            }
            return ((User)Session["user"]).FullName;
        }
    }
}