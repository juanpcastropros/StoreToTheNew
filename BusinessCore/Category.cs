using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCore
{
    public class Category: Demostrable
    {
        public Category()
        {
            this.Stores = new HashSet<Store>();
        }

        public virtual ICollection<Store> Stores{ get; set; }

    }
}
