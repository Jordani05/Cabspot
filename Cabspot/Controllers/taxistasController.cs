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

namespace Cabspot.Controllers
{
    [Authorize]
    public class taxistasController : Controller
    {
        private CabspotDB db = new CabspotDB();

        // GET: taxistas
        public async Task<ActionResult> Index()
        {
            return View(await db.taxistas.ToListAsync());
        }

        // GET: taxistas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            bool ubicacionDisponible = false;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            taxistas taxistas = await db.taxistas.FindAsync(id);
            if (taxistas == null)
            {
                return HttpNotFound();
            }

            vehiculos v = new vehiculos();
            v.listaCondicion = new SelectList(db.condicionvehiculos, "idCondicionVehiculo", "condicionVehiculo");
            v.listaEstado = new SelectList(db.estadovehiculos, "idEstadoVehiculo", "estadoVehiculo");
            v.listaTipo = new SelectList(db.tipovehiculos, "idTipoVehiculo", "tipoVehiculo");
            taxistas.vehiculo = v;

            //ruta relativa de foto de perfil
            //taxistas.personas.foto = Clases.Utilidades.RutaRelativa(taxistas.personas.foto, "~/Content/Images/");

            //ubicacion disponible
            if (taxistas.idEstadoDisponibilidad == 81 || taxistas.idEstadoDisponibilidad == 91)
            {
                ubicacionDisponible = true;
            }

            ViewBag.ubicacionDisponbile = ubicacionDisponible;
            return View(taxistas);
        }

        // GET: taxistas/Create
        public ActionResult Create()
        {
            taxistas taxista = new taxistas();
            vehiculos v = new vehiculos();
            direcciones d = new direcciones();
            personas p = new personas();
            contactos c = new contactos();
            taxista.listaBases = new SelectList(db.bases, "idBase", "nombreBase");
            //empleado.listaRoles = new SelectList(db.roles, "idRol", "rol");
            d.listaProvincias = new SelectList(db.provincias, "idProvincia", "nombreProvincia");  //enviando el listado de provincias al View
            p.direcciones = d;
            taxista.personas = p;
            //v.listaCondicion = new SelectList(db.condicionvehiculos, "idCondicionVehiculo", "condicionVehiculo");            
            //v.listaEstado= new SelectList(db.estadovehiculos, "idEstadoVehiculo", "estadoVehiculo");
            //v.listaTipo = new SelectList(db.tipovehiculos, "idTipoVehiculo", "tipoVehiculo");
            //taxista.vehiculo = v;

            return View(taxista);
        }

        // POST: taxistas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(taxistas taxistas, HttpPostedFileBase foto, string comando)
        {
            //asignando ids
                try
                {
                    //base
                    taxistas.idBase = int.Parse(taxistas.baseSeleccionada);

                    //creado como NO ACTIVO por default
                    //var idEstadoDisponibilidad = 111;
                    taxistas.idEstadoDisponibilidad = 111;
                    //direccion
                    taxistas.personas.direcciones.idMunicipio = taxistas.personas.direcciones.municipioSeleccionado;

                    //subir foto
                    var filename = Path.GetFileName(foto.FileName);
                    var path = "~/FotosPerfil/" + filename;
                    foto.SaveAs(path);
                    taxistas.personas.foto = path;

                    //unique de cedula
                    if (taxistas.personas.identificacion != null)
                    {
                        taxistas.personas.identificacion = taxistas.personas.identificacion.Replace("-", "").Trim();
                        var cedula = from n in db.taxistas where n.personas.identificacion.Equals(taxistas.personas.identificacion) select n;
                        if (cedula.Count() > 0)
                        {
                            ModelState.AddModelError("personas.identificacion", "Existe una persona con esta cédula");
                        }
                    }
                    //fin unique cedula

                    //unique numero movil
                    if (taxistas.personas.contactos.telefonoMovil != null)
                    {
                        taxistas.personas.contactos.telefonoMovil = taxistas.personas.contactos.telefonoMovil.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim();
                        var movil = from n in db.bases where n.contactos.telefonoMovil.Equals(taxistas.personas.contactos.telefonoMovil) select n;
                        if (movil.Count() > 0)
                        {
                            ModelState.AddModelError("personas.contactos.telefonoMovil", "Este móvil ya está en uso");
                        }
                    }
                    //fin unique nnumero movil

                    //unique email
                    if (taxistas.personas.contactos.email != null)
                    {
                        taxistas.personas.contactos.email = taxistas.personas.contactos.email.Trim();
                        var movil = from n in db.bases where n.contactos.email.Equals(taxistas.personas.contactos.email) select n;
                        if (movil.Count() > 0)
                        {
                            ModelState.AddModelError("personas.contactos.email", "Este móvil ya está en uso");
                        }
                    }
                    //fin unique email

                    //unique codigo
                    if (taxistas.codigoTaxista != null)
                    {
                        taxistas.codigoTaxista = taxistas.codigoTaxista.Trim();
                        var usuario = from n in db.taxistas where n.codigoTaxista.Equals(taxistas.codigoTaxista) select n;
                        if (usuario.Count() > 0)
                        {
                            ModelState.AddModelError("usuario", "El código ya existe");
                        }
                    }

                    //fin unique codigo

                }
                catch (Exception e)
                {
                    taxistas.listaBases = new SelectList(db.bases, "idBase", "nombreBase");
                    taxistas.personas.direcciones.listaProvincias = new SelectList(db.provincias, "idProvincia", "nombreProvincia");  //enviando el listado de provincias al View

                    return View(taxistas);
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        db.taxistas.Add(taxistas);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Details", "taxistas", new { id = taxistas.idTaxista });
                    }
                    catch (Exception e)
                    {
                        TempData["error"] = e.ToString();
                        return Redirect("~/Shared/Error.cshtml");
                    }
                }

