using CIPlatformWeb.Entities.Data;
using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Repositories.GenericRepository;
using CIPlatformWeb.Repositories.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Repositories.Repository
{
    public class StoryMediaRepository : GenericRepository<StoryMedium> , IStoryMediaRepository
    {
        public StoryMediaRepository(CIDbContext db) : base(db)
        {

        }
    }
}
