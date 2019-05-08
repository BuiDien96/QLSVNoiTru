using QLSVNoiTru.Database;
using QLSVNoiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QLSVNoiTru.Controllers
{
    public class HoaDonController : BaseController
    {
        // GET: HoaDon
        public ActionResult DanhSachHoaDonDienNuoc(DateTime? date = null)
        {
            if (!CheckLogin(QuyenDangNhap.BPDienNuoc))
                return Redirect("/Login/DangNhap");
            List<HoaDonDienNuoc> hoaDonDienNuocs = new List<HoaDonDienNuoc>();
            var db = new DB();
            if (date == null) date = DateTime.Now.Date;
            else
                date = date.Value.Date;

            DateTime thangtruoc = date.Value.AddMonths(-1);
            List<Phong> phongs = db.Phongs.OrderBy(x => x.TangId).ToList();
            int giadienId = db.GiaDiens.OrderByDescending(x => x.NgayCapNhat.Year == date.Value.Year && x.NgayCapNhat.Month == date.Value.Month).FirstOrDefault().GiaDienId;
            int gianuocId = db.GiaNuocs.OrderByDescending(x => x.NgayCapNhat.Year == date.Value.Year && x.NgayCapNhat.Month == date.Value.Month).FirstOrDefault().GiaNuocId;
            phongs.ForEach(x =>
            {
                HoaDonDienNuoc hoaDonDienNuoc = db.HoaDonDienNuocs.FirstOrDefault(y => y.ThangGhi == date && y.SoHieuPhong == x.SoHieuPhong);
                if (hoaDonDienNuoc == null)
                {
                    HoaDonDienNuoc hoaDonDienNuocThangTruoc = db.HoaDonDienNuocs.FirstOrDefault(y => y.ThangGhi == thangtruoc && y.SoHieuPhong == x.SoHieuPhong);
                    int chisodiendaumoi = 0;
                    int chisonuocdaumoi = 0;
                    if (hoaDonDienNuocThangTruoc != null && hoaDonDienNuocThangTruoc.TrangThai == 1)
                    {
                        chisodiendaumoi = hoaDonDienNuocThangTruoc.Chisodiencuoi;
                        chisonuocdaumoi = hoaDonDienNuocThangTruoc.Chisonuoccuoi;
                    }
                    hoaDonDienNuoc = new HoaDonDienNuoc()
                    {
                        SoHieuPhong = x.SoHieuPhong,
                        ThangGhi = date.Value,
                        Chisodiencuoi = 0,
                        Chisodiendau = chisodiendaumoi,
                        Chisonuoccuoi = 0,
                        Chisonuocdau = chisonuocdaumoi,
                        GiaDienId = giadienId,
                        GiaNuocId = gianuocId,
                        GhiChu = "",
                        NguoiNopTien = "",
                        TrangThai = -1,
                        TongTien = 0
                    };
                }
                hoaDonDienNuocs.Add(hoaDonDienNuoc);
            });
            ViewData["hoaDonDienNuocs"] = hoaDonDienNuocs;
            ViewBag.dateFilter = date.Value.ToString("yyyy-MM-dd");
            return View();
        }

        [HttpPost]
        public ActionResult CapNhatHoaDon(List<HoaDonDienNuoc> hoaDonDienNuocs, string thangghi)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            DateTime dateTimeNow = DateTime.Now;
            hoaDonDienNuocs.ForEach(x =>
            {
                HoaDonDienNuoc hoaDonDienNuoc = db.HoaDonDienNuocs.FirstOrDefault(y => y.SoHieuPhong == x.SoHieuPhong && x.ThangGhi == y.ThangGhi);
                if (hoaDonDienNuoc is null)
                {
                    db.HoaDonDienNuocs.Add(new HoaDonDienNuoc()
                    {
                        ThangGhi = x.ThangGhi,
                        Chisodiencuoi = x.Chisodiencuoi,
                        Chisodiendau = x.Chisodiendau,
                        Chisonuoccuoi = x.Chisonuoccuoi,
                        Chisonuocdau = x.Chisonuocdau,
                        GhiChu = x.GhiChu,
                        GiaDienId = x.GiaDienId,
                        GiaNuocId = x.GiaNuocId,
                        NgayLap = dateTimeNow,
                        NguoiNopTien = x.NguoiNopTien,
                        SoHieuPhong = x.SoHieuPhong,
                        TrangThai = 0,
                        TongTien = db.GiaDiens.FirstOrDefault(y => y.GiaDienId == x.GiaDienId).Dongia * (x.Chisodiencuoi - x.Chisodiendau)
                                    + db.GiaNuocs.FirstOrDefault(y => y.GiaNuocId == x.GiaNuocId).Dongia * (x.Chisonuoccuoi - x.Chisonuocdau)
                    });
                }
                else
                {
                    hoaDonDienNuoc.Chisonuocdau = x.Chisonuocdau;
                    hoaDonDienNuoc.Chisonuoccuoi = x.Chisonuoccuoi;
                    hoaDonDienNuoc.Chisodiendau = x.Chisodiendau;
                    hoaDonDienNuoc.Chisodiencuoi = x.Chisodiencuoi;
                    hoaDonDienNuoc.NguoiNopTien = x.NguoiNopTien;
                    hoaDonDienNuoc.TongTien = db.GiaDiens.FirstOrDefault(y => y.GiaDienId == x.GiaDienId).Dongia * (x.Chisodiencuoi - x.Chisodiendau)
                                    + db.GiaNuocs.FirstOrDefault(y => y.GiaNuocId == x.GiaNuocId).Dongia * (x.Chisonuoccuoi - x.Chisonuocdau);
                }
            });
            db.SaveChanges();
            return RedirectToAction("DanhSachHoaDonDienNuoc", new { date = thangghi });
        }

        public ActionResult ThanhToanHoaDon(int mahoadon, DateTime thangghi)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            HoaDonDienNuoc hoaDonDienNuoc = db.HoaDonDienNuocs.FirstOrDefault(x => x.HoaDonDienNuocId == mahoadon);
            if (hoaDonDienNuoc != null)
            {
                hoaDonDienNuoc.TrangThai = 1;
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachHoaDonDienNuoc", new { date = thangghi });
        }
    }
}