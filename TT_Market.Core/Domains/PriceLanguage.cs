using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class PriceLanguage
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string LanguageName { get; set; }
        public bool IsDefault { get; set; }
    }
}
