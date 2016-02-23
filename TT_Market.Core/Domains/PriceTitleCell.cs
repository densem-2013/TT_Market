using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT_Market.Core.Domains
{
    public class PriceTitleCell
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public string ColumnName { get; set; }
        public virtual ICollection<PriceList> PriceLists { get; set; }
        public virtual WorkSheet WorkSheet { get; set; }
        public virtual ICollection<ChildCellColumnName> ChildCells { get; set; } 
    }
}
