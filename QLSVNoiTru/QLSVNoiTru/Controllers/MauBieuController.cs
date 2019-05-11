using QLSVNoiTru.Database;
using QLSVNoiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QLSVNoiTru.Controllers
{
    public class MauBieuController : BaseController
    {
        // GET: MauBieu
        public ActionResult DanhSachMauBieu()
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            List<MauBieu> mauBieus = db.MauBieux.ToList();
            ViewData["mauBieus"] = mauBieus;
            return View();
        }

        public ActionResult Chitiet(int maubieuId)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            MauBieu mauBieu = db.MauBieux.FirstOrDefault(x => x.MauBieuId == maubieuId);
            ViewData["mauBieu"] = mauBieu;
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CapNhat(MauBieu mauBieu)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            MauBieu mauBieuOld = db.MauBieux.FirstOrDefault(x => x.MauBieuId == mauBieu.MauBieuId);
            if (mauBieuOld != null)
            {
                mauBieuOld.TieuDe = mauBieu.TieuDe;
                mauBieuOld.NoiDung = mauBieu.NoiDung;
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachMauBieu");
        }

        public ActionResult InHopDongNoiTru(string maSinhVien)
        {
            var db = new DB();
            MauBieu mauBieu = db.MauBieux.FirstOrDefault(x => x.LoaiMauBieuId == (int)LoaiMauBieu.HOPDONGNOITRU);
            DateTime dateTime = DateTime.Now;
            SinhVien sinhVien = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == maSinhVien);
            Lop lop = db.Lops.FirstOrDefault(x => x.MaLop == sinhVien.MaLop);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{masinhvien}", sinhVien.MaSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tensinhvien}", sinhVien.TenSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{ngay}", dateTime.Day.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{thang}", dateTime.Month.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{nam}", dateTime.Year.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{gioitinh}", sinhVien.GioiTinh);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{sodienthoai}", sinhVien.SDT);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{lop}", lop.TenLop);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{khoa}", lop.Khoa.TenKhoa);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{quequan}", sinhVien.HoKhau);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{daidienbanqlktx}", GetLoginFullName());
            ViewData["NoiDung"] = mauBieu.NoiDung;
            return View();
        }

        public ActionResult InBienLaiThuTienCoc(string maSinhVien)
        {
            var db = new DB();
            MauBieu mauBieu = db.MauBieux.FirstOrDefault(x => x.LoaiMauBieuId == (int)LoaiMauBieu.BIENLAITHUTIENCOC);
            DateTime dateTime = DateTime.Now;
            SinhVien sinhVien = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == maSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{sophieu}", "BLTC" + sinhVien.MaSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tensinhvien}", sinhVien.TenSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{ngay}", dateTime.Day.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{thang}", dateTime.Month.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{nam}", dateTime.Year.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{phong}", sinhVien.Phong.SoHieuPhong);
            ViewData["NoiDung"] = mauBieu.NoiDung;
            return View();
        }

        public ActionResult InBienLaiThuPhong(string maSinhVien)
        {
            var db = new DB();
            MauBieu mauBieu = db.MauBieux.FirstOrDefault(x => x.LoaiMauBieuId == (int)LoaiMauBieu.BIENLAITHUPHONG);
            PhiPhong phiPhong = db.PhiPhongs.Where(x => x.MaSinhVien == maSinhVien).OrderByDescending(x => x.Thang).FirstOrDefault();
            DateTime dateTime = DateTime.Now;
            string TuThang = phiPhong == null ? "" : phiPhong.Thang.ToString("dd-MM");
            string DenThang = phiPhong == null ? "" : phiPhong.DenThang.ToString("dd-MM");
            string SoTien = phiPhong == null ? "0" : phiPhong.SoTien.ToString();
            string SoTienBangChu = TienHelper.Convert_NumtoText(SoTien);
            SinhVien sinhVien = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == maSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{sophieu}", "BLTP" + sinhVien.MaSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tensinhvien}", sinhVien.TenSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{ngay}", dateTime.Day.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{thang}", dateTime.Month.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{nam}", dateTime.Year.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{phong}", sinhVien.Phong.SoHieuPhong);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tuthang}", TuThang);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{denthang}", DenThang);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{sotien}", SoTien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{sotienbangchu}", SoTienBangChu);
            ViewData["NoiDung"] = mauBieu.NoiDung;
            return View();
        }

        public ActionResult InBienLaiThanhLyHopDong(string maSinhVien)
        {
            var db = new DB();
            MauBieu mauBieu = db.MauBieux.FirstOrDefault(x => x.LoaiMauBieuId == (int)LoaiMauBieu.BIENLAITHANHLYHOPDONG);
            DateTime dateTime = DateTime.Now;
            SinhVien sinhVien = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == maSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tensinhvien}", sinhVien.TenSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{ngay}", dateTime.Day.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{thang}", dateTime.Month.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{nam}", dateTime.Year.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{phong}", sinhVien.Phong.SoHieuPhong);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tang}", sinhVien.Phong.Tang.TenTang);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{daidienbanqlktx}", GetLoginFullName());
            ViewData["NoiDung"] = mauBieu.NoiDung;
            return View();
        }

        public ActionResult InDonXinRaNoiTru(string maSinhVien)
        {
            var db = new DB();
            MauBieu mauBieu = db.MauBieux.FirstOrDefault(x => x.LoaiMauBieuId == (int)LoaiMauBieu.DONXINRANOITRU);
            DateTime dateTime = DateTime.Now;
            SinhVien sinhVien = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == maSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tensinhvien}", sinhVien.TenSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{ngay}", dateTime.Day.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{thang}", dateTime.Month.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{nam}", dateTime.Year.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{phong}", sinhVien.Phong.SoHieuPhong);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tang}", sinhVien.Phong.Tang.TenTang);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{daidienbanqlktx}", GetLoginFullName());
            ViewData["NoiDung"] = mauBieu.NoiDung;
            return View();
        }

        public ActionResult InHoaDonDienNuoc(int hoadondiennuocId)
        {
            var db = new DB();
            MauBieu mauBieu = db.MauBieux.FirstOrDefault(x => x.LoaiMauBieuId == (int)LoaiMauBieu.PHIEUBAODIENNUOC);
            HoaDonDienNuoc hoaDonDienNuoc = db.HoaDonDienNuocs.FirstOrDefault(x => x.HoaDonDienNuocId == hoadondiennuocId);
            DateTime dateTime = DateTime.Now;
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{mahoadon}", hoadondiennuocId.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{ngay}", dateTime.Day.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{thang}", dateTime.Month.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{nam}", dateTime.Year.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{phong}", hoaDonDienNuoc.Phong.SoHieuPhong);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{thanghoadon}", hoaDonDienNuoc.ThangGhi.ToString("MM-yyyy"));
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{chisodiendau}", hoaDonDienNuoc.Chisodiendau.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{chisodiencuoi}", hoaDonDienNuoc.Chisodiencuoi.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tieuthu}", (hoaDonDienNuoc.Chisodiencuoi - hoaDonDienNuoc.Chisodiendau).ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{chisonuocdau}", hoaDonDienNuoc.Chisonuocdau.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{chisonuoccuoi}", hoaDonDienNuoc.Chisonuoccuoi.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tieuthunuoc}", (hoaDonDienNuoc.Chisonuoccuoi - hoaDonDienNuoc.Chisonuocdau).ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{dongiadien}", hoaDonDienNuoc.GiaDien.Dongia.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{dongianuoc}", hoaDonDienNuoc.GiaNuoc.Dongia.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{thanhtiendien}", ((hoaDonDienNuoc.Chisodiencuoi - hoaDonDienNuoc.Chisodiendau) * hoaDonDienNuoc.GiaDien.Dongia).ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{thanhtiennuoc}", ((hoaDonDienNuoc.Chisonuoccuoi - hoaDonDienNuoc.Chisonuocdau) * hoaDonDienNuoc.GiaNuoc.Dongia).ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tongtien}", hoaDonDienNuoc.TongTien.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tienbangchu}", TienHelper.Convert_NumtoText(hoaDonDienNuoc.TongTien.ToString()));
            ViewData["NoiDung"] = mauBieu.NoiDung;
            return View();
        }
    }
}
