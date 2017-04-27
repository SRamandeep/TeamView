using Common.DataAccess.Mongo.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfdcConnector.Data.Repositories
{
    public sealed class RepositoryCase : RepositoryCaseContract
    {
        public RepositoryCase(IConnect Connect)
            : base(Connect) { }
    }
}
