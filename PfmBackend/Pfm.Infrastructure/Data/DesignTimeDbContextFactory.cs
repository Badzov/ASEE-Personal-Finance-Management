using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PfmDbContext>
    {
        public PfmDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PfmDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=pfm;Trusted_Connection=True;TrustServerCertificate=True;");

            return new PfmDbContext(optionsBuilder.Options);
        }
    }
}
