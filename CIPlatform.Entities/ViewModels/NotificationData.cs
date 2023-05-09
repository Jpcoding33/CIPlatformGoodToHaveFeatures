using CIPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Entities.ViewModels
{
    public class NotificationData
    {
        public IEnumerable<NotiType> NotificationTypeList { get; set; } = new List<NotiType>();
        public IEnumerable<NotifAlluser> NotificationToAllList { get; set; } = new List<NotifAlluser>();
        public int NotificationCount { get; set; } = 0;
    }
}
