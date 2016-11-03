namespace Cabspot.Models
{
    using Cabspot.Controllers.Clases;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;


    [Table("cabspotdb.clientes")]
    public partial class clientes
    {
        public static CabspotDB db = new CabspotDB();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public clientes()
        {
            carreras = new HashSet<carreras>();
            sugerencias = new HashSet<sugerencias>();
            notificacionCliente = new HashSet<notificacionCliente>();
        }

        [Key]
        public int idCliente { get; set; }

        public int? idPersona { get; set; }

        [StringLength(15)]
        public string nombreUsuario { get; set; }

        [StringLength(255)]
        public string apikey { get; set; }

        [Column(TypeName = "date")]
        public DateTime fechaRegistro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<carreras> carreras { get; set; }

        public virtual personas personas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sugerencias> sugerencias { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<notificacionCliente> notificacionCliente { get; set; }

        //generar codigo aleatorio para autenticar al cliente
        public static autenticacionsms generarCodigoCliente(int idCliente)
        {
            if (idCliente != null)
            {
                clientes cliente = db.clientes.Find(idCliente);
                if (cliente != null)
                {
                    Random random = new Random();
                    autenticacionsms sms = new autenticacionsms();

                    int otp = random.Next(100000, 999999);

                    //guardando sms
                    sms.idClienteMovil = idCliente;
                    sms.codigo = otp.ToString();

                    try
                    {
                        db.autenticacionSms.Add(sms);
                        db.SaveChanges();
                        return sms;

                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }

                return null;

            }
            return null;
        }

        //crear notificacion cuando una carrera es atendida 
        //se envia al cliente la informacion del taxista y del vehiculo, asi como la posicion donde se encuentra y el tiempo 
        //aproximado de recogida
        public static void crearNotificaciones(int idCarrera)
        {
            CabspotDB db = new CabspotDB();
            notificacionCliente notificacion = new notificacionCliente();
            var carrera = db.carreras.Find(idCarrera);
            var taxista = db.taxistas.Find(carrera.idTaxista);
            var vehiculo = taxista.vehiculos.Where(x => x.activo).First();

            //respuesta json a almacenar en bd
            respuestaCarera respuesta = new respuestaCarera();
            
            respuesta.idCarrera = idCarrera;
            respuesta.nombreTaxista = taxista.personas.nombres + " " + taxista.personas.apellidos;
            respuesta.ubicacion = taxista.latitudActual + "," + taxista.longitudActual;
            respuesta.vehiculo = vehiculo.marca + " " + vehiculo.modelo + " " + vehiculo.anio;
            respuesta.colorVehiculo = vehiculo.color;
            
            notificacion.idCliente = (int)carrera.idCliente;
            notificacion.codigoTaxista = taxista.codigoTaxista;
            notificacion.nombreTaxista = taxista.personas.nombres + " " + taxista.personas.apellidos;
            notificacion.ubicacionTaxista = Utilidades.getAddress((double)taxista.latitudActual, (double)taxista.longitudActual);
            notificacion.vehiculo = vehiculo.marca + " " + vehiculo.modelo + " " + vehiculo.anio + " [" + vehiculo.color + "]";
            notificacion.tiempoAproximadoRecogida = Utilidades.getTravelTime((double)carrera.latitudOrigen, (double)carrera.longitudOrigen, (double)taxista.latitudActual, (double)taxista.longitudActual);
            
            try
            {
                db.notificacionCliente.Add(notificacion);
                db.SaveChangesAsync();
            }
            catch (Exception e)
            {

            }

        }

        public static List<notificacionCliente> getNotificaciones(int idCliente)
        {
            //buscar cliente
            CabspotDB db = new CabspotDB();
            clientes cliente = db.clientes.Find(idCliente);

            if (cliente != null)
            {
                //buscar notificaciones para ese cliente
                //var notificaciones = db.notificacionCliente.Where(x => x.idCliente == cliente.idCliente).ToList();
                var notificaciones = from n in db.notificacionCliente where n.idCliente == idCliente && !n.enviada select n;
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
                        return null;
                    }

                    //devolver las notificaciones
                    return notificacionesReturn.ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


    }

    [NotMapped]
    public class clienteNuevo
    {
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string fechaNacimiento { get; set; }
        public string telefonoMovil { get; set; }
        public string identificacion { get; set; }
        public string email { get; set; }
        public string genero { get; set; }
        public string nombreUsuario { get; set; }

        public string apikey { get; set; }
    }


   
}
