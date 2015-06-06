using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Net;
using System.Threading.Tasks;
using MvcBiblioteka.Models;

namespace MvcBiblioteka.Controllers
{
    public class KsiazkaController : Controller
    {
        public ActionResult Index()
        {
            var items = DocumentDBRepository<Ksiazka>.GetItems(d => !d.Ilosc.Equals(0));
            return View(items);
        }

        public ActionResult SearchIndex(string autor)
        {
            var items = DocumentDBRepository<Ksiazka>.GetItems(d => !d.Ilosc.Equals(0) && d.Autor == autor);
            return View(items);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Tytul,Autor,Wydawnictwo,Miejsce_Wydania,Rok_Wydania,Nr_ISBN,Ilosc")] Ksiazka ksiazka)
        {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<Ksiazka>.CreateItemAsync(ksiazka);
                return RedirectToAction("Index");
            }
            return View(ksiazka);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Tytul,Autor,Wydawnictwo,Miejsce_Wydania,Rok_Wydania,Nr_ISBN,Ilosc")] Ksiazka ksiazka)
        {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<Ksiazka>.UpdateItemAsync(ksiazka.Id, ksiazka);
                return RedirectToAction("Index");
            }

            return View(ksiazka);
        }

        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ksiazka ksiazka = (Ksiazka)DocumentDBRepository<Ksiazka>.GetItem(d => d.Id == id);

            if (ksiazka == null)
            {
                return HttpNotFound();
            }

            return View(ksiazka);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete([Bind(Include = "Id,Tytul,Autor,Wydawnictwo,Miejsce_Wydania,Rok_Wydania,Nr_ISBN,Ilosc")] Ksiazka ksiazka)
        {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<Ksiazka>.DeleteItemAsync(ksiazka.Id);
                return RedirectToAction("Index");
            }

            return View(ksiazka);
        }

        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ksiazka ksiazka = (Ksiazka)DocumentDBRepository<Ksiazka>.GetItem(d => d.Id == id);

            if (ksiazka == null)
            {
                return HttpNotFound();
            }

            return View(ksiazka);
        }
    }
}