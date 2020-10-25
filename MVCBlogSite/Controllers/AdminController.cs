using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCBlogSite;
using MVCBlogSite.Models;

namespace MVCBlogSite.Controllers
{
    public class AdminController : Controller
    {
        mvcblogDB dB = new mvcblogDB();
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.MakaleSayisi = dB.Makales.Count();
            ViewBag.YorumSayisi = dB.Yorums.Count();
            ViewBag.KategoriSayisi = dB.Kategoris.Count();
            ViewBag.UyeSayisi = dB.Uyes.Count();
            return View();
        }
    }
}