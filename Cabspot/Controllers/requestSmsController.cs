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
using Twilio;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Cabspot.Controllers.Clases;

namespace Cabspot.Controllers
{
    public partial class MyDatabaseEntities : DbContext
    {
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Class: {0}, Property: {1}, Error: {2}",
                            validationErrors.Entry.Entity.GetType().FullName,
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }

                throw;  // You can also choose to handle the exception here...
            }
        }
    }

    public class requestSmsController : Controller
    {
        private CabspotDB db = new CabspotDB();

        // GET: requestSms
        public async Task<ActionResult> Index()
        {
            var clientes = db.clientes.Include(c => c.personas);
            return View(await clientes.ToListAsync());
        }

        

        // GET: requestSms/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clientes clientes = await db.clientes.FindAsync(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // GET: requestSms/Create
        public ActionResult Create()
        {
            ViewBag.idPersona = new SelectList(db.personas, "idPersona", "identificacion");
            return View();
        }

        [System.Web.Http.Route("taxistas/telefono/{telefonoMovil}")]
        public JsonResult getTelefonoMovil(string telefonoMovil)
        {
            //recibo un json el cual convierto a string
            string telefono = "+18297596854";
            Random random = new Random();
            //clientesMovil c = createClienteMovil(telefono);
            //autenticacionsms sms = generarCodigo(telefono);
            int otp = random.Next(100000, 999999);
            string AccountSid = Constantes.ACCOUNT_SID;
            string AuthToken = Constantes.AUTH_TOKEN;
            var twilio = new TwilioRestClient(AccountSid, AuthToken);

            var message = twilio.SendMessage(
                "+19179831394", telefono,
                "Tu codigo es este " + otp.ToString());

            return Json("jordani" + "+" + telefono, JsonRequestBehavior.AllowGet);
        }

        public clientesMovil createClienteMovil(String telefonoMovil)
        {
            clientesMovil c = new clientesMovil();
            c.telefonoMovil = "+" + telefonoMovil;
            //c.telefonoMovil = telefonoMovil = "+18099801767";
            db.clientesMovil.Add(c);
            db.SaveChanges();
        
            return c;
        }

        public autenticacionsms generarCodigo(String telefonoMovil)
        {
            Random random = new Random();
            autenticacionsms sms = new autenticacionsms();

            int otp = random.Next(100000, 999999);
            //buscando al cliente con el numeroMovil correcto
            var idclienteMovil = from cl in db.clientesMovil where cl.telefonoMovil.Equals("+" + telefonoMovil) select cl.idClienteMovil;
            
            sms.idClienteMovil = idclienteMovil.First();   //hay que validar que no sea nulo (debe existir)
            sms.codigo = otp.ToString();

            db.autenticacionSms.Add(sms);
            db.SaveChanges();

            return sms;
        }

        public JsonResult verificarCodigo(int otp)
        {
            autenticacionsms objetoSeleccionado = new autenticacionsms();
            var activarCliente = from clm in db.clientesMovil join sms in db.autenticacionSms on clm.idClienteMovil equals sms.idClienteMovil where (sms.idClienteMovil == clm.idClienteMovil) select sms;
            if (activarCliente.Count() > 0)
            {
                objetoSeleccionado = activarCliente.First();
                objetoSeleccionado.verificado = true;
                //db.autenticacionSms.Add(objetoSeleccionado);
                db.SaveChanges();
            }

            return Json("jordani " + otp, JsonRequestBehavior.AllowGet);
        }

        // POST: requestSms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idCliente,idPersona,nombreUsuario,apikey,fechaRegistro")] clientes clientes)
        {
            if (ModelState.IsValid)
            {
                db.clientes.Add(clientes);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idPersona = new SelectList(db.personas, "idPersona", "identificacion", clientes.idPersona);
            return View(clientes);
        }

        // GET: requestSms/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clientes clientes = await db.clientes.FindAsync(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            ViewBag.idPersona = new SelectList(db.personas, "idPersona", "identificacion", clientes.idPersona);
            return View(clientes);
        }

        // POST: requestSms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idCliente,idPersona,nombreUsuario,apikey,fechaRegistro")] clientes clientes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientes).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idPersona = new SelectList(db.personas, "idPersona", "identificacion", clientes.idPersona);
            return View(clientes);
        }

        // GET: requestSms/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clientes clientes = await db.clientes.FindAsync(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // POST: requestSms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            clientes clientes = await db.clientes.FindAsync(id);
            db.clientes.Remove(clientes);
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
