using CIPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Entities.ViewModels
{
    public class MissionThemeDetails
    {
        public long MissionThemeId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Required!")]
        public int? Status { get; set; }
        public IEnumerable<MissionTheme> MissionThemeLists { get; set; } = new List<MissionTheme>();
    }
}
