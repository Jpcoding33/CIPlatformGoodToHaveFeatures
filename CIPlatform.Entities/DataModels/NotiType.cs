using System;
using System.Collections.Generic;

namespace CIPlatformWeb.Entities.DataModels;

public partial class NotiType
{
    public long NottypeId { get; set; }

    public string? NotifType { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<NotifPreference> NotifPreferences { get; } = new List<NotifPreference>();
}
