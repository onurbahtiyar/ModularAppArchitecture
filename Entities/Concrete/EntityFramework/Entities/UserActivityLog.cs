using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Concrete.EntityFramework.Entities;

[Table("UserActivityLog")]
public class UserActivityLog : IEntity {
    [Key]
    public int Id { get; set; }

    public Guid? UserGuid { get; set; }

    public string? ActivityType { get; set; }

    public string? ActivityDetail { get; set; }

    [Column("IPAddress")]
    public string? Ipaddress { get; set; }

    public string? BrowserInfo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ActivityDate { get; set; }

    public string? AdditionalData { get; set; }

    public string? ActivityPage { get; set; }
}
