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
    public class CMSPageRepository : GenericRepository<CmsPage> , ICMSPageRepository
    {
        public CMSPageRepository(CIDbContext db) : base(db)
        {

        }
    }
}
