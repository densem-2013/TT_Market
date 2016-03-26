using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class Diameter
    {
        public Diameter()
        {
            Tires = new List<Tire>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string DSize { get; set; }
        public ICollection<Tire> Tires { get; set; }
    }
}
