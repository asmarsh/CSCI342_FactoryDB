using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI342_FactoryDB
{
    public class FactoryDB : DbContext
    {
        public DbSet<GraduateStudent> GraduateStudent { get; set; }
    }
}
