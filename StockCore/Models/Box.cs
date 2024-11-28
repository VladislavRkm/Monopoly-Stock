using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.Models;

public class Box
{
    [Key]
    public Guid BoxId { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }
    public float Weight { get; set; }
    public float Volume { get; set; }
    public DateOnly productionDate { get; set; }
    public DateOnly expirationDate { get; set; }
    [ForeignKey(nameof(Pallet))]
    public Guid? PalletId { get; set; }
    public Pallet Pallet { get; set; }
} 
