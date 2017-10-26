using BusinessCore;
using BusinessCore.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace BusinessCore.Context
{
    public class STTContext : DbContext
    {
        public STTContext() : base("name=STTContext")
        {
        }

        public STTContext(string connString) : base(connString)
        {
        }

        //public System.Data.Entity.DbSet<Ad> Ads { get; set; }
        public System.Data.Entity.DbSet<Category> Categories { get; set; }
        public System.Data.Entity.DbSet<Store> Stores { get; set; }

        public System.Data.Entity.DbSet<Product> Products { get; set; }
        public System.Data.Entity.DbSet<User> Users { get; set; }

        public System.Data.Entity.DbSet<UserStore> UsersStore { get; set; }
        public System.Data.Entity.DbSet<UserNavigation> UserNavigation { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<STTContext>(null);
            base.OnModelCreating(modelBuilder);
        }

    }
}
