using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT_Market.Core.Domains
{
    public class ConvSign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public virtual ICollection<Tire> Tires { get; set; } 
    }
}
