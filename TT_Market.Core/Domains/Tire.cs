﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public class Tire
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TireTitle { get; set; }
        public virtual TireProposition TireProposition { get; set; }
        public virtual Width Width { get; set; }
        public virtual Height Height { get; set; }
        public virtual Diameter Diameter { get; set; }
        public virtual SpeedIndex SpeedIndex { get; set; }
        public virtual PressIndex PressIndex { get; set; }
        public virtual Model Model { get; set; }
        public virtual Country Country { get; set; }
        public virtual ProductionYear ProductionYear { get; set; }

    }
}
