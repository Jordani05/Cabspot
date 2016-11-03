namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("metodopago")]
    public partial class metodopago
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public metodopago()
        {
            carreras = new HashSet<carreras>();
        }

        [Key]
        public int idMEtodoPago { get; set; }

        [Column("metodoPago")]
        [Required]
        [StringLength(15)]
        public string metodoPago { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<carreras> carreras { get; set; }
    }
}
