﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public class SeasonTitle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }
        public virtual Season Season { get; set; }
        public virtual PriceLanguage PriceLanguage { get; set; }
    }
}
