using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class SeasonTitle
    {
        public SeasonTitle()
        {
            SeasonTitleAlters = new List<SeasonTitleAlter>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }
        public Season Season { get; set; }
        public PriceLanguage PriceLanguage { get; set; }
        public ICollection<SeasonTitleAlter> SeasonTitleAlters { get; set; }
    }
}
