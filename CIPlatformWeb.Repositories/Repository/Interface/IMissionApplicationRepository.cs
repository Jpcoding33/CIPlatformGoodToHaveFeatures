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
    public interface IMissionApplicationRepository : IGenericRepository<MissionApplication>
    {
        IEnumerable<MissionApplication> GetMissionApplicationList(Expression<Func<MissionApplication, bool>> filter);
        IEnumerable<MissionApplication> GetAllMissAppList(Expression<Func<MissionApplication, bool>> filter);
        IEnumerable<MissionApplication> GetMissAppList();
        List<MissionApplication> GetMissionApplicationsData(long userId);
    }
}
