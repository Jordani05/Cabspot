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
using System.IO;
using Cabspot.Controllers.Clases;

namespace Cabspot.Controllers
{
    [Authorize]
    public class empleadosController : Controller
    {
        private CabspotDB db = new CabspotDB();

        public  JsonResult getUserData(string id)
        {
            List<string> userData = new List<string>();
            var data = (from u in db.empleados where u.usuario.Equals(id) select new { nombre = u.personas.nombres, nombreCompleto = u.personas.nombres + " " + u.personas.apellidos, foto = u.personas.foto }).First();
            JsonResult json = Json(new { data.nombreCompleto, data.foto, data.nombre }, JsonRequestBehavior.AllowGet);
          
            return json;
        }
         
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

        // GET: empleados
        public async Task<ActionResult> Index()
        {
            empleados emp = db.empleados.Where(x => x.usuario.Equals(User.Identity.Name)).First();

            if (emp.roles.rol.Equals("Developer"))
            {
                var empleadosTodos = db.empleados.Include(e => e.bases).Include(e => e.estadoempleados).Include(e => e.roles);
                return View(await empleadosTodos.ToListAsync());
            }
            else
            {
                var empleadosBase = db.empleados.Where(e => e.bases.idBase == emp.bases.idBase).Include(e => e.bases).Include(e => e.estadoempleados).Include(e => e.roles);
                return View(await empleadosBase.ToListAsync());
            }
            
        }

        // GET: empleados/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            empleados empleados = await db.empleados.FindAsync(id);
            if (empleados == null)
            {
                return HttpNotFound();
            }
            return View(empleados);
        }

        // GET: empleados/Create
        [CustomAuthorizeRole("Admin", "Super Admin", "Developer")]
        public ActionResult Create()
        {
            empleados empleado = new empleados();
            direcciones d = new direcciones();
            personas p = new personas();
            contactos c = new contactos();
            empleado.listaBases = new SelectList(db.bases, "idBase", "nombreBase");
            empleado.listaRoles = new SelectList(db.roles, "idRol", "rol");
            d.listaProvincias = new SelectList(db.provincias, "idProvincia", "nombreProvincia");  //enviando el listado de provincias al View
            p.direcciones = d;
            empleado.personas = p;
            
            return View(empleado);
        }

