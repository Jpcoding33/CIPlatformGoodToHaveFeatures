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
    public class TimeSheetRepository : GenericRepository<Timesheet>, ITimeSheetRepository
    {
        public TimeSheetRepository(CIDbContext db) : base(db)
        {
        }

        public IEnumerable<Timesheet> GetTimeSheetData(Expression<Func<Timesheet, bool>> filter)
        {
            IQueryable<Timesheet> query = dbSet;
            query = query.Where(filter).Include(t => t.Mission);
            return query.ToList();
        }
    }
}
