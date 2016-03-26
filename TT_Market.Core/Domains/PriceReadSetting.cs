using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT_Market.Core.Domains
{
    public sealed class PriceReadSetting
    {
        public PriceReadSetting()
        {
            PriceDocuments = new List<PriceDocument>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string TransformMask { get; set; }
        public ICollection<PriceDocument> PriceDocuments { get; set; }
    }
}