        // POST: empleados/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [CustomAuthorizeRole("Admin", "Super Admin", "Developer")]
        public async Task<ActionResult> Create(empleados empleados, HttpPostedFileBase foto)
        {
            
            //asignando ids
            try
            {
                //unique de cedula
                if (empleados.personas.identificacion != null) 
                { 
                    empleados.personas.identificacion = empleados.personas.identificacion.Replace("-", "").Trim();
                    var cedula = from n in db.empleados where n.personas.identificacion.Equals(empleados.personas.identificacion) select n;
                    if (cedula.Count() > 0)
                    {
                        ModelState.AddModelError("personas.identificacion", "Existe una persona con esta cédula");
                    }
                }
                //fin unique cedula

                //unique numero movil
                if (empleados.personas.contactos.telefonoMovil != null)
                {
                    empleados.personas.contactos.telefonoMovil = empleados.personas.contactos.telefonoMovil.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim();
                    var movil = from n in db.bases where n.contactos.telefonoMovil.Equals(empleados.personas.contactos.telefonoMovil) select n;
                    if (movil.Count() > 0)
                    {
                        ModelState.AddModelError("personas.contactos.telefonoMovil", "Este móvil ya está en uso");
                    }
                }
                //fin unique nnumero movil

                //unique email
                if (empleados.personas.contactos.email != null)
                {
                    empleados.personas.contactos.email = empleados.personas.contactos.email.Trim();
                    var movil = from n in db.bases where n.contactos.email.Equals(empleados.personas.contactos.email) select n;
                    if (movil.Count() > 0)
                    {
                        ModelState.AddModelError("personas.contactos.email", "Este móvil ya está en uso");
                    }
                }
                //fin unique email

                //unique usuario
                if (empleados.usuario != null)
                {
                    empleados.usuario = empleados.usuario.Trim();
                    var usuario = from n in db.empleados where n.usuario.Equals(empleados.usuario) select n;
                    if (usuario.Count() > 0)
                    {
                        ModelState.AddModelError("usuario", "El usuario ya existe");
                    }
                }

                //fin unique usuario


                //base y rol
                empleados.idBase = int.Parse(empleados.baseSeleccionada);
                empleados.idRol = int.Parse(empleados.rolSeleccionado);

                //estado siempre activo
                var idEstadoEmpleado = (from e in db.estadoempleados where e.estadoEmpleado.Equals("Activo") select e.idEstadoEmpleado).First();
                empleados.idEstadoEmpleado = idEstadoEmpleado;
                empleados.fechaRegistro = DateTime.Now;
                //registradoPor

                //direccion
                empleados.personas.direcciones.idMunicipio = empleados.personas.direcciones.municipioSeleccionado;

                //subir foto
                var filename = Path.GetFileName(foto.FileName);
                var path = "~/FotosPerfil/" + filename;
                foto.SaveAs(Server.MapPath(path));
                empleados.personas.foto = path;
               // ModelState.SetModelValue("personas.foto", new ValueProviderResult(path,path, ));

                //agregando la base del usuario loggeado sino es DEVELOPER
                empleados logeado = db.empleados.Where(e => e.usuario.Equals(User.Identity.Name)).First();
                if(logeado.roles.rol.Equals("Developer"))
                {
                    empleados.idBase = logeado.idBase;
                }

                //hashing contrasena
                empleados.SetPassword(empleados.contrasena);
            }
            catch (Exception e)
            {
                empleados.listaBases = new SelectList(db.bases, "idBase", "nombreBase");
                empleados.listaRoles = new SelectList(db.roles, "idRol", "rol");
                empleados.personas.direcciones.listaProvincias = new SelectList(db.provincias, "idProvincia", "nombreProvincia");  //enviando el listado de provincias al View

                return View(empleados);
            }
            
            
            if (ModelState.IsValid)
            {
                try
                {
                    db.empleados.Add(empleados);
                    await db.SaveChangesAsync();
                    //redirigiendo a index...tempdata para mensaje de exito
                    TempData["success"] = "Se ha añadido un empleado exitosamente";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    TempData["error"] = e.ToString();
                    return Redirect("~/Shared/Error.cshtml");
                }
                
            }
            var errors = ViewData.ModelState.Where(n => n.Value.Errors.Count > 0).ToList();
            //retornando en caso de falta de algo
            empleados.listaBases = new SelectList(db.bases, "idBase", "nombreBase");
            empleados.listaRoles = new SelectList(db.roles, "idRol", "rol");
            empleados.personas.direcciones.listaProvincias = new SelectList(db.provincias, "idProvincia", "nombreProvincia");  //enviando el listado de provincias al View
                
            return View(empleados);
        }

        // GET: empleados/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            empleados empleados = await db.empleados.FindAsync(id);
            if (empleados == null)
            {
                return HttpNotFound();
            }
            ViewBag.idBase = new SelectList(db.bases, "idBase", "nombreBase", empleados.idBase);
            ViewBag.registradoPor = new SelectList(db.empleados, "idEmpleado", "usuario", empleados.registradoPor);
            ViewBag.idEstadoEmpleado = new SelectList(db.estadoempleados, "idEstadoEmpleado", "estadoEmpleado", empleados.idEstadoEmpleado);
            ViewBag.idRol = new SelectList(db.roles, "idRol", "rol", empleados.idRol);
            return View(empleados);
        }

        // POST: empleados/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idEmpleado,fechaRegistro,registradoPor,idEstadoEmpleado,idBase,idRol,usuario,contrasena")] empleados empleados)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empleados).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idBase = new SelectList(db.bases, "idBase", "nombreBase", empleados.idBase);
            ViewBag.registradoPor = new SelectList(db.empleados, "idEmpleado", "usuario", empleados.registradoPor);
            ViewBag.idEstadoEmpleado = new SelectList(db.estadoempleados, "idEstadoEmpleado", "estadoEmpleado", empleados.idEstadoEmpleado);
            ViewBag.idRol = new SelectList(db.roles, "idRol", "rol", empleados.idRol);
            return View(empleados);
        }

        // GET: empleados/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            empleados empleados = await db.empleados.FindAsync(id);
            if (empleados == null)
            {
                return HttpNotFound();
            }
            return View(empleados);
        }

        // POST: empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            empleados empleados = await db.empleados.FindAsync(id);
            db.empleados.Remove(empleados);
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
