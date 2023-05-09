using System;
using System.Collections.Generic;

namespace CIPlatformWeb.Entities.DataModels;

public partial class NotiSpecuser
{
    public long NotspecId { get; set; }

    public string? Notification { get; set; }

    public string? Url { get; set; }

    public long? FromUserid { get; set; }

    public long? ToUserid { get; set; }

    public byte Isread { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? FromUser { get; set; }

    public virtual User? ToUser { get; set; }
}
