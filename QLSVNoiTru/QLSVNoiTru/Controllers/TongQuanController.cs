using QLSVNoiTru.Database;
using QLSVNoiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QLSVNoiTru.Controllers
{
    public class TongQuanController : BaseController
    {
        // GET: TongQuan
        public ActionResult ManHinhTongQuan()
        {
            if (!CheckLogin())
                return Redirect("/Login/DangNhap");

            DateTime dateTimeNow = DateTime.Now;
            int nam = dateTimeNow.Year;
            int thang = dateTimeNow.Month;
            var db = new DB();
            List<SinhVien> sinhViens = db.SinhViens.Where(x => x.TrangThaiO != (int)TrangThaiO.CheckOut).ToList();
            ViewBag.svMoi = db.SinhViens.Where(x => x.NgayNhanPhong.Value.Year == nam && x.NgayNhanPhong.Value.Month == thang).Count();
            ViewBag.tongSV = sinhViens.Count();
            ViewBag.tongSoPhong = db.Phongs.Where(y => y.TrangThai != null && y.TrangThai.Value).Count();
            float tienDien = 0;
            float tienNuoc = 0;
            if(db.HoaDonDienNuocs.FirstOrDefault(x => x.ThangGhi.Year == nam && x.ThangGhi.Month == thang && x.TrangThai != -1)!= null)
            {
                tienDien = (float)db.HoaDonDienNuocs.Where(x => x.ThangGhi.Year == nam && x.ThangGhi.Month == thang && x.TrangThai != -1)
                .Sum(x => (x.Chisodiencuoi - x.Chisodiendau) * x.GiaDien.Dongia);
            }
            if(db.HoaDonDienNuocs.FirstOrDefault(x => x.ThangGhi.Year == nam && x.ThangGhi.Month == thang && x.TrangThai != -1)!= null)
            {
                tienNuoc = (float)db.HoaDonDienNuocs.Where(x => x.ThangGhi.Year == nam && x.ThangGhi.Month == thang && x.TrangThai != -1)
                .Sum(x => (x.Chisonuoccuoi - x.Chisonuocdau) * x.GiaNuoc.Dongia);
            }
            List<HoaDonDienNuoc> hoaDonDienNuocs = db.HoaDonDienNuocs.Where(x => x.ThangGhi.Year == nam && x.ThangGhi.Month == thang && x.TrangThai != -1)
                .OrderByDescending(x => x.TongTien)
                .Skip(0)
                .Take(6)
                .ToList();
            ViewBag.tienDien = tienDien;
            ViewBag.tienNuoc = tienNuoc;
            ViewData["hoaDonDienNuocs"] = hoaDonDienNuocs;
            ViewData["sinhViens"] = sinhViens;
            return View();
        }
    }
}
