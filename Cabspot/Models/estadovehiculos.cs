namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("estadovehiculos")]
    public partial class estadovehiculos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public estadovehiculos()
        {
            vehiculos = new HashSet<vehiculos>();
        }

        [Key]
        public int idEstadoVehiculo { get; set; }

        [Required]
        [StringLength(15)]
        [Display(Name = "Estado")]
        public string estadoVehiculo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<vehiculos> vehiculos { get; set; }
    }
}
