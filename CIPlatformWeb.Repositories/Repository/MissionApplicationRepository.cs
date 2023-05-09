using CIPlatformWeb.Entities.Data;
using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Repositories.GenericRepository;
using CIPlatformWeb.Repositories.GenericRepository.Interface;
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
    public class MissionApplicationRepository : GenericRepository<MissionApplication> , IMissionApplicationRepository
    {
        private readonly CIDbContext _db;
        public MissionApplicationRepository(CIDbContext db) : base(db)
        {
            _db = db;
        }
        public IEnumerable<MissionApplication> GetMissionApplicationList(Expression<Func<MissionApplication, bool>> filter)
        {
            IQueryable<MissionApplication> query = dbSet;
            query = query.Where(filter).Include(m => m.Mission).Include(m => m.Mission.City);
            return query.ToList();
        }
        public IEnumerable<MissionApplication> GetAllMissAppList(Expression<Func<MissionApplication, bool>> filter)
        {
            IQueryable<MissionApplication> query = dbSet;
            query = query.Where(filter).Include(m => m.Mission).Include(m => m.User);
            return query.ToList();
        }
        public IEnumerable<MissionApplication> GetMissAppList()
        {
            IQueryable<MissionApplication> query = dbSet;
            query = query.Include(m => m.Mission).Include(m => m.User);
            return query.ToList();
        }

        public List<MissionApplication> GetMissionApplicationsData(long userId)
        {
            var query = from ma in _db.MissionApplications
                        join gm in _db.GoalMissions
                        on ma.MissionId equals gm.MissionId into g
                        from gm in g.DefaultIfEmpty()
                        where ma.UserId == userId && ma.ApprovalStatus == "APPROVE" && ma.Mission.MissionType == "GOAL" && gm.AchievedValue > gm.GoalValue
                        select ma;

            return query.ToList();
        }
    }
}
