namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    [Table("direcciones")]
    public partial class direcciones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public direcciones()
        {
            bases = new HashSet<bases>();
            personas = new HashSet<personas>();
        }

        [Key]
        public int idDireccion { get; set; }

        [Required(ErrorMessage = "¿Cuál es el número en la puerta?")]
        [StringLength(5)]
        [Display(Name = "Número Puerta")]
        public string numeroPuerta { get; set; }

        [StringLength(5)]
        [Display(Name = "Número Edificio")]
        public string numeroEdificio { get; set; }

        [StringLength(100)]
        [Display(Name = "Edificio / Residencial")]
        public string nombreEdificio { get; set; }

        [Required(ErrorMessage = "¿En qué calle?")]
        [StringLength(255)]
        [Display(Name = "Calle")]
        public string calle { get; set; }

        [Required(ErrorMessage="¿En qué ciudad?")]
        [StringLength(100)]
        [Display(Name = "Ciudad / Sector")]
        public string ciudad { get; set; }

        public int idMunicipio { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<bases> bases { get; set; }

        public virtual municipios municipios { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<personas> personas { get; set; }


        //not mapped properties-------------------------------------------------------------

        [NotMapped]
        public SelectList listaProvincias { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "¿En qué provincia?")]
        [Display(Name = "Provincia")]
        public int provinciaSeleccionada { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "¿En qué municipio?")]
        [Display(Name = "Municipio")]
        public int municipioSeleccionado { get; set; }

        [NotMapped]
        public SelectList listaMunicipios { get; set; }
    }
}
