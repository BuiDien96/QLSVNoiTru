using QLSVNoiTru.Database;
using QLSVNoiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            if(mauBieuOld != null)
            {
                mauBieuOld.TieuDe = mauBieu.TieuDe;
                mauBieuOld.NoiDung = mauBieu.NoiDung;
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachMauBieu");
        }
    }
}