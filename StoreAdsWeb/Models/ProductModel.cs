using BusinessCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreAdsWeb.Models
{
    public class ProductModel
    {
        private Product item;

        public Product Item
        {
            get { return item; }
            set { item = value; }
        }
        private string categoryID;

        public string CategoryID
        {
            get { return categoryID; }
            set { categoryID = value; }
        }
        private string storeID;

        public string StoreID
        {
            get { return storeID; }
            set { storeID = value; }
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price{ get; set; }

    }
}