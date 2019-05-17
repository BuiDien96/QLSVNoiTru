using QLSVNoiTru.Database;
using QLSVNoiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLSVNoiTru.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(User user)
        {
            var db = new DB();
            string passCheck = DataHelper.MD5(user.Password);
            var userLogin = db.Users.FirstOrDefault(x => x.UserName == user.UserName && x.Password == passCheck && x.Quyen == user.Quyen && x.Status == true);
            if(userLogin != null)
            {
                Session["user"] = userLogin;
                Session["error"] = null;
                return Redirect("/");
            }
            else
            {
                Session["error"] = "Tài khoản hoặc mật khẩu không đúng";
            }
            return RedirectToAction("DangNhap");
        }
        public ActionResult DangXuat()
        {
            Session["user"] = null;
            Session["error"] = null;
            return RedirectToAction("DangNhap");
        }
    }
}