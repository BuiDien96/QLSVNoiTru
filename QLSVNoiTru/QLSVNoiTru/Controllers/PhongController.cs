using QLSVNoiTru.Database;
using QLSVNoiTru.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QLSVNoiTru.Controllers
{
    public class PhongController : BaseController
    {
        // GET: Phong
        public ActionResult DanhSachPhong()
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            ViewData["loaiphongs"] = db.LoaiPhongs.ToList();
            ViewData["phongs"] = db.Phongs.ToList();
            ViewData["tangs"] = db.Tangs.ToList();
            return View();
        }
        public JsonResult KiemTraTrung(string soHieuPhong)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Json("");
            var db = new DB();
            bool result = db.Phongs.Any(x => x.SoHieuPhong == soHieuPhong);
            return Json(new
            {
                result = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ThemMoi(Phong phong)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            db.Phongs.Add(phong);
            db.SaveChanges();
            return RedirectToAction("DanhSachPhong");
        }

        public ActionResult Xoa(string soHieuPhong)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            Phong phong = db.Phongs.FirstOrDefault(x => x.SoHieuPhong == soHieuPhong);
            if (phong != null)
            {
                phong.TrangThai = false;
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachPhong");
        }

        public JsonResult ChiTiet(string soHieuPhong)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Json("");
            var db = new DB();
            Phong phong = db.Phongs.FirstOrDefault(x => x.SoHieuPhong == soHieuPhong);
            return Json(new
            {
                phong.MaLoaiPhong,
                phong.SoHieuPhong,
                phong.TangId,
                phong.SucChuaToiDa,
                SvDaO = db.SinhViens.Where(x => x.SoHieuPhong == phong.SoHieuPhong && x.TrangThaiO == (int)TrangThaiO.DangO).Count(),
                TrangThai = phong.TrangThai ?? false
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CapNhat(Phong phong)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            Phong phongCu = db.Phongs.FirstOrDefault(x => x.SoHieuPhong == phong.SoHieuPhong);
            bool phongConSinhVien = db.SinhViens.Where(x => x.SoHieuPhong == phongCu.SoHieuPhong && x.TrangThaiO == (int)TrangThaiO.DangO).Count() > 0;
            if (phongCu != null && !phongConSinhVien)
            {
                phongCu.MaLoaiPhong = phong.MaLoaiPhong;
                phongCu.TangId = phong.TangId;
                phongCu.SucChuaToiDa = phong.SucChuaToiDa;
                phongCu.TrangThai = phong.TrangThai;
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachPhong");
        }

        public ActionResult PhongTrangThietBi()
        {
            var db = new DB();
            ViewData["loaiphongs"] = db.LoaiPhongs.ToList();
            ViewData["phongs"] = db.Phongs.Where(y => y.TrangThai != null && y.TrangThai.Value).ToList();
            return View();
        }

        public ActionResult ChiTietTrangThietBiPhong(string soHieuPhong)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            Phong phong = db.Phongs.Where(y => y.TrangThai != null && y.TrangThai.Value).FirstOrDefault(x => x.SoHieuPhong == soHieuPhong);
            List<ThietBi> thietBis = db.ThietBis.ToList();
            List<PhongThietBi> phongThietBis = db.PhongThietBis.Where(x => x.SoHieuPhong == soHieuPhong).ToList();
            List<EThietBi> ethietBis = new List<EThietBi>();
            thietBis.ForEach(x =>
            {
                bool chon = phongThietBis.Any(y => y.MaThietBi == x.MaThietBi);
                ethietBis.Add(new EThietBi()
                {
                    Chon = chon,
                    Gia = x.Gia,
                    HinhAnh = x.HinhAnh,
                    MaThietBi = x.MaThietBi,
                    TenThietBi = x.TenThietBi
                });
            });
            ViewData["ethietBis"] = ethietBis;
            ViewData["phong"] = phong;
            return View();
        }

        [HttpPost]
        public ActionResult CapNhatTrangThietBiPhong(string soHieuPhong, List<EThietBi> ethietBis)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            var transaction = db.Database.BeginTransaction();
            db.PhongThietBis.RemoveRange(db.PhongThietBis.Where(x => x.SoHieuPhong == soHieuPhong));
            ethietBis.ForEach(x =>
            {
                if (x.Chon)
                {
                    db.PhongThietBis.Add(new PhongThietBi()
                    {
                        MaThietBi = x.MaThietBi,
                        SoHieuPhong = soHieuPhong
                    });
                }
            });
            db.SaveChanges();
            transaction.Commit();
            return RedirectToAction("PhongTrangThietBi");
        }

        public ActionResult DSSinhVienO(string soHieuPhong)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            Phong phong = db.Phongs.FirstOrDefault(x => x.SoHieuPhong == soHieuPhong);
            if (phong.TrangThai == false)
                return Redirect("/Phong/DanhSachPhong");
            ViewData["sinhViens"] = db.SinhViens
                    .Where(x => x.TrangThaiO == (int)TrangThaiO.DangO && x.SoHieuPhong == soHieuPhong)
                    .OrderByDescending(x => x.NgayNhanPhong).ToList();
            ViewData["phong"] = db.Phongs.FirstOrDefault(x => x.SoHieuPhong == soHieuPhong);
            return View();
        }

        public ActionResult Diennuoctieuthu(string soHieuPhong)
        {
            var db = new DB();
            List<HoaDonDienNuoc> hoaDonDienNuocs = db.HoaDonDienNuocs.Where(x => x.SoHieuPhong == soHieuPhong).OrderByDescending(x => x.ThangGhi).ToList();
            Phong phong = db.Phongs.FirstOrDefault(x => x.SoHieuPhong == soHieuPhong);
            ViewData["phong"] = phong;
            ViewData["hoaDonDienNuocs"] = hoaDonDienNuocs;
            return View();
        }
    }
}