using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MVCBlogSite.Models;
using PagedList;
using PagedList.Mvc;
namespace MVCBlogSite.Controllers
{
    public class AdminMakaleController : Controller
    {
        mvcblogDB dB = new mvcblogDB();
        // GET: AdminMakale
        public ActionResult Index(int Page=1)
        {
            var makales = dB.Makales.OrderByDescending(m=>m.MakaleId).ToPagedList(Page, 10);
            return View(makales);
        }

        // GET: AdminMakale/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminMakale/Create
        public ActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(dB.Kategoris,"KategoriId","KategoriAdi"); //viewbag ile kategori id'yi kategoris'ten çektik.
            return View();
        }

        // POST: AdminMakale/Create
        [HttpPost]
        public ActionResult Create(Makale makale,string etiketler,HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);

                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newfoto);
                    makale.Foto = "~/Uploads/MakaleFoto/" + newfoto; //resmin yolunu veritabanında tuttuk kendisini klasöre yükledik.
                    

                }
                if (etiketler != null)
                {
                    string[] etiketdizi = etiketler.Split(',');
                    foreach (var i in etiketdizi)
                    {
                        var yenietiket = new Etiket { EtiketAdi = i };
                        dB.Etikets.Add(yenietiket);
                        makale.Etikets.Add(yenietiket);
                    }
                }
                makale.UyeId = Convert.ToInt32(Session["uyeid"]);
                makale.Okunma = 1;
                dB.Makales.Add(makale);
                dB.SaveChanges();
                dB.SaveChanges();
                return RedirectToAction("Index");
            }
                
                return View(makale);
            
        }

        // GET: AdminMakale/Edit/5
        public ActionResult Edit(int id)
        {
            var makale = dB.Makales.Where(m=>m.MakaleId==id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriId = new SelectList(dB.Kategoris, "KategoriId", "KategoriAdi",makale.KategoriId);
            return View(makale);
        }

        // POST: AdminMakale/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, HttpPostedFileBase Foto, Makale makale)
        {
            try
            {
                var makales = dB.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                if (Foto!=null)
                {
                    if (System.IO.File.Exists(Server.MapPath(makales.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(makales.Foto));
                    }
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);

                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newfoto );
                    makales.Foto = "~/Uploads/MakaleFoto/" + newfoto;
                    makales.Baslik = makale.Baslik;
                    makales.Icerik = makale.Icerik;
                    makales.KategoriId = makale.KategoriId;
                    dB.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View();
                
            }
            catch
            {
                ViewBag.KategoriId = new SelectList(dB.Kategoris, "KategoriId", "KategoriAdi", makale.KategoriId);

                return View();
            }
        }

        // GET: AdminMakale/Delete/5
        public ActionResult Delete(int id)
        {
            var makale = dB.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }

        // POST: AdminMakale/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection Collections)
        {
            try
            {
                var makales = dB.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                if (makales == null)
                {
                    return HttpNotFound();
                }
                if (System.IO.File.Exists(Server.MapPath(makales.Foto)))
                {
                    System.IO.File.Delete(Server.MapPath(makales.Foto));
                }
                foreach (var i in makales.Yorums.ToList())
                {
                    dB.Yorums.Remove(i);
                }
                foreach (var i in makales.Etikets.ToList())
                {
                    dB.Etikets.Remove(i);
                }
                dB.Makales.Remove(makales);
                dB.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
