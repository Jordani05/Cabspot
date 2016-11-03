namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("comentarios")]
    public partial class comentarios
    {
        [Key]
        public int idComentario { get; set; }

        public int? idCarrera { get; set; }

        [Column(TypeName = "date")]
        public DateTime fechaComentario { get; set; }

        [Required]
        [StringLength(50)]
        public string comentario { get; set; }

        public virtual carreras carreras { get; set; }
    }
}
