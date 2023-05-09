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
    public interface ITimeSheetRepository : IGenericRepository<Timesheet>
    {
        IEnumerable<Timesheet> GetTimeSheetData(Expression<Func<Timesheet, bool>> filter);
    }
}
