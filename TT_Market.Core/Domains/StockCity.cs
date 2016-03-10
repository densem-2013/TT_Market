using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT_Market.Core.Domains
{
    public class StockCity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CityTitle { get; set; }
        public int Stock { get; set; }
        public virtual PriceDocument PriceDocument { get; set; }
        public virtual ICollection<Tire> Tires { get; set; } 
    }
}
