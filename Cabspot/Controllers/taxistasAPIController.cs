using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Cabspot.Models;
using System.Web.Mvc;
using Cabspot.Controllers.Clases;
using Twilio;
using Cabspot.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Cabspot.Controllers
{
    public class taxistasAPIController : ApiController
    {
        private CabspotDB db = new CabspotDB();

        //login -------------------------------------------------------------------------
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("taxistas/login/{codigoTaxista}/{apikey}")]
        public bool loginTaxista(string codigoTaxista, string apikey)
        {
            //variables de twilio
            string AccountSid = Constantes.ACCOUNT_SID_CABSPOT;
            string AuthToken = Constantes.AUTH_TOKEN_CABSPOT;
            var twilio = new TwilioRestClient(AccountSid, AuthToken);

            //buscar el taxista que tiene ese codigoTaxista
            if (!string.IsNullOrEmpty(codigoTaxista) && !string.IsNullOrEmpty(apikey))
            {
                var taxistasCodigo = (from t in db.taxistas where t.codigoTaxista.Equals(codigoTaxista) select t);   //taxistas.BuscarPorCodigo(codigoTaxista);
                taxistas taxistaLogin = null;
                if(taxistasCodigo.Count() > 0)
                {
                    taxistaLogin = taxistasCodigo.First();
                    taxistaLogin.apikey = apikey;
                    db.Entry(taxistaLogin).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    return false;
                }
                


                if (taxistaLogin != null)
                {
                    //enviar mensaje de texto 
                    autenticacionsmstaxista sms = taxistas.generarCodigoTaxista(taxistaLogin.idTaxista);
                    if (sms != null)
                    {
                        try
                        {
                            //formato EI64 para el numero
                            var numero = contactos.FormatearCelular(taxistaLogin.personas.contactos.telefonoMovil);  
                            //si el numero no esta en el formato correcto SALIR
                            if (numero == null)
                            {
                                return false;
                            }
                            //enviando mensaje
                            var message = twilio.SendMessage(Constantes.PHONE_CABSPOT, numero, Constantes.Mensaje_Codigo + sms.codigo);

                            //guardando apikey
                            try
                            {
                                //actualizar entidad
                                taxistaLogin.apikey = apikey;
                                db.Entry(taxistaLogin).State = EntityState.Modified;

                                db.SaveChanges();
                            }
                            catch (Exception e) { }



                            //respuesta si el mensaje fue enviado o no
                            if (!string.IsNullOrEmpty(message.Sid))
                                return true;
                            return false;
                        }
                        catch (Exception e)
                        {
                            return false;
                        }
                    }
                    return false;
                    
                }
                return false; 
            }
            return false;          
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("taxistas/autenticar/{codigoVerificacion}")]
        [ResponseType(typeof(taxistas))]
        public IHttpActionResult autenticarTaxista(string codigoVerificacion)
        {
            if (!string.IsNullOrEmpty(codigoVerificacion))
            {
                //buscar codigo en tabla autenticacionsmstaxista
                autenticacionsmstaxista sms = new autenticacionsmstaxista();// = autenticacionsmstaxista.getAutenticacionSMS(codigoVerificacion);
                
                var listaSMS = from l in db.autenticacionSmsTaxista where l.codigo.Equals(codigoVerificacion) select l;
                if (listaSMS.Count() > 0)
                {
                    sms = listaSMS.FirstOrDefault();
                }
                

                if (sms != null)
                {
                    //revisar codigo no ha sido verificado
                    if (!sms.verificado)
                    {
                        //buscar taxista
                        taxistas taxista = db.taxistas.Find(sms.idTaxista);
                        
                        if (taxista != null)
                        {
                            //cambio de verificado en DB
                            sms.verificado = true;
                            var vehiculos = taxista.vehiculos.Where(v => v.idEstadoVehiculo == 31);

                            //cambiando estado del taxista a disponible si tiene por lo menos un vehiculo activo
                            if (vehiculos.Count() > 0 && taxista.idEstadoDisponibilidad != 81)
                            {
                                taxista.idEstadoDisponibilidad = 81;
                            }
                                                        
                            try
                            {
                                //actualizando en bd
                                db.Entry(sms).State = EntityState.Modified;
                                db.Entry(taxista).State = EntityState.Modified;
                                db.SaveChanges();
                                //devolver taxista
                                //return Ok(taxista);
                                return getVehiculos(taxista.idTaxista);  //Ok(vehiculosEnviar);
                            }
                            catch (Exception e)
                            {
                                return BadRequest();
                            }
                            
                        }
                        else
                        {
                            return BadRequest();
                        }                        
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return NotFound();
                }                
            }
            else
            {
                return BadRequest();
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("vehiculos/getVehiculos/{idTaxista}")]
        public IHttpActionResult getVehiculos(int idTaxista)
        {
                taxistas taxista = db.taxistas.Find(idTaxista);

                if (taxista != null)
                {
                    var vehiculos = taxista.vehiculos;

                    //enviar vehiculos con estado activo para posterior elección
                    var vehiculosEnviar = from v in vehiculos where v.idEstadoVehiculo == 31 select new { v.idTaxista,v.idVehiculo, v.marca, v.modelo, v.serie, v.placa, v.anio, v.color, v.unidad, v.tipovehiculos.tipoVehiculo };
                    return Ok(vehiculosEnviar);
                }
                else
                {
                    return BadRequest();
                }  
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("vehiculos/activacion/{idVehiculo}")]
        public bool elegirVehiculo(int idVehiculo)
        {
            //buscar vehiculo
            vehiculos vehiculo = db.vehiculos.Find(idVehiculo);
            if (vehiculo == null)
            {
                return false;
            }

            //en caso que el vehiculo ya esta activo
            if (vehiculo.activo)
            {
                return true;
            }

            //sino esta activo

            //buscar todos los vehiculos de ese taxista y deshabilitar
            var todos = db.vehiculos.Where(t => t.idTaxista == vehiculo.idTaxista);
            foreach (var v in todos)
            {
                v.activo = false;
                db.Entry(v).State = EntityState.Modified;
            }

            //activar el seleccionado
            vehiculo.activo = true;
            db.Entry(vehiculo).State = EntityState.Modified;


            //guardar cambios
            try
            {
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            
//            return true;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("taxistas/actualizarEstado")]
        public void actualizarEstado(taxistas taxista)
        {
            if(taxista.idTaxista > 0)
            {
                taxistas taxistaBD = db.taxistas.Find(taxista.idTaxista);
                if (taxistaBD != null)   //si el taxista esta activo
                {
                    List<estadodisponibilidad> estados = db.estadodisponibilidad.ToList();
                    estadodisponibilidad est = db.estadodisponibilidad.Find(taxista.idEstadoDisponibilidad);

                    if(est != null)
                    {
                        taxistaBD.estadodisponibilidad = est;
                        taxistaBD.idEstadoDisponibilidad = est.idEstadoDisponibilidad;
                        taxistaBD.ultimaActualizacionEstado = DateTime.Now;
                        try
                        {
                            //actualizar entidad
                            db.Entry(taxistaBD).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch (Exception e) { }

                    }
                }
            }
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("taxistas/actualizarUbicacion")]
        public void actualizarUbicacion(taxistas taxistas)
        {
            
           //verificar que el id no sea nulo y que exista
            if (taxistas.idTaxista > 0)
            {
                taxistas taxistaBD = db.taxistas.Find(taxistas.idTaxista);
                if (taxistaBD != null && taxistaBD.idEstadoDisponibilidad != 111)   //si el taxista esta activo
                {
                    //verificar que longitud y latitud no sea nulos
                    if (taxistas.longitudActual != null && taxistas.latitudActual != null)
                    {
                        //verificar formato de posicion
                        string patron = @"^(\-?\d+\.\d+)";
                        var validarLat = Regex.Match(taxistas.latitudActual.ToString(), patron);
                        var validarLong = Regex.Match(taxistas.longitudActual.ToString(), patron);

                        if (validarLat.Success && validarLong.Success)
                        {
                            //actualizar ubicacion taxista
                            taxistaBD.latitudActual = taxistas.latitudActual;
                            taxistaBD.longitudActual = taxistas.longitudActual;

                            taxistaBD.ultimaActualizacionPosicion = DateTime.Now;

                            try
                            {
                                //actualizar entidad
                                db.Entry(taxistaBD).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            catch (Exception e) { }
                        }
                    }
                }
            }           
        }



        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("taxistas/getDatos/{idTaxista}")]
        public IHttpActionResult getDatos(int idTaxista)
        {
            taxistas taxista = db.taxistas.Find(idTaxista);

            if (taxista != null)
            {
                var t = from x in db.taxistas 
                        where x.idTaxista == idTaxista 
                        select new { x.idTaxista, x.codigoTaxista, x.personas.nombres, x.personas.apellidos, x.personas.foto };
                
                if (t.Count() == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(t.First());
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("taxistas/getPerfil/{idTaxista}")]
        public IHttpActionResult getPerfil(int idTaxista)
        {
            taxistas taxista = db.taxistas.Find(idTaxista);

            if(taxista != null)
            {
                var t = from x in db.taxistas 
                        where x.idTaxista == idTaxista 
                        select new {
                                        //informacion basica
                                        x.idTaxista, x.codigoTaxista, x.personas.nombres, x.personas.apellidos, x.personas.foto, x.personas.identificacion, x.personas.fechaNacimiento, x.personas.sexo,
                                        x.rating, x.personas.nacionalidad,
                                        //direccion
                                        x.personas.direcciones.nombreEdificio, x.personas.direcciones.numeroPuerta, x.personas.direcciones.calle, x.personas.direcciones.numeroEdificio, 
                                        x.personas.direcciones.municipios.nombreMunicipio, x.personas.direcciones.municipios.provincias.nombreProvincia,
                                        //contactos
                                        x.personas.contactos.telefonoMovil            
                                    };
                if (t.Count() == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(t.First());
                }
            }
            else
            {
                return BadRequest();
            }                        
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("taxistas/getCarreras/{idTaxista}")]
        public IHttpActionResult getCarreras(int idTaxista)
        {
           var carreras = db.carreras.Where(x => x.idTaxista == idTaxista);
            
           return Ok(carreras);           
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("taxistas/responderSolicitud")]
        public IHttpActionResult responderSolicitud(RespuestaSolicitud respuesta)
        {
            //buscar solicitud
            if (respuesta.idSolicitud > 0)
            {
                solicitudes solicitud = db.solicitudes.Find(respuesta.idSolicitud);

                if (solicitud != null)
                {
                    if (respuesta.respuesta)
                    {
                        //solicitud es aceptada
                        carreras carrera = db.carreras.Find(solicitud.idCarrera);
                        taxistas taxista = db.taxistas.Find(respuesta.idTaxista);
                        if (carrera != null || taxista != null)
                        {
                            //buscar que la carrera no haya sido aceptada por nadie
                            if (carrera.idTaxista == null)
                            {
                                //ELIMINAR todas las demas solicitudes para esta carrera
                                var solicitudesCarrera = db.solicitudes.Where(x => x.idCarrera == carrera.idCarrera && x.idSolicitud != solicitud.idSolicitud);
                                db.solicitudes.RemoveRange(solicitudesCarrera);
                                                                
                                //aceptar solicitud
                                if (respuesta.idTaxista == solicitud.idTaxista)
                                {
                                    solicitud.idEstadoSolicitud = 41;   //aceptada;
                                    carrera.idTaxista = respuesta.idTaxista;
                                    carrera.fechaInicioCarrera = DateTime.Now;
                                    carrera.idEstado = 71;  //en curso
                                    db.Entry(solicitud).State = EntityState.Modified;

                                    string msj;
                                    //ClientesAPIController clientes = new ClientesAPIController();                                  

                                    //msj = clientes.getNotificacionesCliente((int)carrera.idCliente).ToString();
                                    List<notificacionCliente> notificaciones = clientes.getNotificaciones((int)carrera.idCliente);
                                    var json = JsonConvert.SerializeObject(notificaciones);
                                    Push envios = new Push(json);
                                    envios.EnviarClientes((int)carrera.idCliente);
                                }
                                else
                                {
                                    return BadRequest("Esta solicitud no fue hecha para usted");
                                }

                                //guardar cambios
                                try
                                {
                                    db.SaveChangesAsync();

                                    //creando notificacion para cliente
                                    clientes.crearNotificaciones(idCarrera: carrera.idCarrera);

                                    return Ok("Su solicitud ha sido aceptada");
                                }
                                catch(Exception e)
                                {
                                    return BadRequest("No se ha podido guardar cambios");
                                }
                            }
                            else
                            {
                                return BadRequest("Esta carrera ya esta siendo atendida");
                            }
                        }
                        else
                        {
                            return BadRequest("Esta carrera no existe");
                        }
                        
                    }
                    else  //solicitud es rechazada
                    {
                        solicitud.idEstadoSolicitud = 31;//Rechazada
                        db.Entry(solicitud).State = EntityState.Modified;

                        try
                        {
                            db.SaveChangesAsync();
                            return Ok("Solicitud rechazada");
                        }
                        catch
                        {
                            return BadRequest("No se ha podido guardar el cambio");
                        }
                    }
                }
                else
                {
                    return BadRequest("La solicitud no existe");
                }
            }
            else
            {
                return BadRequest("La solicitud no existe");
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("taxistas/getNotificaciones/{idTaxista}")]
        public IHttpActionResult getNotificacionesTaxista(int idTaxista)
        {
            //buscar taxista
            taxistas taxista = db.taxistas.Find(idTaxista);

            if (taxista != null)
            {
                //buscar notificaciones para ese cliente
                var notificacionesUpdate = from n in db.notificacionTaxista where n.idTaxista == idTaxista && !n.enviada select n;
                List<notificacionTaxista> notificacionesReturn = new List<notificacionTaxista>();

                if (notificacionesUpdate.Count() > 0)
                {
                    //marcar notificaciones como leidas
                    try
                    {
                        foreach (var n in notificacionesUpdate)
                        {
                            n.enviada = true;
                            db.Entry(n).State = EntityState.Modified;
                            notificacionesReturn.Add(n);
                        }

                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return BadRequest("Ha ocurrido un error");
                    }

                    //devolver las notificaciones
                    return Ok(notificacionesReturn.ToList());
                }
                else
                {
                    return Ok("No tiene notificaciones sin leer");
                }
            }
            else
            {
                return BadRequest("El taxista no existe");
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("taxistas/notificacionleida/{idNotificacion}")]
        public IHttpActionResult notificacionLeida(int idNotificacion)
        {
            if (idNotificacion > 0)
            {
                notificacionTaxista notificacion = db.notificacionTaxista.Find(idNotificacion);
                if (notificacion != null)
                {
                    try
                    {
                        db.notificacionTaxista.Remove(notificacion);
                        db.SaveChanges();

                        return Ok();
                    }
                    catch (Exception e)
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }


        //--------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------



        public IQueryable<taxistas> Gettaxistas()
        {
            return db.taxistas;
        }

        // GET: api/taxistasAPI/5
        [ResponseType(typeof(taxistas))]
        public IHttpActionResult Gettaxistas(int id)
        {
            taxistas taxistas = db.taxistas.Find(id);
            if (taxistas == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(taxistas);
            }
        }

        // PUT: api/taxistasAPI/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Puttaxistas(int id, taxistas taxistas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != taxistas.idTaxista)
            {
                return BadRequest();
            }

            db.Entry(taxistas).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!taxistasExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/taxistasAPI
        [ResponseType(typeof(taxistas))]
        public async Task<IHttpActionResult> Posttaxistas(taxistas taxistas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.taxistas.Add(taxistas);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = taxistas.idTaxista }, taxistas);
        }

        // DELETE: api/taxistasAPI/5
        [ResponseType(typeof(taxistas))]
        public async Task<IHttpActionResult> Deletetaxistas(int id)
        {
            taxistas taxistas = await db.taxistas.FindAsync(id);
            if (taxistas == null)
            {
                return NotFound();
            }

            db.taxistas.Remove(taxistas);
            await db.SaveChangesAsync();

            return Ok(taxistas);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool taxistasExists(int id)
        {
            return db.taxistas.Count(e => e.idTaxista == id) > 0;
        }        
    }
}
