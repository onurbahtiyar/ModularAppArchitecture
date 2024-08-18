using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Concrete.EntityFramework.Entities;

[Table("UserRole")]
public class UserRole : IEntity {
    [Key]
    public int UserRoleId { get; set; }

    public Guid UserGuid { get; set; }

    public int RoleId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime AssignedDate { get; set; }

    public int UserId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("UserRoles")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("UserGuid")]
    [InverseProperty("UserRoles")]
    public virtual User User { get; set; } = null!;
}
