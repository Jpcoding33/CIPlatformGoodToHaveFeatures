using CIPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Entities.ViewModels
{
    public class CMSPageDetails
    {

        public long CmsPageId { get; set; }

        [Required(ErrorMessage = " Required!")]
        public string? Title { get; set; }

        [Required(ErrorMessage = " Required!")]
        public string? Description { get; set; }

        [Required(ErrorMessage = " Required!")]
        public string Slug { get; set; } = null!;

        [Required(ErrorMessage = " Required!")]
        public int? Status { get; set; }

        public IEnumerable<CmsPage> CMSLists { get; set; } = new List<CmsPage>();

    }
}
