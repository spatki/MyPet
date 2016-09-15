using System.Data.Entity;

namespace ProcessAccelerator.WebUI.Dto
{
    public interface IDbContextFactory
    {
        DbContext GetContext();
    }

    public class DBContextFactory : IDbContextFactory
    {
        private DbContext dbContext;

        public DBContextFactory()
        {
            dbContext = new DbContext("DefaultConnection");
            dbContext.Configuration.ProxyCreationEnabled = false;
        }

        public DbContext GetContext()
        {
            return dbContext;
        }
    }
}