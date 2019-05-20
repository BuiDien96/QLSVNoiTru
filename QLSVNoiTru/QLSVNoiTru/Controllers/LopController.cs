using QLSVNoiTru.Database;
using QLSVNoiTru.Models;
using System.Linq;
using System.Web.Mvc;

namespace QLSVNoiTru.Controllers
{
    public class LopController : BaseController
    {
        // GET: Lop
        public ActionResult DanhSachLop()
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            ViewData["khoas"] = db.Khoas.ToList();
            ViewData["lops"] = db.Lops.ToList();
            return View();
        }

        public JsonResult KiemTraTrung(string maLop)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Json("");
            var db = new DB();
            bool result = db.Lops.Any(x => x.MaLop == maLop);
            return Json(new
            {
                result = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ThemMoi(Lop lop)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            db.Lops.Add(lop);
            db.SaveChanges();
            return RedirectToAction("DanhSachLop");
        }

        public ActionResult Xoa(string maLop)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            Lop lop = db.Lops.FirstOrDefault(x => x.MaLop == maLop);
            if (lop != null)
            {
                db.Lops.Remove(lop);
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachLop");
        }

        public JsonResult ChiTiet(string maLop)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Json("");
            var db = new DB();
            Lop lop = db.Lops.FirstOrDefault(x => x.MaLop == maLop);
            return Json(new
            {
                lop.MaKhoa,
                lop.MaLop,
                lop.TenLop
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CapNhat(Lop lop)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            Lop lopCu = db.Lops.FirstOrDefault(x => x.MaLop == lop.MaLop);
            if (lopCu != null)
            {
                lopCu.TenLop = lop.TenLop;
                lopCu.MaKhoa = lop.MaKhoa;
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachLop");
        }
    }
}