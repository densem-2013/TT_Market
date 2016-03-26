using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class PriceLanguage
    {
        public PriceLanguage()
        {
            PriceDocuments = new List<PriceDocument>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string LanguageName { get; set; }
        public bool IsDefault { get; set; }
        public ICollection<PriceDocument> PriceDocuments { get; set; }
    }
}
