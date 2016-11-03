namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tipovehiculos")]
    public partial class tipovehiculos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tipovehiculos()
        {
            vehiculos = new HashSet<vehiculos>();
        }

        [Key]
        public int idTipoVehiculo { get; set; }

        [Required]
        [StringLength(15)]
        public string tipoVehiculo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<vehiculos> vehiculos { get; set; }
    }
}
