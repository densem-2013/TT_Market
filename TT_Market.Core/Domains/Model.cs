using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT_Market.Core.Domains
{
    public class Model
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ModelTitle { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual HomolAttribute Homol { get; set; }
        public virtual ProtectorType ProtectorType { get; set; }
        public virtual Season Season { get; set; }
        public virtual ICollection<Tire> Tires { get; set; } 
    }
}
