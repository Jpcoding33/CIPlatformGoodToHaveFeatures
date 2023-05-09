using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Repositories.Repository.Interface
{
    public interface IMissionSkillRepository : IGenericRepository<MissionSkill>
    {
        IEnumerable<MissionSkill> GetMissionSkills();
    }
}
