using System;
using System.Collections.Generic;

namespace CIPlatformWeb.Entities.DataModels;

public partial class City
{
    public long CityId { get; set; }

    public long CountryId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Mission> Missions { get; } = new List<Mission>();

}
