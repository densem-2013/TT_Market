using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT_Market.Core.Domains
{
    public class Brand
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string BrandTitle { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<Model> Models { get; set; }
        public virtual ICollection<Tire> Tires { get; set; } 
    }
}
