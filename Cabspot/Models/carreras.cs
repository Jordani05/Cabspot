namespace Cabspot.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("carreras")]
    [JsonObject(IsReference = true)]
    public partial class carreras
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public carreras()
        {
            comentarios = new HashSet<comentarios>();
            encuestas = new HashSet<encuestas>();
            solicitudes = new HashSet<solicitudes>();
        }

        [Key]
        public int idCarrera { get; set; }

        public int? idTaxista { get; set; }

        public int? idEstado { get; set; }

        public DateTime? fechaInicioCarrera { get; set; }

        public DateTime? fechaFinCarrera { get; set; }

        public TimeSpan? tiempoCarrera { get; set; }

        public int? idCliente { get; set; }

        public DateTime fechaSolicitud { get; set; }

        public double longitudOrigen { get; set; }

        public double longitudDestino { get; set; }

        public double latitudDestino { get; set; }

        public double latitudOrigen { get; set; }

        public int idViaSolicitud { get; set; }

        public int idMetodoPago { get; set; }

        public float? costo { get; set; }

        public virtual taxistas taxistas { get; set; }

        public virtual estadocarreras estadocarreras { get; set; }

        [JsonIgnore]
        public virtual clientes clientes { get; set; }

        public virtual viasolicitud viasolicitud { get; set; }

        public virtual metodopago metodopago { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<comentarios> comentarios { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<encuestas> encuestas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<solicitudes> solicitudes { get; set; }
    }

    [NotMapped]
    public class respuestaCarera
    {
        public int idCarrera {get;set;}
        public string nombreTaxista { get; set; }
        public string foto { get; set; }
        public string ubicacion { get; set; }
        public string vehiculo { get; set; }
        public string colorVehiculo { get; set; }
    }
}
