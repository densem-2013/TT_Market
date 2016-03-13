using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT_Market.Core.Domains
{
    public class Agent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AgentTitle { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<PriceDocument> PriceDocuments { get; set; } 
    }
}
