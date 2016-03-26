using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class Brand
    {
        public Brand()
        {
            Models = new List<Model>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string BrandTitle { get; set; }
        public ICollection<Model> Models { get; set; }
    }
}
