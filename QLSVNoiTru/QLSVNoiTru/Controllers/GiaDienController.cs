using QLSVNoiTru.Database;
using QLSVNoiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLSVNoiTru.Controllers
{
    public class GiaDienController : BaseController
    {
        // GET: GiaDien
        public ActionResult CapNhatGiaDien()
        {
            if (!CheckLogin())
                return Redirect("/Login/DangNhap");
            var db = new DB();
            ViewData["giaDiens"] = db.GiaDiens.OrderByDescending(x=>x.NgayCapNhat).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CapNhat(float DonGia)
        {
            if (!CheckLogin())
                return Redirect("/Login/DangNhap");
            var db = new DB();
            GiaDien giaDien = new GiaDien()
            {
                Dongia = DonGia,
                NgayCapNhat = DateTime.Now
            };
            db.GiaDiens.Add(giaDien);
            db.SaveChanges();
            return RedirectToAction("CapNhatGiaDien");
        }
    }
}