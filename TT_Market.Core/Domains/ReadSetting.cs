using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT_Market.Core.Domains
{
    public class ReadSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ProviderCell { get; set; }
        public string ProviderPhoneCell { get; set; }
        public string ProviderEmailCell { get; set; }
        public string InsertDateCell { get; set; }
        public int ColumnNameRow { get; set; }
        public virtual Provider Provider { get; set; }
        public virtual ICollection<PriceList> PriceLists { get; set; } 
    }
}
