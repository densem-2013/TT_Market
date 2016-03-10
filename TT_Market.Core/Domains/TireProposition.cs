using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT_Market.Core.Domains
{
    public class TireProposition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ExtendedData { get; set; }
        public double RegularPrice { get; set; }
        public double DiscountPrice { get; set; }
        public double SpecialPrice { get; set; }
        public virtual Tire Tire { get; set; }
        public virtual PriceDocument PriceDocument { get; set; }
    }
}
