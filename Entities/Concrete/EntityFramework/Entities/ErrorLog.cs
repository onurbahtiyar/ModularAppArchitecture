using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Concrete.EntityFramework.Entities;

[Table("ErrorLog")]
public class ErrorLog : IEntity {
    [Key]
    public int Id { get; set; }

    public Guid UserGuid { get; set; }

    [StringLength(150)]
    public string? ActionName { get; set; }

    [StringLength(150)]
    public string? ControllerName { get; set; }

    public string? Message { get; set; }

    public string? StackTrace { get; set; }

    public string? UserInput { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateCreated { get; set; }

    [StringLength(50)]
    public string? System { get; set; }
}
