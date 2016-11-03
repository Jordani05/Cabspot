namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;
    using System.Linq;
    using Cabspot.Controllers.Clases;
    using System.Web.Http;
    using System.Threading.Tasks;
    using System.Data.Entity.SqlServer;
    using System.Net;
    using System.IO;
    using Newtonsoft.Json.Linq;
    using System.Data.Entity;
    [Table("taxistas")]
    public partial class taxistas
    {
        public static CabspotDB db = new CabspotDB();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public taxistas()
        {
            carreras = new HashSet<carreras>();
            solicitudes = new HashSet<solicitudes>();
            vehiculos = new HashSet<vehiculos>();
            notificacionTaxista = new HashSet<notificacionTaxista>();
        }

        [Key]
        public int idTaxista { get; set; }

        public int idPersona { get; set; }

        [Required(ErrorMessage="Debe especificar un código")]
        [StringLength(10)]
        [Column("codigoTaxista")]
        [Display(Name = "Código Taxista")]
        public string codigoTaxista { get; set; }

        [Display(Name = "Rating")]
        public int? rating { get; set; }

        public int? idEstadoDisponibilidad { get; set; }

        public int? idBase { get; set; }

        [Column(TypeName = "date")]
        public DateTime fechaRegistro { get; set; }

        public int? registradoPor { get; set; }

        public double? latitudActual { get; set; }

        public double? longitudActual { get; set; }

        public DateTime? ultimaActualizacionPosicion { get; set; }

        public DateTime? ultimaActualizacionEstado { get; set; }

        [StringLength(255)]
        public string apikey { get; set; }

        public virtual personas personas { get; set; }

        public virtual estadodisponibilidad estadodisponibilidad { get; set; }

        [NotMapped]
        public virtual vehiculos vehiculo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<carreras> carreras { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<solicitudes> solicitudes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<vehiculos> vehiculos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<autenticacionsmstaxista> autenticacionsmstaxista { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<notificacionTaxista> notificacionTaxista { get; set; }

        //not mapped-----------------------------------------------------
        [NotMapped]
        public SelectList listaBases { get; set; }
        [NotMapped]
        [Display(Name = "Base")]
        public string baseSeleccionada { get; set; }


        //buscar al taxista por telefonoMovil
        public static taxistas BuscarPorMovil(string telefonoMovil)
        {
            if (!string.IsNullOrEmpty(telefonoMovil))
            {
                var taxistas = from t in db.taxistas where t.personas.contactos.telefonoMovil.Equals(telefonoMovil) select t;
                if (taxistas.Count() > 0)
                {
                    return taxistas.First();
                }
                return null;
            }
            return null;
        }

        //buscar taxista por codigo
        public static taxistas BuscarPorCodigo(string codigoTaxista)
        {
            if (!string.IsNullOrEmpty(codigoTaxista))
            {
                var taxistas = from t in db.taxistas where t.codigoTaxista.Equals(codigoTaxista) select t;
                if (taxistas.Count() > 0)
                {
                    return taxistas.First();
                }
                return null;
            }
            return null;
        }

        //generar codigo aleatorio para autenticar al taxista
        public static autenticacionsmstaxista generarCodigoTaxista(int idTaxista)
        {
            if (idTaxista > 0)
            {
                taxistas taxista = db.taxistas.Find(idTaxista);
                if (taxista != null)
                {
                    Random random = new Random();
                    autenticacionsmstaxista sms = new autenticacionsmstaxista();

                    int otp = random.Next(100000, 999999);

                    //guardando sms
                    sms.idTaxista = idTaxista;
                    sms.codigo = otp.ToString();

                    try
                    {
                        db.autenticacionSmsTaxista.Add(sms);
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
        
        //crear solicitudes a taxistas cercanos a la ubicacion del cliente
        public static bool solicitudTaxista(carreras carrera)
        {
            //
            var taxistasDisponibles = new List<taxistas>();          
            
            //buscar taxistas en un radio de 5km a la ubicacion del cliente y que estan disponibles
            foreach (taxistas tax in db.taxistas)
            {
                string coordOrigen = tax.latitudActual + "," + tax.longitudActual ;
                string coordDestino = carrera.latitudOrigen +  "," + carrera.longitudOrigen;

                if (Utilidades.getDistance((double)tax.latitudActual, (double)tax.longitudActual, carrera.latitudOrigen, carrera.longitudOrigen) < Constantes.RADIO_DISTANCIA)
                {
                    taxistasDisponibles.Add(tax);
                }
            }
            
            //hay por lo menos un taxista en la zona del cliente
            if (taxistasDisponibles.Count() > 0)
            {
                List<solicitudes> solicitudesNuevas = new List<solicitudes>();

                //crear solicitudes
                foreach (var t in taxistasDisponibles)
                {
                    solicitudes solicitud = new solicitudes();
                    solicitud.idCarrera = carrera.idCarrera;
                    solicitud.fechaSolicitud = DateTime.Now;
                    solicitud.idEstadoSolicitud = 51;  //en espera
                    solicitud.idTaxista = t.idTaxista;

                    solicitudesNuevas.Add(solicitud);                 
                }

                try
                {
                    db.solicitudes.AddRange(solicitudesNuevas);
                    db.SaveChangesAsync();
                    
                    //generar notificaciones
                    crearNotificaciones(taxistasDisponibles, solicitudesNuevas);

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        private static void crearNotificaciones(List<taxistas> taxistasDisponibles, List<solicitudes> solicitudes)
        {
            CabspotDB db = new CabspotDB();
            List<notificacionTaxista> notificaciones = new List<notificacionTaxista>();

            foreach (taxistas t in taxistasDisponibles)
            {
                foreach(solicitudes s in solicitudes){
                    //trama json (idSolicitud y direciones origen y destino)
                    var solicitudesTaxista = db.solicitudes.Where(x => x.idTaxista == t.idTaxista && x.idSolicitud == s.idSolicitud)
                                        .Select(x => new
                                        {
                                            x.idSolicitud,
                                            x.carreras.latitudOrigen,
                                            x.carreras.longitudOrigen,
                                            x.carreras.latitudDestino,
                                            x.carreras.longitudDestino,
                                            x.carreras.metodopago.metodoPago
                                        });

                    if (solicitudesTaxista.Count() > 0)
                    {
                        var solicitud = solicitudesTaxista.First();
                        notificacionTaxista notificacion = new notificacionTaxista();
                        notificacion.idTaxista = t.idTaxista;
                        notificacion.metodoPago = solicitud.metodoPago;
                        notificacion.ubicacionDestino = Utilidades.getAddress(solicitud.latitudDestino, solicitud.longitudDestino);
                        notificacion.ubicacionOrigen = Utilidades.getAddress(solicitud.latitudOrigen, solicitud.longitudOrigen); ;

                        notificaciones.Add(notificacion);
                        break;
                    }                   
               }                
            }

            try
            {
                db.notificacionTaxista.AddRange(notificaciones);
                db.SaveChangesAsync();
            }
            catch (Exception e)
            {

            }
        }
               
        public static List<notificacionTaxista> getNotificacionesTaxista(int idTaxista)
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
                        return null;
                    }

                    //devolver las notificaciones
                    return notificacionesReturn;
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
}
