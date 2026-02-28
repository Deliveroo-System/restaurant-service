using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestaurantService.Models;

public partial class Restaurant
{
    [Key]
    public int RestaurantId { get; set; }

    public int OwnerId { get; set; }

    public int CategoryId { get; set; }

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string Address { get; set; } = null!;

    [StringLength(20)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(100)]
    public string? Email { get; set; }

    public TimeOnly OpeningTime { get; set; }

    public TimeOnly ClosingTime { get; set; }

    public bool IsApproved { get; set; }

    public bool IsAvailable { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Restaurants")]
    public virtual RestaurantCategory Category { get; set; } = null!;

    [InverseProperty("Restaurant")]
    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();

    [ForeignKey("OwnerId")]
    [InverseProperty("Restaurants")]
    public virtual User Owner { get; set; } = null!;
}
