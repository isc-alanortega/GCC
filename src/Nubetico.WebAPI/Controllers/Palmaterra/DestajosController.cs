using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Nubetico.WebAPI.Controllers.Palmaterra
{
    public class DestajosController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetCatalogs()
        {
            return null;
        }

        [HttpPost]
        public ActionResult Create(IFormCollection collection)
        {
            return null;
        }

        // GET: DestajosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DestajosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            return null;
        }

        // GET: DestajosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }
    }
}
