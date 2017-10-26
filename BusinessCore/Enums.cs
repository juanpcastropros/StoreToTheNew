using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCore
{
    public enum Categoryy
    {
        Remera,
        [Display(Name = "Pantalónes de Jean")]
        Jean,
        [Display(Name = "Pantalónes de Traje")]
        PantTraje
    }
    public enum AccessLevel
    {
        [Display(Name ="Administrador")]
        Admin,
        [Display(Name = "Dueño de Tienda")]
        Store,
        [Display(Name = "Navegante")]
        Regular
    }

}
