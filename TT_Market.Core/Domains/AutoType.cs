using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class AutoType
    {
        public AutoType()
        {
            AutoTypeAlters = new List<AutoTypeAlter>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string TypeValue { get; set; }
        public ICollection<AutoTypeAlter> AutoTypeAlters { get; set; }
    }
}
