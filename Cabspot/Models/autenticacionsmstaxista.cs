using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
namespace Cabspot.Models
{
    [Table("cabspotdb.autenticacionsmstaxista")]
    public partial class autenticacionsmstaxista
    {

        [Key]
        public int idSms { get; set; }

        public int idTaxista { get; set; }

        [StringLength(15)]
        public string codigo { get; set; }

        public bool verificado { get; set; }

        public virtual taxistas taxistas { get; set; }


        //buscar codigo de verificacion
        public static autenticacionsmstaxista getAutenticacionSMS(string codigoVerificacion)
        {
            CabspotDB db = new CabspotDB();

            if (!string.IsNullOrEmpty(codigoVerificacion))
            {
                var listaSMS = from l in db.autenticacionSmsTaxista where l.codigo.Equals(codigoVerificacion) select l;
                if (listaSMS.Count() > 0)
                {
                    return listaSMS.FirstOrDefault();
                }
            }
            return null;
        }

    }
}