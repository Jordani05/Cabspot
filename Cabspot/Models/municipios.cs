namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    [Table("municipios")]
    public partial class municipios
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public municipios()
        {
            direcciones = new HashSet<direcciones>();
        }

        [Key]
        public int idMunicipio { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Municipio")]
        public string nombreMunicipio { get; set; }

        public int idProvincia { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<direcciones> direcciones { get; set; }

        public virtual provincias provincias { get; set; }



        //not mapped properties-------------------------------------------------------------

        
    }
}
