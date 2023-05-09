using CIPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Entities.ViewModels
{
    public class VolTimeSheetViewModel
    {
        public long TimeSheetId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public long MissionId { get; set; }
         
        [Required(ErrorMessage = "Required!")]
        public int Hours { get; set; }

        [Required(ErrorMessage = "Required!")]
        public int Minutes { get; set; }

        [Required(ErrorMessage = "Required!")]
        public int Action { get; set; }

        [Required(ErrorMessage = "Required!")]
        public DateTime DateVolunteered { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? Notes { get; set; }

        public User? UserDetail { get; set; }
        public IEnumerable<MissionApplication> TimeBasedMissionAppList { get; set; } = new List<MissionApplication>();
        public IEnumerable<MissionApplication> GoalBasedMissionAppList { get; set; } = new List<MissionApplication>();
        public IEnumerable<Timesheet> TimeBasedData { get; set; } = new List<Timesheet>();
        public IEnumerable<Timesheet> GoalBasedData { get; set; } = new List<Timesheet>();
    }
}
