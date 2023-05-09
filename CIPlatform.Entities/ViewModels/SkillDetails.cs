using CIPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Entities.ViewModels
{
    public class SkillDetails
    {
        public long SkillId { get; set; }

        [Required(ErrorMessage = " Required!")]
        public string SkillName { get; set; } = null!;

        [Required(ErrorMessage = " Required!")]
        public int? Status { get; set; }
        public IEnumerable<Skill> SkillLists { get; set; } = new List<Skill>();
    }
}
