using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestaurantService.Models;

public partial class Menu
{
    [Key]
    public int MenuId { get; set; }

    public int RestaurantId { get; set; }

    [StringLength(100)]
    public string MenuName { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Menu")]
    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    [ForeignKey("RestaurantId")]
    [InverseProperty("Menus")]
    public virtual Restaurant Restaurant { get; set; } = null!;
}
