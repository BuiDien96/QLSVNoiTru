using QLSVNoiTru.Database;
using QLSVNoiTru.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace QLSVNoiTru.Controllers
{
    public class GiaNuocController : BaseController
    {
        // GET: GiaNuoc
        public ActionResult CapNhatGiaNuoc()
        {
            if (!CheckLogin(QuyenDangNhap.BPDienNuoc))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            ViewData["giaNuocs"] = db.GiaNuocs.OrderByDescending(x => x.NgayCapNhat).ToList();
            return View();         
        }

        [HttpPost]
        public ActionResult CapNhat(float DonGia)
        {
            if (!CheckLogin(QuyenDangNhap.BPDienNuoc))
                return Redirect("/Login/DangNhap");
            var db = new DB();
            GiaNuoc giaNuoc = new GiaNuoc()
            {
                Dongia = DonGia,
                NgayCapNhat = DateTime.Now
            };
            db.GiaNuocs.Add(giaNuoc);
            db.SaveChanges();
            return RedirectToAction("CapNhatGiaNuoc");
        }
    }
}