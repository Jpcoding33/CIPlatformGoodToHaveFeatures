using System;
using System.Collections.Generic;

namespace CIPlatformWeb.Entities.DataModels;

public partial class NotifPreference
{
    public long NotprefId { get; set; }

    public long? Userid { get; set; }

    public long? NottypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual NotiType? Nottype { get; set; }

    public virtual User? User { get; set; }
}
