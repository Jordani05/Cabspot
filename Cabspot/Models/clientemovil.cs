using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Cabspot.Models
{
    [Table("cabspotdb.clientemovil")]
    public partial class clientemovil
    {
        [Key]
        public int idCliente { get; set; }
    }
}