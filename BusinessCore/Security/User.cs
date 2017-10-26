using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BusinessCore.Security
{
    public class User: BaseObject
    {
        [Key, Column(Order = 1)]
        public string Name { get; set; }
        [StringLength(10,ErrorMessage = "Invalid PassWord",MinimumLength =8)]
        
        public string Password { get; set; }

        public AccessLevel Level { get; set; }
    }
}
