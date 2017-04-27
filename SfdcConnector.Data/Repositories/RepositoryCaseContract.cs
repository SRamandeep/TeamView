using Common.DataAccess.Mongo.Connection;
using Common.DataAccess.Mongo.Contracts;
using SfdcConnector.Data.Models;


namespace SfdcConnector.Data.Repositories
{
    public abstract class RepositoryCaseContract : Repository<Case>, IRepository<Case>
    {
        public RepositoryCaseContract(IConnect Connect)
            : base(Connect) { }
    }
}
