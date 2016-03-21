using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT_Market.Core.Domains
{
    public class PriceDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime DownLoadDate { get; set; }
        public string FileName { get; set; }
        public virtual Agent Agent { get; set; }
        public virtual PriceLanguage PriceLanguage { get; set; }
        public virtual PriceReadSetting PriceReadSetting { get; set; }
        public virtual ICollection<TireProposition> TirePropositions { get; set; } 
    }
}
