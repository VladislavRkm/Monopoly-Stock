using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.Models;

public class Pallet
{
    [Key]
    public Guid PalletId { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }
    public float Weight { get; set; }
    public DateOnly? expirationDate { get; set; }
    public float Volume { get; set; }
    public ICollection<Box> Boxes { get; set; }
}
