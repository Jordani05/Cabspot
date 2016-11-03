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
using Twilio;
using Cabspot.Controllers.Clases;
using System.Text.RegularExpressions;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;

namespace Cabspot.Controllers
{
    public class ClientesAPIController : ApiController
    {
        CabspotDB db = new CabspotDB();


        //login
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("clientes/login/{telefonoMovil}/{apikey}")]
        public bool loginCliente(string telefonoMovil, string apikey)
        {
            //telefono con formato E.164  ej. +1809453123
            string telefonoFormat = "";

            //variables de twilio
            string AccountSid = Constantes.ACCOUNT_SID_CABSPOT;
            string AuthToken = Constantes.AUTH_TOKEN_CABSPOT;
            var twilio = new TwilioRestClient(AccountSid, AuthToken);


            if (!string.IsNullOrEmpty(telefonoMovil) && !string.IsNullOrEmpty(apikey))
            {
                //verificar que es un numero en formato correcto ---------------------
                telefonoFormat = contactos.FormatearCelular(telefonoMovil);
                if (telefonoFormat != null)
                {

                    //buscar al cliente en clientesMovil
                    var clientes = db.clientesMovil.Where(x => x.telefonoMovil.Equals(telefonoMovil));

                    //verificar si existe un cliente con ese numero movil
                    if (clientes.Count() > 0)
                    {
                        //si existe enviar mensaje de texto
                        clientesMovil cliente = clientes.First();

                        cliente.apikey = apikey;
                        db.Entry(cliente).State = EntityState.Modified;
                        db.SaveChanges();
                        //enviar mensaje de texto
                        return clientesMovil.enviarMensajeTexto(cliente);

                    }
                    else
                    {
                        //sino existe crear el clienteMovil
                        clientesMovil clientePrimeraVez = new clientesMovil();
                        clientePrimeraVez.fechaRegistro = DateTime.Now;
                        clientePrimeraVez.telefonoMovil = telefonoMovil;
                        clientePrimeraVez.apikey = apikey;

                        try
                        {
                            //guardar el cliente
                            db.clientesMovil.Add(clientePrimeraVez);
                            db.SaveChanges();

                            //buscar este cliente recien guardado
                            var clientesMoviles = db.clientesMovil.Where(x => x.telefonoMovil.Equals(telefonoMovil));

                            if (clientesMoviles.Count() > 0)
                            {
                                clientesMovil clienteMovil = clientesMoviles.First();

                                //enviar mensaje de texto
                                return clientesMovil.enviarMensajeTexto(clienteMovil);
                            }
                        }
                        catch (Exception e)
                        {
                            return false;
                        }
                    }
                }

                return false;
            }
            return false;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("clientes/autenticar/{codigoVerificacion}")]
        public IHttpActionResult autenticarCliente(string codigoVerificacion)
        {
            if (!string.IsNullOrEmpty(codigoVerificacion))
            {
                //buscar codigo en BD
                autenticacionsms sms = new autenticacionsms();
                var listaSMS = db.autenticacionSms.Where(x => x.codigo.Equals(codigoVerificacion));
                if (listaSMS.Count() > 0)
                {
                    sms = listaSMS.First();
                }

                //verificar el mensaje se ha enviado
                if (sms != null)
                {
                    //verificar que no haya sido verificado ya
                    if (!sms.verificado)
                    {
                        sms.verificado = true;
                        var clientes = db.clientes.Where(x => x.personas.contactos.telefonoMovil.Equals(sms.clientesMovil.telefonoMovil));
                        var clientesMovil = db.clientesMovil.Where(x => x.telefonoMovil.Equals(sms.clientesMovil.telefonoMovil));

                        clienteRetorno cr = new clienteRetorno();
                        //cr.autenticado = true;

                           
                        if(clientesMovil.Count() > 0)
                        {
                            var clienteMovil = clientesMovil.First();
                            //si es un cliente antiguo 
                            if (clientes.Count() > 0)
                            {
                                var cliente = clientes.First();                               

                                cliente.apikey = clienteMovil.apikey;

                                cr.antiguedadCliente = "Existente";
                                cr.idCliente = clientes.First().idCliente;

                                try
                                {
                                    db.Entry(sms).State = EntityState.Modified;
                                    db.Entry(cliente).State = EntityState.Modified;
                                    db.SaveChanges();
                                    

                                }
                                catch (Exception e)
                                {
                                    return BadRequest();
                                }
                            }
                            //si es un cliente nuevo
                            else
                            {
                                cr.antiguedadCliente = "Nuevo";
                                cr.apikey = clienteMovil.apikey;
                            }

                            return Ok(cr);

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

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("clientes/create")]
        [ResponseType(typeof(clientes))]
        public async Task<IHttpActionResult> crearCliente(clienteNuevo cliente)
        {
            //verificar ningun elemento sea nulo
            if (!string.IsNullOrEmpty(cliente.nombres) || !string.IsNullOrEmpty(cliente.apellidos) || cliente.fechaNacimiento != null ||
                !string.IsNullOrEmpty(cliente.telefonoMovil) || !string.IsNullOrEmpty(cliente.identificacion) || !string.IsNullOrEmpty(cliente.email) 
                || !string.IsNullOrEmpty(cliente.genero) || !string.IsNullOrEmpty(cliente.apikey))
            {
                //crear objeto cliente y agregar campos
                clientes clienteAgregar = new clientes();
                personas persona = new personas();
                contactos contacto = new contactos();

                persona.nombres = cliente.nombres;
                persona.apellidos = cliente.apellidos;
                persona.fechaNacimiento = DateTime.ParseExact(cliente.fechaNacimiento, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                persona.identificacion = cliente.identificacion;
                persona.sexo = cliente.genero;
                contacto.telefonoMovil = cliente.telefonoMovil;
                contacto.email = cliente.email;
                persona.contactos = contacto;

                //refiriendonos a la direccion por default
                //sera cambiada una vez el usuario pueda entrar a la app movil y editar su perfil
                persona.idDireccion = 0;
                
                clienteAgregar.personas = persona;        
                clienteAgregar.apikey = cliente.apikey;


                if (!string.IsNullOrEmpty(cliente.nombreUsuario))
                {
                    clienteAgregar.nombreUsuario = cliente.nombreUsuario;
                }

                clienteAgregar.fechaRegistro = DateTime.Now;

                if (ModelState.IsValid)
                {
                    //agregar a bd
                    try
                    {
                        db.clientes.Add(clienteAgregar);
                        await db.SaveChangesAsync();

                        //devolvemos ok junto al idCliente para llamar otras acciones desde movil
                        return Ok(clienteAgregar.idCliente);
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
             
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("clientes/getDatos/{idCliente}")]
        public IHttpActionResult getDatos(int idCliente)
        {
            clientes cliente = db.clientes.Find(idCliente);

            Notifications.generarNotificaciones();

            if (cliente != null)
            {
                var c = from x in db.clientes
                        where x.idCliente == idCliente
                        select new { x.idCliente, x.personas.nombres, x.personas.apellidos, x.personas.foto, x.personas.contactos.telefonoMovil };

                if (c.Count() == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(c.First());
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("clientes/getPerfil/{idCliente}")]
        public IHttpActionResult getPerfil(int idCliente)
        {
            clientes cliente = db.clientes.Find(idCliente);

            if (cliente != null)
            {
                var c = from x in db.clientes
                        where x.idCliente == idCliente
                        select new
                        {
                            //informacion basica
                            x.idCliente,
                            x.personas.nombres,
                            x.personas.apellidos,
                            x.personas.foto,
                            x.personas.identificacion,
                            x.personas.fechaNacimiento,
                            x.personas.sexo,
                            x.nombreUsuario,
                            x.personas.nacionalidad,
                            //direccion
                            x.personas.direcciones.nombreEdificio,
                            x.personas.direcciones.numeroPuerta,
                            x.personas.direcciones.calle,
                            x.personas.direcciones.numeroEdificio,
                            x.personas.direcciones.municipios.nombreMunicipio,
                            x.personas.direcciones.municipios.provincias.nombreProvincia,
                            //contactos
                            x.personas.contactos.telefonoMovil
                        };
                if (c.Count() == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(c.First());
                }
            }
            else
            {
                return BadRequest();
            }   
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("clientes/carreraNueva")]
        [ResponseType(typeof(clientes))]
        public async Task<IHttpActionResult> crearCarrera(carreras carrera)
        {
            //verificar elementos nulos
            if(carrera.idCliente > 0 &&   //cliente
               carrera.longitudOrigen != null && carrera.latitudOrigen != null &&  //origen
               carrera.longitudDestino != null && carrera.latitudDestino != null && //destino
               carrera.idMetodoPago > 0 && carrera.idViaSolicitud > 0)   //metodo pago / via solicitud
            {
                //verificar formato de posicion
                string patron = @"^(\-?\d+\.\d+)";
                var validarLatOrigen = Regex.Match(carrera.latitudOrigen.ToString(), patron);
                var validarLongOrigen = Regex.Match(carrera.longitudOrigen.ToString(), patron);
                var validarLatDest = Regex.Match(carrera.latitudDestino.ToString(), patron);
                var validarLongDest = Regex.Match(carrera.longitudDestino.ToString(), patron);

                if (validarLatOrigen.Success && validarLatOrigen.Success && validarLatDest.Success && validarLongDest.Success)
                {
                    //buscar cliente
                    clientes cliente = db.clientes.Find(carrera.idCliente);

                    if (cliente != null)
                    {
                        carrera.idEstado = 41; // Creada como carrear "En Espera"
                        carrera.fechaSolicitud = DateTime.Now;

                        if (ModelState.IsValid)
                        {
                            //agregar a bd
                            try
                            {
                                db.carreras.Add(carrera);
                                await db.SaveChangesAsync();

                                //crear las solicitudes a los taxistas
                                if (taxistas.solicitudTaxista(carrera))
                                {
                                    //string msj;
                                    //msj = carrera.ToString();
                                    //Push envios = new Push(msj);
                                    //envios.EnviarTodosTaxistas();

                                    var json = JsonConvert.SerializeObject(carrera);
                                   
                                    Push envios = new Push(json);
                                    envios.EnviarTodosTaxistas();

                                    return Ok("Su solicitud esta siendo atendida.  Gracias.");
                                }
                                else
                                {
                                    //borrar la carrera y las solicitudes
                                    var solicitudesCarrera = db.solicitudes.Where(x => x.idCarrera == carrera.idCarrera);

                                    if (solicitudesCarrera.Count() > 0)
                                    {
                                        db.solicitudes.RemoveRange(solicitudesCarrera);
                                    }  

                                    db.carreras.Remove(carrera);                                                                    
                                                                        
                                    db.SaveChanges();
                                    //fin borrado

                                    return BadRequest("Ha ocurrido un error y no se ha podido crear la carrera.  Intente de nuevo.");
                                }

                            }
                            catch (Exception e)
                            {
                                return BadRequest("Ha ocurrido un error y no se ha podido crear la carrera.  Intente de nuevo.");
                            }
                        }
                        else
                        {
                            return BadRequest("Los datos proporcionados no son correctos");
                        }
                    }
                    else
                    {
                        return BadRequest("El cliente no existe");
                    }
                }
                else
                {
                    return BadRequest("Ubicación origen y/o destino incorrecta(s)");
                }
            }
            else{
                return BadRequest("No todos los datos necesarios han sido proporcionados");
            }

        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("clientes/getNotificaciones/{idCliente}")]
        public IHttpActionResult getNotificacionesCliente(int idCliente)
        {
            //buscar cliente
            clientes cliente = db.clientes.Find(idCliente);

            if (cliente != null)
            {
                //buscar notificaciones para ese cliente
                //var notificaciones = db.notificacionCliente.Where(x => x.idCliente == cliente.idCliente).ToList();
                var notificaciones= from n in db.notificacionCliente where n.idCliente == idCliente && !n.enviada select n;
                //var notificacionesReturn = from n in db.notificacionCliente where n.idCliente == idCliente && !n.enviada select n.tramaJson;
                List<notificacionCliente> notificacionesReturn = new List<notificacionCliente>();

                if (notificaciones.Count() > 0)
                {
                    //marcar notificaciones como leidas
                    try
                    {
                        foreach (var n in notificaciones)
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
                return BadRequest("El cliente no existe");
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("clientes/notificacionleida/{idNotificacion}")]
        public IHttpActionResult notificacionLeida(int idNotificacion)
        {
            if (idNotificacion > 0)
            {
                notificacionCliente notificacion = db.notificacionCliente.Find(idNotificacion);
                if (notificacion != null)
                {
                    try
                    {
                        db.notificacionCliente.Remove(notificacion);
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

        // GET: api/ClientesAPI
        public IQueryable<clientes> Getclientes()
        {
            // Find your Account Sid and Auth Token at twilio.com/user/account
            string AccountSid = "AC44ec8cb4a1f64fbaf319c8631d9ad15b";
            string AuthToken = "e4b0d062616e0d65e9bc155b829b816a";

            var twilio = new TwilioRestClient(AccountSid, AuthToken);
            var message = twilio.SendMessage("+19179831394", "+18297596854", "Cabspot en accion", "");

            Console.WriteLine(message.Sid);
            return db.clientes;
        }

        // GET: api/ClientesAPI/5
        [ResponseType(typeof(clientes))]
        public async Task<IHttpActionResult> Getclientes(int id)
        {
            clientes clientes = await db.clientes.FindAsync(id);
            if (clientes == null)
            {
                return NotFound();
            }

            return Ok(clientes);
        }

        // PUT: api/ClientesAPI/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putclientes(int id, clientes clientes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != clientes.idCliente)
            {
                return BadRequest();
            }

            db.Entry(clientes).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!clientesExists(id))
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

        // POST: api/ClientesAPI
        [ResponseType(typeof(clientes))]
        public async Task<IHttpActionResult> Postclientes(clientes clientes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.clientes.Add(clientes);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = clientes.idCliente }, clientes);
        }

        // DELETE: api/ClientesAPI/5
        [ResponseType(typeof(clientes))]
        public async Task<IHttpActionResult> Deleteclientes(int id)
        {
            clientes clientes = await db.clientes.FindAsync(id);
            if (clientes == null)
            {
                return NotFound();
            }

            db.clientes.Remove(clientes);
            await db.SaveChangesAsync();

            return Ok(clientes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool clientesExists(int id)
        {
            return db.clientes.Count(e => e.idCliente == id) > 0;
        }
    }
}