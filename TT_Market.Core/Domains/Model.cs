using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class Model
    {
        public Model()
        {
            Tires = new List<Tire>();
            ConvSigns = new List<ConvSign>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ModelTitle { get; set; }
        public Brand Brand { get; set; }
        public HomolAttribute Homol { get; set; }
        public AutoType AutoType { get; set; }
        public ProtectorType ProtectorType { get; set; }
        public Season Season { get; set; }
        public ICollection<ConvSign> ConvSigns { get; set; }
        public ICollection<Tire> Tires { get; set; }

    }
}
