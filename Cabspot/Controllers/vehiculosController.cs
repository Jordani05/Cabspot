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

namespace Cabspot.Controllers
{
    [Authorize]
    public class vehiculosController : Controller
    {
        private CabspotDB db = new CabspotDB();

        // GET: vehiculos
        public async Task<ActionResult> Index()
        {
            var vehiculos = db.vehiculos.Include(v => v.condicionvehiculos).Include(v => v.estadovehiculos).Include(v => v.tipovehiculos);
            return View(await vehiculos.ToListAsync());
        }

        // GET: vehiculos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vehiculos vehiculos = await db.vehiculos.FindAsync(id);
            if (vehiculos == null)
            {
                return HttpNotFound();
            }
            return View(vehiculos);
        }

        // GET: vehiculos/Create
        public ActionResult Create()
        {
            ViewBag.idCondicionVehiculo = new SelectList(db.condicionvehiculos, "idCondicionVehiculo", "condicionVehiculo");
            ViewBag.idEstadoVehiculo = new SelectList(db.estadovehiculos, "idEstadoVehiculo", "estadoVehiculo");
            ViewBag.idTipoVehiculo = new SelectList(db.tipovehiculos, "idTipoVehiculo", "tipoVehiculo");
            return View();
        }

        // POST: vehiculos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(vehiculos vehiculos)
        {
            if (ModelState.IsValid)
            {
                db.vehiculos.Add(vehiculos);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idCondicionVehiculo = new SelectList(db.condicionvehiculos, "idCondicionVehiculo", "condicionVehiculo", vehiculos.idCondicionVehiculo);
            ViewBag.idEstadoVehiculo = new SelectList(db.estadovehiculos, "idEstadoVehiculo", "estadoVehiculo", vehiculos.idEstadoVehiculo);
            ViewBag.idTipoVehiculo = new SelectList(db.tipovehiculos, "idTipoVehiculo", "tipoVehiculo", vehiculos.idTipoVehiculo);
            return View(vehiculos);
        }

        // GET: vehiculos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vehiculos vehiculos = await db.vehiculos.FindAsync(id);
            if (vehiculos == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCondicionVehiculo = new SelectList(db.condicionvehiculos, "idCondicionVehiculo", "condicionVehiculo", vehiculos.idCondicionVehiculo);
            ViewBag.idEstadoVehiculo = new SelectList(db.estadovehiculos, "idEstadoVehiculo", "estadoVehiculo", vehiculos.idEstadoVehiculo);
            ViewBag.idTipoVehiculo = new SelectList(db.tipovehiculos, "idTipoVehiculo", "tipoVehiculo", vehiculos.idTipoVehiculo);
            return View(vehiculos);
        }

        // POST: vehiculos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idVehiculo,idTaxista,chasis,placa,marca,modelo,serie,anio,color,unidad,idTipoVehiculo,idEstadoVehiculo,idCondicionVehiculo,cantidadAsientos,fechaRegistro,registradoPor,fechaUltimaModificacion,modificadoPor")] vehiculos vehiculos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vehiculos).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idCondicionVehiculo = new SelectList(db.condicionvehiculos, "idCondicionVehiculo", "condicionVehiculo", vehiculos.idCondicionVehiculo);
            ViewBag.idEstadoVehiculo = new SelectList(db.estadovehiculos, "idEstadoVehiculo", "estadoVehiculo", vehiculos.idEstadoVehiculo);
            ViewBag.idTipoVehiculo = new SelectList(db.tipovehiculos, "idTipoVehiculo", "tipoVehiculo", vehiculos.idTipoVehiculo);
            return View(vehiculos);
        }

        // GET: vehiculos/Delete/5
        public async Task<ActionResult> Inactivar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vehiculos vehiculos = await db.vehiculos.FindAsync(id);
            if (vehiculos == null)
            {
                return HttpNotFound();
            }
            return View(vehiculos);
        }
        
        // POST: vehiculos/Delete/5
        [HttpPost, ActionName("Inactivar")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            vehiculos vehiculos = await db.vehiculos.FindAsync(id);
            taxistas taxista = await db.taxistas.FindAsync(vehiculos.taxistas); 

            if (vehiculos != null)
            {
                vehiculos.idEstadoVehiculo = 41;
                db.Entry(vehiculos).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //si es el unico vehiculo, marcar al taxista como inactivo
                if (taxista.vehiculos.Where(v => v.idEstadoVehiculo == 31).Count() > 0)
                {
                    taxista.idEstadoDisponibilidad = 111;
                    db.Entry(taxista).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                //si se inhabilita el vehiculo activo, mandar a elegir un vehículo




                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }


            db.vehiculos.Remove(vehiculos);
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
