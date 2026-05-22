using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

using System.Data.Entity;// added for the Database.SetInitializer() method

namespace ESerranoEcoConnect.Models
{
    public class EcoConnectDbContext :IdentityDbContext<User>
    {
        public EcoConnectDbContext()
            : base("EcoConnectConnection", throwIfV1Schema: false)//changed first in <connectionStrings>... name=Defaultconnection to name= EcoConnectConnection in Web.config
        {
            Database.SetInitializer(new DatabaseInitialiser());
        }

        public static EcoConnectDbContext Create()
        {
            return new EcoConnectDbContext();
        }
    }
}