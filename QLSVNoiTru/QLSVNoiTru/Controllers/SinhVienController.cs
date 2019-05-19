using Newtonsoft.Json;
using QLSVNoiTru.Database;
using QLSVNoiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QLSVNoiTru.Controllers
{
    enum LoaiMauBieu
    {
        DONXINVAONOITRU = 1,
        HOPDONGNOITRU = 2,
        DONXINRANOITRU = 3,
        BIENLAITHANHLYHOPDONG = 4,
        PHIEUBAODIENNUOC = 5,
        BIENLAITHUTIENCOC = 6,
        BIENLAITHUPHONG = 7

    }
    public class SinhVienController : BaseController
    {
        // GET: SinhVien
        public ActionResult DanhSachSinhVien()
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            ViewData["sinhViens"] = db.SinhViens.OrderByDescending(x => x.NgayNhanPhong).ToList();
            return View();
        }

        public ActionResult DangKyNoiTru(string gioitinh = "Nam")
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            ViewData["lops"] = db.Lops.ToList();
            List<Tang> tangs = db.Tangs.OrderByDescending(x => x.TangId).ToList();
            if (tangs == null)
                tangs = new List<Tang>();
            List<ETang> eTangs = new List<ETang>();
            tangs.ForEach(x =>
            {
                ETang eTang = new ETang()
                {
                    TangId = x.TangId,
                    TenTang = x.TenTang,
                    Phongs = new List<EPhong>()
                };
                x.Phongs.Where(y => y.TrangThai != null && y.TrangThai.Value).ToList().ForEach(y =>
                  {
                      if ((gioitinh == "Nam" && (y.LoaiPhong.MucDich == (int)MucDichLoaiPhong.ChiDanhCHoNam || y.LoaiPhong.MucDich == (int)MucDichLoaiPhong.CaNamNu))
                      || (gioitinh == "Nữ" && (y.LoaiPhong.MucDich == (int)MucDichLoaiPhong.ChiDanhCHoNu || y.LoaiPhong.MucDich == (int)MucDichLoaiPhong.CaNamNu)))
                      {
                          int svDaO = db.SinhViens.Where(z => z.SoHieuPhong == y.SoHieuPhong && z.TrangThaiO == (int)TrangThaiO.DangO).Count();
                          eTang.Phongs.Add(new EPhong()
                          {
                              LoaiPhong = y.LoaiPhong,
                              MaLoaiPhong = y.MaLoaiPhong,
                              SoHieuPhong = y.SoHieuPhong,
                              SoPhongDaO = svDaO,
                              SucChuaToiDa = y.SucChuaToiDa,
                              TangId = y.TangId
                          });
                      }
                  });
                eTangs.Add(eTang);
            });
            ViewData["eTangs"] = eTangs;
            ViewBag.gioitinh = gioitinh;
            return View();
        }
        public JsonResult KiemTraTrung(string maSinhVien)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Json("");
            var db = new DB();
            bool result = db.SinhViens.Any(x => x.MaSinhVien == maSinhVien);
            SinhVien sinhVien = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == maSinhVien);
            string chitietsinhvien = "";
            if (sinhVien != null)
            {
                chitietsinhvien = JsonConvert.SerializeObject(new
                {
                    sinhVien.MaSinhVien,
                    sinhVien.MaLop,
                    sinhVien.TenSinhVien,
                    sinhVien.NgaySinh,
                    sinhVien.GioiTinh,
                    sinhVien.DanToc,
                    sinhVien.HoKhau,
                    sinhVien.SDT,
                    sinhVien.GhiChu,
                    sinhVien.TrangThaiO,
                    sinhVien.HinhAnh
                });
            }
            return Json(new
            {
                result = result,
                sinhVien = chitietsinhvien
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DangKyNoiTru(SinhVien sinhVien, DateTime TuThang, DateTime DenThang, float TienPhi, int dangkychosinhvien = 1)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            if (dangkychosinhvien == 1)
            {
                sinhVien.NgayNhanPhong = DateTime.Now;
                sinhVien.TrangThaiO = (int)TrangThaiO.DangO;
                sinhVien.NgayNhanPhong = new DateTime(TuThang.Year, TuThang.Month, TuThang.Day);
                db.SinhViens.Add(sinhVien);
            }
            else
            {
                SinhVien sinhVienOld = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == sinhVien.MaSinhVien);
                if (sinhVienOld != null)
                {
                    sinhVienOld.MaLop = sinhVien.MaLop;
                    sinhVienOld.HoKhau = sinhVien.HoKhau;
                    sinhVienOld.DanToc = sinhVien.DanToc;
                    sinhVienOld.GhiChu = sinhVien.GhiChu;
                    sinhVienOld.GioiTinh = sinhVien.GioiTinh;
                    sinhVienOld.HinhAnh = sinhVien.HinhAnh;
                    sinhVienOld.NgaySinh = sinhVien.NgaySinh;
                    sinhVienOld.SDT = sinhVien.SDT;
                    sinhVienOld.TenSinhVien = sinhVien.TenSinhVien;
                    sinhVienOld.SoHieuPhong = sinhVien.SoHieuPhong;
                    sinhVienOld.TrangThaiO = (int)TrangThaiO.DangO;
                    sinhVienOld.NgayNhanPhong = TuThang;
                }
            }
            db.PhiPhongs.Add(new PhiPhong()
            {
                MaSinhVien = sinhVien.MaSinhVien,
                Thang = TuThang,
                DenThang = DenThang,
                SoTien = TienPhi,
                TrangThai = true,
            });
            db.SaveChanges();
            return RedirectToAction("DanhSachSinhVien");
        }

        public ActionResult ChuyenPhong(string maSinhVien = "")
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            List<SinhVien> sinhViens = db.SinhViens.Where(x => (string.IsNullOrEmpty(maSinhVien) || x.MaSinhVien.Contains(maSinhVien)) && x.TrangThaiO != (int)TrangThaiO.CheckOut).OrderByDescending(x => x.NgayNhanPhong).ToList();
            List<Phong> phongs = db.Phongs.Where(y => y.TrangThai != null && y.TrangThai.Value).ToList();
            List<EPhong> ePhongs = new List<EPhong>();
            phongs.ForEach(x =>
            {
                int svDaO = db.SinhViens.Where(y => y.SoHieuPhong == x.SoHieuPhong && y.TrangThaiO == (int)TrangThaiO.DangO).Count();
                if (svDaO < x.SucChuaToiDa)
                {
                    ePhongs.Add(new EPhong()
                    {
                        LoaiPhong = x.LoaiPhong,
                        MaLoaiPhong = x.MaLoaiPhong,
                        SoHieuPhong = x.SoHieuPhong,
                        SoPhongDaO = svDaO,
                        SucChuaToiDa = x.SucChuaToiDa,
                        TangId = x.TangId
                    });
                }
            });
            ViewData["sinhViens"] = sinhViens;
            ViewData["ePhongs"] = ePhongs;
            return View();
        }

        [HttpPost]
        public ActionResult ChuyenPhong(string MaSinhVien, string SoHieuPhongCu, string SoHieuPhongMoi, string GhiChu)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            SinhVien sinhVien = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == MaSinhVien);
            if (sinhVien != null)
            {
                sinhVien.SoHieuPhong = SoHieuPhongMoi;
                db.SinhVienChuyenPhongs.Add(new SinhVienChuyenPhong()
                {
                    SoHieuPhongCu = SoHieuPhongCu,
                    SoHieuPhongMoi = SoHieuPhongMoi,
                    NgayCapNhat = DateTime.Now,
                    GhiChu = GhiChu,
                    MaSinhVien = MaSinhVien
                });
                db.SaveChanges();
            }
            return RedirectToAction("ChuyenPhong");
        }

        public ActionResult DangKyNoiTruNhanh()
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            ViewData["sinhViens"] = db.SinhViens.Where(x => x.TrangThaiO == (int)TrangThaiO.ChoNhanPhongMoi).OrderByDescending(x => x.NgayNhanPhong).ToList();
            return View();
        }

        public ActionResult ChiTietDangKyNoiTruNhanh(string masinhvien)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            SinhVien sinhVien = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == masinhvien);
            List<Tang> tangs = db.Tangs.OrderByDescending(x => x.TangId).ToList();
            if (tangs == null)
                tangs = new List<Tang>();
            List<ETang> eTangs = new List<ETang>();
            tangs.ForEach(x =>
            {
                ETang eTang = new ETang()
                {
                    TangId = x.TangId,
                    TenTang = x.TenTang,
                    Phongs = new List<EPhong>()
                };
                x.Phongs.ToList().ForEach(y =>
                {
                    if ((sinhVien.GioiTinh == "Nam" && (y.LoaiPhong.MucDich == (int)MucDichLoaiPhong.ChiDanhCHoNam || y.LoaiPhong.MucDich == (int)MucDichLoaiPhong.CaNamNu))
                       || (sinhVien.GioiTinh == "Nữ" && (y.LoaiPhong.MucDich == (int)MucDichLoaiPhong.ChiDanhCHoNu || y.LoaiPhong.MucDich == (int)MucDichLoaiPhong.CaNamNu)))
                    {
                        int svDaO = db.SinhViens.Where(z => z.SoHieuPhong == y.SoHieuPhong && z.TrangThaiO == (int)TrangThaiO.DangO).Count();
                        eTang.Phongs.Add(new EPhong()
                        {
                            LoaiPhong = y.LoaiPhong,
                            MaLoaiPhong = y.MaLoaiPhong,
                            SoHieuPhong = y.SoHieuPhong,
                            SoPhongDaO = svDaO,
                            SucChuaToiDa = y.SucChuaToiDa,
                            TangId = y.TangId
                        });
                    }
                });
                eTangs.Add(eTang);
            });
            ViewData["eTangs"] = eTangs;
            ViewData["lops"] = db.Lops.ToList();
            ViewData["sinhvien"] = sinhVien;
            return View();
        }
        [HttpPost]
        public ActionResult DangKyNoiTruNhanh(SinhVien sinhVien)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            SinhVien sv = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == sinhVien.MaSinhVien);
            if (sv != null)
            {
                sv.SoHieuPhong = sinhVien.SoHieuPhong;
                sv.TrangThaiO = (int)TrangThaiO.DangO;
                db.SaveChanges();
            }
            return RedirectToAction("DangKyNoiTruNhanh");
        }

        public ActionResult DangKyThem(string gioitinh = "Nam")
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            ViewData["lops"] = db.Lops.ToList();
            List<Tang> tangs = db.Tangs.OrderByDescending(x => x.TangId).ToList();
            if (tangs == null)
                tangs = new List<Tang>();
            List<ETang> eTangs = new List<ETang>();
            tangs.ForEach(x =>
            {
                ETang eTang = new ETang()
                {
                    TangId = x.TangId,
                    TenTang = x.TenTang,
                    Phongs = new List<EPhong>()
                };
                x.Phongs.ToList().ForEach(y =>
                {
                    if ((gioitinh == "Nam" && (y.LoaiPhong.MucDich == (int)MucDichLoaiPhong.ChiDanhCHoNam || y.LoaiPhong.MucDich == (int)MucDichLoaiPhong.CaNamNu))
                      || (gioitinh == "Nữ" && (y.LoaiPhong.MucDich == (int)MucDichLoaiPhong.ChiDanhCHoNu || y.LoaiPhong.MucDich == (int)MucDichLoaiPhong.CaNamNu)))
                    {
                        int svDaO = db.SinhViens.Where(z => z.SoHieuPhong == y.SoHieuPhong && z.TrangThaiO == (int)TrangThaiO.DangO).Count();
                        eTang.Phongs.Add(new EPhong()
                        {
                            LoaiPhong = y.LoaiPhong,
                            MaLoaiPhong = y.MaLoaiPhong,
                            SoHieuPhong = y.SoHieuPhong,
                            SoPhongDaO = svDaO,
                            SucChuaToiDa = y.SucChuaToiDa,
                            TangId = y.TangId
                        });
                    }
                });
                eTangs.Add(eTang);
            });
            ViewData["eTangs"] = eTangs;
            ViewBag.gioitinh = gioitinh;
            return View();
        }

        public ActionResult ChiTietSinhVien(string masinhvien)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            SinhVien sinhVien = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == masinhvien);
            ViewData["sinhVien"] = sinhVien;
            ViewData["lops"] = db.Lops.ToList();
            ViewData["phiPhongs"] = db.PhiPhongs.Where(x => x.MaSinhVien == masinhvien).OrderByDescending(x => x.Thang).ToList();
            return View();
        }
        public ActionResult Xoa(string masinhvien)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            SinhVien sinhVien = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == masinhvien);
            if (sinhVien != null)
            {
                db.SinhVienKyLuats.RemoveRange(db.SinhVienKyLuats.Where(x => x.MaSinhVien == masinhvien));
                db.SinhVienChuyenPhongs.RemoveRange(db.SinhVienChuyenPhongs.Where(x => x.MaSinhVien == masinhvien));
                db.SinhVienOLais.RemoveRange(db.SinhVienOLais.Where(x => x.MaSinhVien == masinhvien));
                db.SinhViens.Remove(sinhVien);
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachSinhVien");
        }

        public ActionResult CapNhatThongTin(SinhVien sinhVien)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            SinhVien sinhVienOld = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == sinhVien.MaSinhVien);
            if (sinhVienOld != null)
            {
                sinhVienOld.MaLop = sinhVien.MaLop;
                sinhVienOld.MaSinhVien = sinhVienOld.MaSinhVien;
                sinhVienOld.HoKhau = sinhVien.HoKhau;
                sinhVienOld.DanToc = sinhVien.DanToc;
                sinhVienOld.GhiChu = sinhVien.GhiChu;
                sinhVienOld.GioiTinh = sinhVien.GioiTinh;
                sinhVienOld.HinhAnh = sinhVien.HinhAnh;
                sinhVienOld.NgaySinh = sinhVien.NgaySinh;
                sinhVienOld.SDT = sinhVien.SDT;
                sinhVienOld.TenSinhVien = sinhVien.TenSinhVien;
                db.SaveChanges();
            }
            return RedirectToAction("ChiTietSinhVien", new { masinhvien = sinhVien.MaSinhVien });
        }

        public ActionResult InHopDongNoiTru(string MaSinhVien, string TenSinhVien, string GioiTinh, string SDT, string MaLop, string QueQuan)
        {
            var db = new DB();
            MauBieu mauBieu = db.MauBieux.FirstOrDefault(x => x.LoaiMauBieuId == (int)LoaiMauBieu.HOPDONGNOITRU);
            DateTime dateTime = DateTime.Now;
            Lop lop = db.Lops.FirstOrDefault(x => x.MaLop == MaLop);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{masinhvien}", MaSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tensinhvien}", TenSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{ngay}", dateTime.Day.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{thang}", dateTime.Month.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{nam}", dateTime.Year.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{gioitinh}", GioiTinh);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{sodienthoai}", SDT);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{lop}", lop.TenLop);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{khoa}", lop.Khoa.TenKhoa);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{quequan}", QueQuan);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{daidienbanqlktx}", GetLoginFullName());
            ViewData["NoiDung"] = mauBieu.NoiDung;
            return View();
        }

        public ActionResult InBienLaiThuTienCoc(string MaSinhVien, string TenSinhVien, string Phong)
        {
            var db = new DB();
            MauBieu mauBieu = db.MauBieux.FirstOrDefault(x => x.LoaiMauBieuId == (int)LoaiMauBieu.BIENLAITHUTIENCOC);
            DateTime dateTime = DateTime.Now;
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{sophieu}", "BLTC" + MaSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tensinhvien}", TenSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{ngay}", dateTime.Day.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{thang}", dateTime.Month.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{nam}", dateTime.Year.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{phong}", Phong);
            ViewData["NoiDung"] = mauBieu.NoiDung;
            return View();
        }
        public ActionResult InBienLaiThuTienPhong(string MaSinhVien, string TenSinhVien, string Phong, string TuThang, string DenThang, string SoTien)
        {
            var db = new DB();
            MauBieu mauBieu = db.MauBieux.FirstOrDefault(x => x.LoaiMauBieuId == (int)LoaiMauBieu.BIENLAITHUPHONG);
            DateTime dateTime = DateTime.Now;
            string SoTienBangChu = TienHelper.Convert_NumtoText(SoTien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{sophieu}", "BLTP" + MaSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tensinhvien}", TenSinhVien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{ngay}", dateTime.Day.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{thang}", dateTime.Month.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{nam}", dateTime.Year.ToString());
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{phong}", Phong);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{tuthang}", TuThang);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{denthang}", DenThang);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{sotien}", SoTien);
            mauBieu.NoiDung = mauBieu.NoiDung.Replace("{sotienbangchu}", SoTienBangChu);
            ViewData["NoiDung"] = mauBieu.NoiDung;
            return View();
        }
    }
}
