using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Cabspot.Models
{
    [Table("notificacionCliente")]
    public partial class notificacionCliente
    {
        [Key]
        public int idNotificacion { get; set; }                
        
        public int idCliente { get; set; }

        public string nombreTaxista { get; set; }

        public string codigoTaxista { get; set; }

        public string vehiculo { get; set; }

        public string ubicacionTaxista { get; set; }

        public int tiempoAproximadoRecogida { get; set; }        

        public bool enviada { get; set; } 
    }
}