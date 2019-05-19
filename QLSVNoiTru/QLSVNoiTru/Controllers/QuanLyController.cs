using QLSVNoiTru.Database;
using QLSVNoiTru.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

namespace QLSVNoiTru.Controllers
{
    public class QuanLyController : BaseController
    {
        // GET: QuanLy
        public ActionResult KhoiTaoPhongKTXMoi()
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
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
                });
                eTangs.Add(eTang);
            });
            List<ESinhVien> eSinhViens = new List<ESinhVien>();
            DateTime currentDate = DateTime.Now;
            List<SinhVien> sinhViens = db.SinhViens.Where(x=>x.TrangThaiO == (int)TrangThaiO.DangO).ToList();
            sinhViens.ForEach(x =>
            {
                PhiPhong phiPhongCuoiCung = x.PhiPhongs.Where(y => y.TrangThai == true && y.Thang <= currentDate && y.DenThang >= currentDate).OrderByDescending(z => z.DenThang).FirstOrDefault();
                if (phiPhongCuoiCung is null)
                {
                    eSinhViens.Add(new ESinhVien()
                    {
                        SinhVien = x,
                        Chon = false,
                        MaSinhVien = x.MaSinhVien
                    });
                }
                //bool svOLai = db.SinhVienOLais.Any(y => y.MaSinhVien == x.MaSinhVien);
                //eSinhViens.Add(new ESinhVien()
                //{
                //    SinhVien = x,
                //    Chon = false,
                //    MaSinhVien = x.MaSinhVien
                //});
            });

            ViewData["eTangs"] = eTangs;
            ViewData["eSinhViens"] = eSinhViens;
            return View();
        }

        [HttpPost]
        public ActionResult KhoiTaoPhongKTXMoi(List<ESinhVien> eSinhViens)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            DateTime currentDate = DateTime.Now;
            db.SinhVienOLais.RemoveRange(db.SinhVienOLais);
            List<SinhVien> sinhViens = db.SinhViens.Where(x => x.TrangThaiO == (int)TrangThaiO.DangO).ToList();
            //sinhViens.ForEach(x =>
            //{
            //    if (eSinhViens.Any(y => y.MaSinhVien == x.MaSinhVien && y.Chon))
            //        x.TrangThaiO = (int)TrangThaiO.ChoNhanPhongMoi;
            //    else
            //        x.TrangThaiO = (int)TrangThaiO.CheckOut;
            //});
            //eSinhViens.ForEach(x =>
            //{
            //    if (x.Chon)
            //    {
            //        db.SinhVienOLais.Add(new SinhVienOLai()
            //        {
            //            MaSinhVien = x.MaSinhVien
            //        });
            //    }
            //});
            sinhViens.ForEach(x =>
            {
                if (eSinhViens.Any(y => y.MaSinhVien == x.MaSinhVien && y.Chon))
                {
                    PhiPhong phiPhongCuoiCung = x.PhiPhongs.Where(y => y.TrangThai == true && y.Thang <= currentDate && y.DenThang >= currentDate).OrderByDescending(z => z.DenThang).FirstOrDefault();
                    if (phiPhongCuoiCung is null)
                    {
                        x.TrangThaiO = (int)TrangThaiO.CheckOut;
                        x.KhenThuong = "";
                        foreach (var item in x.SinhVienKyLuats)
                        {
                            item.Chon = false;
                        }
                    }
                }
            });
            db.SaveChanges();
            return RedirectToAction("KhoiTaoPhongKTXMoi");
        }

        public ActionResult SinhVienCheckout(string masinhvien)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            ViewBag.masinhvien = masinhvien;
            var db = new DB();
            SinhVien sinhVien = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == masinhvien && x.TrangThaiO != (int)TrangThaiO.CheckOut);
            ViewData["sinhVien"] = sinhVien;
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(string masinhvien)
        {
            if (!CheckLogin(QuyenDangNhap.BPQuanLy))
                return Redirect("/Login/DangNhap");
            ViewBag.masinhvien = masinhvien;
            var db = new DB();
            SinhVien sinhVien = db.SinhViens.FirstOrDefault(x => x.MaSinhVien == masinhvien);
            if (sinhVien != null)
            {
                sinhVien.TrangThaiO = (int)TrangThaiO.CheckOut;
                sinhVien.KhenThuong = "";
                foreach (var item in sinhVien.SinhVienKyLuats)
                {
                    item.Chon = false;
                }
                db.SaveChanges();
            }
            return RedirectToAction("SinhVienCheckout");
        }


    }
}