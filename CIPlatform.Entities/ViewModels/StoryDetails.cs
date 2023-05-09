using CIPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Entities.ViewModels
{
    public class StoryDetails
    {
        public Story? StoryDetail { get; set; }
        public IEnumerable<Story> StoryLists { get; set; } = new List<Story>();     
    }
}
