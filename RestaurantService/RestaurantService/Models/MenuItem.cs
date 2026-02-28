using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestaurantService.Models;

public partial class MenuItem
{
    [Key]
    public int MenuItemId { get; set; }

    public int MenuId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("MenuId")]
    [InverseProperty("MenuItems")]
    public virtual Menu Menu { get; set; } = null!;
}
