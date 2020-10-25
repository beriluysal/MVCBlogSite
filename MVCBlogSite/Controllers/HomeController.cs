using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using MVCBlogSite.Models;
using PagedList;
using PagedList.Mvc;

namespace MVCBlogSite.Controllers
{

   
    public class HomeController : Controller
    {
        // GET: Home
         mvcblogDB dB = new mvcblogDB();
        public ActionResult Index(int Page=1)
        {
            var makale = dB.Makales.OrderByDescending(m => m.MakaleId).ToPagedList(Page,5);
            return View(makale);
        }
        public ActionResult BlogAra(string Ara = null)
        {
            var aranan = dB.Makales.Where(m => m.Baslik.Contains(Ara)).ToList();
            return View(aranan.OrderByDescending(m => m.Tarih));
        }
        public ActionResult SonYorumlar()
        {
            return View(dB.Yorums.OrderByDescending(y => y.YorumId).Take(5));
        }
        public ActionResult PopulerMakaleler()
        {
            return View(dB.Makales.OrderByDescending(m => m.Okunma).Take(5));
        }
        public ActionResult KategoriMakale(int id)
        {
            var makaleler = dB.Makales.Where(m => m.KategoriId == id).ToList();
            return View(makaleler);
        }
        public ActionResult MakaleDetay(int id)
        {
            var makale = dB.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            return View(makale);

        }
        public ActionResult Hakkimda()
        {
            return View();
        }
        public ActionResult Iletisim()
        {
            return View();
        }
        public ActionResult KategoriPartial()
        {
            return View(dB.Kategoris.ToList());
        }
        public JsonResult YorumYap(string yorum, int Makaleid)
        {
            var uyeid = Session["uyeid"];
            if (yorum==null)
            {
               return Json(false, JsonRequestBehavior.AllowGet);
            }
            dB.Yorums.Add(new Yorum { UyeId = Convert.ToInt32(uyeid), MakaleId = Makaleid, Icerik = yorum, Tarih = DateTime.Now });
            dB.SaveChanges();
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult YorumSil(int id)
        {
            var uyeid = Session["uyeid"];
            var yorum = dB.Yorums.Where(y => y.YorumId == id).SingleOrDefault();
            var makale = dB.Makales.Where(m => m.MakaleId == yorum.MakaleId).SingleOrDefault();
            if (yorum.UyeId == Convert.ToInt32(uyeid))
            {
                dB.Yorums.Remove(yorum);
                dB.SaveChanges();
                return RedirectToAction("MakaleDetay", "Home", new { id = makale.MakaleId });
            }
            else
            {
                return HttpNotFound();
            }
        }
        public ActionResult OkunmaArttir(int Makaleid)
        {
            var makale = dB.Makales.Where(m => m.MakaleId == Makaleid).SingleOrDefault();
            makale.Okunma += 1;
            dB.SaveChanges();
            return View();
        }
    }
}