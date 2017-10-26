using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BusinessCore
{
    public class Product : ImageDemostrable
    {
        public Product()
        { }

        public virtual Store Store { get; set; }
        public decimal Price { get; set; }

        public virtual Category Category { get; set; }
    }
}
