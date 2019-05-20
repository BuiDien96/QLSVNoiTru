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
            if (date == null)
                date = DateTime.Now.Date;
            else
                date = date.Value.Date;

            // Lay danh sach phong ktx và giá điện giá nước
            List<Phong> phongs = db.Phongs.Where(y => y.TrangThai != null && y.TrangThai.Value).OrderBy(x => x.TangId).ToList();
            int giadienId = db.GiaDiens.OrderByDescending(x => x.NgayCapNhat).FirstOrDefault().GiaDienId;
            int gianuocId = db.GiaNuocs.OrderByDescending(x => x.NgayCapNhat).FirstOrDefault().GiaNuocId;
            phongs.ForEach(x =>
            {
                // Lấy hóa đơn của tháng hiện tại
                HoaDonDienNuoc hoaDonDienNuoc = x.HoaDonDienNuocs.FirstOrDefault(y => y.ThangGhi.Year == date.Value.Year && y.ThangGhi.Month == date.Value.Month);
                // Lấy hóa đơn của tháng có hóa đơn gần tháng hiện tại nhất
                HoaDonDienNuoc hoaDonDienNuocThangGanNhat = x.HoaDonDienNuocs.Where(y=> DateTime.Compare(y.ThangGhi, (DateTime)date) < 0).OrderByDescending(y => y.ThangGhi).FirstOrDefault();
                if (hoaDonDienNuoc == null)
                {
                    // Trường hợp tháng hiện tại chưa có hóa đơn, tạo mới 
                    int chisodiendaumoi = 0;
                    int chisonuocdaumoi = 0;
                    string nguoinoptruoc = "";
                    if (hoaDonDienNuocThangGanNhat != null && hoaDonDienNuocThangGanNhat.TrangThai == 1)
                    {
                        // nếu có hóa đơn tháng gần nhất, lấy chỉ số cuối của điện nước tháng gần nhất để làm chỉ số đầu của tháng hiện tại,
                        // tương tự với chỉ số cuối của tháng hiện tại
                        chisodiendaumoi = hoaDonDienNuocThangGanNhat.Chisodiencuoi;
                        chisonuocdaumoi = hoaDonDienNuocThangGanNhat.Chisonuoccuoi;
                        nguoinoptruoc = hoaDonDienNuocThangGanNhat.NguoiNopTien;
                    }
                    hoaDonDienNuoc = new HoaDonDienNuoc()
                    {
                        SoHieuPhong = x.SoHieuPhong,
                        ThangGhi = date.Value,
                        Chisodiencuoi = chisodiendaumoi,
                        Chisodiendau = chisodiendaumoi,
                        Chisonuoccuoi = chisonuocdaumoi,
                        Chisonuocdau = chisonuocdaumoi,
                        GiaDienId = giadienId,
                        GiaNuocId = gianuocId,
                        GhiChu = "",
                        NguoiNopTien = nguoinoptruoc,
                        TrangThai = -1,
                        TongTien = 0
                    };
                } else
                {
                    // Trường hợp tháng hiện tại có hóa đơn
                    if (hoaDonDienNuocThangGanNhat != null)
                    {
                        // chỉ số đầu mới là chỉ số cuối của tháng gần nhất
                        // chỉ số cuối mới bằng tổng của chỉ số cuối tháng gần nhất và chênh lệch của chỉ số đầu và chỉ số cuối của tháng hiện tại 
                        hoaDonDienNuoc.Chisodiencuoi = hoaDonDienNuocThangGanNhat.Chisodiencuoi + hoaDonDienNuoc.Chisodiencuoi - hoaDonDienNuoc.Chisodiendau;
                        hoaDonDienNuoc.Chisodiendau = hoaDonDienNuocThangGanNhat.Chisodiencuoi;
                        hoaDonDienNuoc.Chisonuoccuoi = hoaDonDienNuocThangGanNhat.Chisonuoccuoi + hoaDonDienNuoc.Chisonuoccuoi - hoaDonDienNuoc.Chisonuocdau;
                        hoaDonDienNuoc.Chisonuocdau = hoaDonDienNuocThangGanNhat.Chisonuoccuoi;
                        db.SaveChanges();
                    }
                }
                hoaDonDienNuocs.Add(hoaDonDienNuoc);
            });
            ViewData["hoaDonDienNuocs"] = hoaDonDienNuocs;
            ViewBag.dateFilter = date.Value.ToString("MM/yyyy");
            return View();
        }

        [HttpPost]
        public ActionResult CapNhatHoaDon(List<HoaDonDienNuoc> hoaDonDienNuocs, string thangghi)
        {
            if (!CheckLogin(QuyenDangNhap.BPDienNuoc))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            DateTime dateTimeNow = DateTime.Now;
            hoaDonDienNuocs.ForEach(x =>
            {
                HoaDonDienNuoc hoaDonDienNuoc = db.HoaDonDienNuocs.FirstOrDefault(y => y.SoHieuPhong == x.SoHieuPhong && x.ThangGhi.Year == y.ThangGhi.Year && x.ThangGhi.Month == y.ThangGhi.Month);
                if (hoaDonDienNuoc is null)
                {
                    HoaDonDienNuoc hoaDonDienNuoc_temp = new HoaDonDienNuoc()
                    {
                        ThangGhi = x.ThangGhi,
                        Chisodiencuoi = (x.Chisodiencuoi < x.Chisodiendau) ? x.Chisodiendau : x.Chisodiencuoi,
                        Chisodiendau = x.Chisodiendau,
                        Chisonuoccuoi = (x.Chisonuoccuoi < x.Chisonuocdau) ? x.Chisonuocdau : x.Chisonuoccuoi,
                        Chisonuocdau = x.Chisonuocdau,
                        GhiChu = x.GhiChu,
                        GiaDienId = x.GiaDienId,
                        GiaNuocId = x.GiaNuocId,
                        NgayLap = dateTimeNow,
                        NguoiNopTien = x.NguoiNopTien,
                        SoHieuPhong = x.SoHieuPhong,
                        TrangThai = 0
                    };
                    hoaDonDienNuoc_temp.TongTien = db.GiaDiens.FirstOrDefault(y => y.GiaDienId == x.GiaDienId).Dongia * (hoaDonDienNuoc_temp.Chisodiencuoi - hoaDonDienNuoc_temp.Chisodiendau)
                                    + db.GiaNuocs.FirstOrDefault(y => y.GiaNuocId == x.GiaNuocId).Dongia * (hoaDonDienNuoc_temp.Chisonuoccuoi - hoaDonDienNuoc_temp.Chisonuocdau);
                    db.HoaDonDienNuocs.Add(hoaDonDienNuoc_temp);
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
            if (!CheckLogin(QuyenDangNhap.BPDienNuoc))
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
