using BusinessCore;
using BusinessCore.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreAdsWeb.Helper
{
    public class Helper
    {
        public static IEnumerable<System.Web.Mvc.SelectListItem> getCategoriesToDropDown()
        {
            STTContext db = new STTContext();
            var list = db.Categories.ToList();
            //return null;
            List<System.Web.Mvc.SelectListItem> rt = new List<System.Web.Mvc.SelectListItem>();
            foreach(Category c in list)
            {
                rt.Add(new System.Web.Mvc.SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            }
            return rt;

        }
        public static IEnumerable<System.Web.Mvc.SelectListItem> getStoresToDropDown()
        {
            STTContext db = new STTContext();
            var list = db.Stores.ToList();
            //return null;
            List<System.Web.Mvc.SelectListItem> rt = new List<System.Web.Mvc.SelectListItem>();
            foreach (Store c in list)
            {
                rt.Add(new System.Web.Mvc.SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            }
            return rt;

        }

    }
}