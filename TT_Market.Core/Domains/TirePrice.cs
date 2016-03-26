using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public class TirePrice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double RegularPrice { get; set; }
        public double? DiscountPrice { get; set; }
        public double? SpecialPrice { get; set; }
        public virtual TireProposition TireProposition { get; set; } 
        public virtual Currency Currency { get; set; }
    }
}
