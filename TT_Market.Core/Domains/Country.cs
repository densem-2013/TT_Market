using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class Country
    {
        public Country()
        {
            Tires = new List<Tire>();
            CountryTitles = new List<CountryTitle>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public ICollection<CountryTitle> CountryTitles { get; set; }
        public ICollection<Tire> Tires { get; set; }
    }
}