                taxistas.listaBases = new SelectList(db.bases, "idBase", "nombreBase", taxistas.baseSeleccionada);
                taxistas.personas.direcciones.listaProvincias = new SelectList(db.provincias, "idProvincia", "nombreProvincia");  //enviando el listado de provincias al View

                return View(taxistas);
            
        }

        // GET: taxistas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            taxistas taxistas = await db.taxistas.FindAsync(id);
            if (taxistas == null)
            {
                return HttpNotFound();
            }
            return View(taxistas);
        }

        // POST: taxistas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idTaxista,idPersona,codigoTaxita,idEstadoDisponibilidad,idBase,fechaRegistro,registradoPor,latitudActual,longitudActual,ultimaActualizacionPosicion")] taxistas taxistas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(taxistas).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(taxistas);
        }

        // GET: taxistas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            taxistas taxistas = await db.taxistas.FindAsync(id);
            if (taxistas == null)
            {
                return HttpNotFound();
            }
            return View(taxistas);
        }

        // POST: taxistas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            taxistas taxistas = await db.taxistas.FindAsync(id);
            db.taxistas.Remove(taxistas);
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


        //agregar vehiculo al taxista antes de su creacion, agreagar a db junto con el taxista
        [HttpPost]
        public async Task<ActionResult> AgregarVehiculo(taxistas taxista)
        {
            //var idTaxista = Request.Form["idTaxista"];
            //asignando ids de estados
            taxista.vehiculo.idCondicionVehiculo = taxista.vehiculo.condicionSeleccionada;
            taxista.vehiculo.idEstadoVehiculo = taxista.vehiculo.estadoSeleccionado;
            taxista.vehiculo.idTipoVehiculo = taxista.vehiculo.tipoSeleccionado;
            taxista.vehiculo.registradoPor = 1;  //mientras tanto

            //unique de chasis y numero de placa
            var chasis = from c in db.vehiculos where c.chasis.Equals(taxista.vehiculo.chasis) select c;
            var placa = from c in db.vehiculos where c.placa.Equals(taxista.vehiculo.placa) select c;

            if (chasis.Count() > 0)
            {
                ModelState.AddModelError("vehiculo.chasis", "Este chasis ya está registrado");
            }

            if (placa.Count() > 0)
            {
                ModelState.AddModelError("vehiculo.placa", "Esta placa ya está registrada");
            }


            try
            {
                taxistas t = db.taxistas.Find(taxista.idTaxista);
                if (t != null)
                {
                    taxista.vehiculo.idTaxista = t.idTaxista;
                    taxista.codigoTaxista = t.codigoTaxista;  //volando ModelState (sin razon aparente)

                    //si es el primer vehiculo que se asigna a este taxista se crea como activo
                    if (t.vehiculos.Count() > 0)
                    {
                        taxista.vehiculo.activo = true;
                    }

                    if (ModelState.IsValid)
                    {
                        db.vehiculos.Add(taxista.vehiculo);
                        await db.SaveChangesAsync();
                        TempData["mensajeAgregarVehiculo"] = "exito";
                        //return View("Details", t);
                        return RedirectToAction("Details", "taxistas", new { id = t.idTaxista });
                    }

                    var errors = ViewData.ModelState.Where(n => n.Value.Errors.Count > 0).ToList();
                    taxista.vehiculo.listaCondicion = new SelectList(db.condicionvehiculos, "idCondicionVehiculo", "condicionVehiculo", taxista.vehiculo.condicionSeleccionada);
                    taxista.vehiculo.listaEstado = new SelectList(db.estadovehiculos, "idEstadoVehiculo", "estadoVehiculo", taxista.vehiculo.estadoSeleccionado);
                    taxista.vehiculo.listaTipo = new SelectList(db.tipovehiculos, "idTipoVehiculo", "tipoVehiculo", taxista.vehiculo.tipoSeleccionado);
                    TempData["mensajeAgregarVehiculo"] = "error";
                    return View("Details", taxista);
                }
                
            }
            catch (Exception e)
            {
                TempData["mensajeAgregarVehiculo"] = "error";
                return RedirectToAction("Details", "taxistas", new { id = taxista.idTaxista });
            }

            return RedirectToAction("Index", "taxistas");
            
        }




    }
}
