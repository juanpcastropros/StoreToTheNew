using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCore
{
    public class Store: ImageDemostrable
    {

        public Store()
        {
            this.Products = new HashSet<Product>();
        }
        public ICollection<Product> Products { get; set; }
        public string Coordinates { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

    }
}
