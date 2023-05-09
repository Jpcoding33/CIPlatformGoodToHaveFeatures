using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace CIPlatformWeb.Repositories.Repository.Interface
{
    public interface IMissionRepository : IGenericRepository<Mission>
    {
        IEnumerable<Mission> GetMission(Expression<Func<Mission, bool>> filter);
        Mission GetThisMission(Expression<Func<Mission, bool>> filter);
    }
}
