using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class TireProposition
    {
        public TireProposition()
        {
            TirePrices = new List<TirePrice>();
            Stocks = new List<Stock>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string TirePriceCode { get; set; }
        public string ExtendedData { get; set; }
        public int? RegionCount { get; set; }
        public int? WaitingCount { get; set; }
        public int? PartnersCount { get; set; }
        public int? ReservCount { get; set; }
        public PriceDocument PriceDocument { get; set; }
        public ICollection<Stock> Stocks { get; set; }
        public ICollection<TirePrice> TirePrices { get; set; }

    }
}
