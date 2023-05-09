using CIPlatformWeb.Entities.Data;
using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Repositories.GenericRepository;
using CIPlatformWeb.Repositories.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Repositories.Repository
{
    public class MissionRepository : GenericRepository<Mission> , IMissionRepository
    {
        public MissionRepository(CIDbContext db) : base(db)
        {

        }

        public IEnumerable<Mission> GetMission(Expression<Func<Mission, bool>> filter)
        {
            IQueryable<Mission> query = dbSet;
            query = query.Where(filter).Include(mission => mission.City)
                .Include(mission => mission.Country)
                .Include(mission => mission.MissionMedia)
                .Include(mission => mission.MissionTheme)
                .Include(mission => mission.MissionSkills)
                .Include(mission => mission.FavoriteMissions);
            return query.ToList();
        }

        public Mission GetThisMission(Expression<Func<Mission, bool>> filter)
        {
            IQueryable<Mission> query = dbSet;
            query = query.Where(filter).Include(mission => mission.City)
                .Include(mission => mission.Country)
                .Include(mission => mission.MissionMedia)
                .Include(mission => mission.MissionDocuments)
                .Include(mission => mission.MissionTheme)
                .Include(mission => mission.MissionSkills)
                .Include(mission => mission.FavoriteMissions);
            return query.FirstOrDefault();
        }
    }
}
