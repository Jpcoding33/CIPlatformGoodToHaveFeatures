using CIPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Entities.ViewModels
{
    public class BannerDetails
    {
        public long BannerId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? Text { get; set; }
        public int? SortOrder { get; set; }
        public string? Image { get; set; }
        public IEnumerable<Banner> BannerLists { get; set; } = new List<Banner>();
    }
}
