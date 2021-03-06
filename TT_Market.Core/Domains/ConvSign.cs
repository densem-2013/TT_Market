﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class ConvSign
    {
        public ConvSign()
        {
            Tires = new List<Tire>();
            ConvAlters = new List<ConvAlter>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
        public Model Model { get; set; }
        public ICollection<Tire> Tires { get; set; }
        public ICollection<ConvAlter> ConvAlters { get; set; }
    }
}
