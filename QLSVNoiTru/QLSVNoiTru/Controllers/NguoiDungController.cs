using QLSVNoiTru.Database;
using QLSVNoiTru.Models;
using System.Linq;
using System.Web.Mvc;

namespace QLSVNoiTru.Controllers
{
    public class NguoiDungController : BaseController
    {
        // GET: NguoiDung
        public ActionResult DanhSachTaiKhoan()
        {
            if (!CheckLogin(QuyenDangNhap.BPHeThong))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            ViewData["users"] = db.Users.ToList();
            return View();
        }

        public JsonResult KiemTraTrung(string username)
        {
            if (!CheckLogin(QuyenDangNhap.BPHeThong))
                return Json("");
            var db = new DB();
            bool result = db.Users.Any(x => x.UserName == username);
            return Json(new
            {
                result = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ThemMoi(User user)
        {
            if (!CheckLogin(QuyenDangNhap.BPHeThong))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            user.Password = DataHelper.MD5(user.Password);
            db.Users.Add(user);
            db.SaveChanges();
            return RedirectToAction("DanhSachTaiKhoan");
        }


        public JsonResult ChiTiet(string username)
        {
            if (!CheckLogin(QuyenDangNhap.BPHeThong))
                return Json("");
            var db = new DB();
            User user = db.Users.FirstOrDefault(x => x.UserName == username);
            return Json(new
            {
                user.UserName,
                user.Quyen,
                user.Status,
                user.FullName,
                user.Email
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CapNhat(User user)
        {
            if (!CheckLogin(QuyenDangNhap.BPHeThong))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            User userCu = db.Users.FirstOrDefault(x => x.UserName == user.UserName);
            if (userCu != null)
            {
                userCu.FullName = user.FullName;
                userCu.Status = user.Status;
                userCu.Email = user.Email;
                userCu.Quyen = user.Quyen;
                if(!string.IsNullOrEmpty(user.Password))
                    userCu.Password = DataHelper.MD5(user.Password);
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachTaiKhoan");
        }

        public ActionResult Thongtintaikhoan()
        {
            //if (!CheckLogin(QuyenDangNhap.BPQuanLy))
            //    return Redirect("/Login/DangNhap");
            User user = (User)Session["user"];
            ViewData["user"] = user;
            return View();
        }
        [HttpPost]
        public ActionResult Capnhatthongtincanhan(User user)
        {
            if (!CheckLogin(QuyenDangNhap.BPHeThong))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            User userCu = db.Users.FirstOrDefault(x => x.UserName == user.UserName);
            if (userCu != null)
            {
                userCu.FullName = user.FullName;
                userCu.Status = user.Status;
                userCu.Email = user.Email;
                if (!string.IsNullOrEmpty(user.Password))
                    userCu.Password = user.Password;
                db.SaveChanges();
            }
            return RedirectToAction("Thongtintaikhoan");
        }
    }
}