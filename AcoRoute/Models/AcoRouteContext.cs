using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AcoRoute.Models
{
    public class AcoRouteContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public AcoRouteContext() : base("name=AcoRouteContext")
        {
        }

        public System.Data.Entity.DbSet<AcoRoute.Models.Vehicle> Vehicles { get; set; }

        public System.Data.Entity.DbSet<AcoRoute.Models.Address> Addresses { get; set; }

        public System.Data.Entity.DbSet<AcoRoute.Models.Person> People { get; set; }
    }
}
