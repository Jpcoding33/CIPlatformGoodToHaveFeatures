using CIPlatformWeb.Entities.Data;
using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Repositories.GenericRepository;
using CIPlatformWeb.Repositories.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Repositories.Repository
{
    public class MissionSkillRepository : GenericRepository<MissionSkill> , IMissionSkillRepository
    {
        public MissionSkillRepository(CIDbContext db) : base(db)
        {

        }
        public IEnumerable<MissionSkill> GetMissionSkills()
        {
            IQueryable<MissionSkill> query = dbSet;
            return query.Include(missionSkill => missionSkill.Mission).Include(missionSkill => missionSkill.Skill).ToList();
        }

    }
}
