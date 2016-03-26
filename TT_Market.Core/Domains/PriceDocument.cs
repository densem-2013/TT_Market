using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class PriceDocument
    {
        public PriceDocument()
        {
            TirePropositions = new List<TireProposition>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime DownLoadDate { get; set; }
        public string FileName { get; set; }
        public Agent Agent { get; set; }
        public PriceLanguage PriceLanguage { get; set; }
        public PriceReadSetting PriceReadSetting { get; set; }
        public ICollection<TireProposition> TirePropositions { get; set; }
    }
}
