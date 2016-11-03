namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    [Table("provincias")]
    public partial class provincias
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public provincias()
        {
            municipios = new HashSet<municipios>();
        }

        [Key]
        public int idProvincia { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name="Provincia")]
        public string nombreProvincia { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<municipios> municipios { get; set; }


        
    }
}
