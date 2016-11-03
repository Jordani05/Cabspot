using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cabspot.Models;
using Cabspot.Controllers.Clases;

namespace Cabspot.Controllers
{
    [CustomAuthorizeRole("Developer")]
    public class basesController : Controller
    {
        private CabspotDB db = new CabspotDB();

        public JsonResult ListaMunicipios(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int codigoProvincia = int.Parse(id);
                var municipio = from m in db.municipios where m.idProvincia == codigoProvincia orderby m.nombreMunicipio select m;
                JsonResult json = Json(new SelectList(municipio.ToArray(), "idMunicipio", "nombreMunicipio"), JsonRequestBehavior.AllowGet);
                return json;
            }
            return null;
        }

        // GET: bases
        public async Task<ActionResult> Index()
        {
            var bases = db.bases.Include(b => b.contactos).Include(b => b.direcciones);
            return View(await bases.ToListAsync());
        }

        // GET: bases/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bases bases = await db.bases.FindAsync(id);
            if (bases == null)
            {
                return HttpNotFound();
            }
            return View(bases);
        }

        // GET: bases/Create
        public ActionResult Create()
        {
            //creando objetos de entidades bases y direcciones
            bases bases = new bases();

            direcciones d = new direcciones();
            d.listaProvincias = new SelectList(db.provincias, "idProvincia", "nombreProvincia");  //enviando el listado de provincias al View
            bases.direcciones = d;

            return View(bases);
        }

        // POST: bases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(bases bases)
        {
            //quitando parentesis y guiones agregados por la mascara en la vista de numeros de telefono
            if (bases.contactos.telefonoMovil != null) { 
                bases.contactos.telefonoMovil = bases.contactos.telefonoMovil.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim();

                //unique movil
                var movil = from n in db.bases where n.contactos.telefonoMovil.Equals(bases.contactos.telefonoMovil) select n;
                if (movil.Count() > 0)
                {
                    ModelState.AddModelError("contactos.telefonoMovil", "Existe una base con este teléfono móvil");
                }
            }

            if (bases.contactos.telefonoTrabajo != null) { bases.contactos.telefonoTrabajo = bases.contactos.telefonoTrabajo.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim(); }
            if (bases.contactos.telefonoResidencial != null) { bases.contactos.telefonoResidencial = bases.contactos.telefonoResidencial.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim(); }
            if (bases.contactos.fax != null) { bases.contactos.fax = bases.contactos.fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim(); }

            //buscando campos unique de nombre 
            if (!string.IsNullOrEmpty(bases.nombreBase))
            {
                var nombre = from n in db.bases where n.nombreBase.ToUpper().Equals(bases.nombreBase.ToUpper()) select n;
                if (nombre.Count() > 0)
                {
                    ModelState.AddModelError("nombreBase", "Existe una base con este nombre");
                }
            }

            //unique email
            if (bases.contactos.email != null)
            {
                bases.contactos.email = bases.contactos.email.Trim();
                var movil = from n in db.bases where n.contactos.email.Equals(bases.contactos.email) select n;
                if (movil.Count() > 0)
                {
                    ModelState.AddModelError("personas.contactos.email", "Este móvil ya está en uso");
                }
            }
            //fin unique email

            
            
            if (ModelState.IsValid)
            {
                try
                {


                    //municipio para direccion
                    bases.direcciones.idMunicipio = bases.direcciones.municipioSeleccionado;
                    //agregando base a db y guardando
                    db.bases.Add(bases);
                    await db.SaveChangesAsync();

                    //redirigiendo a home...tempdata para mensaje de exito
                    TempData["success"] = "Se ha añadido una base exitosamente";
                    return RedirectToAction("Index", "Bases");
                }
                catch (Exception e)
                {
                    TempData["error"] = e.ToString();
                    return Redirect("~/Shared/Error.cshtml");
                }

            }
            bases.direcciones.listaProvincias = new SelectList(db.provincias, "idProvincia", "nombreProvincia");  //enviando el listado de provincias al View
            ViewBag.municipio = bases.direcciones.municipioSeleccionado;
            return View(bases);
        }

        // GET: bases/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bases bases = await db.bases.FindAsync(id);
            if (bases == null)
            {
                return HttpNotFound();
            }
            ViewBag.idContacto = new SelectList(db.contactos, "idContacto", "telefonoMovil", bases.idContacto);
            ViewBag.idDireccion = new SelectList(db.direcciones, "idDireccion", "numeroPuerta", bases.idDireccion);
            return View(bases);
        }

        // POST: bases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(bases bases)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bases).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idContacto = new SelectList(db.contactos, "idContacto", "telefonoMovil", bases.idContacto);
            ViewBag.idDireccion = new SelectList(db.direcciones, "idDireccion", "numeroPuerta", bases.idDireccion);
            return View(bases);
        }

        // GET: bases/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bases bases = await db.bases.FindAsync(id);
            if (bases == null)
            {
                return HttpNotFound();
            }
            return View(bases);
        }

        // POST: bases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            bases bases = await db.bases.FindAsync(id);
            db.bases.Remove(bases);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
