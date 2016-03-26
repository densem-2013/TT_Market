using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class Agent
    {
        public Agent()
        {
            PriceDocuments = new List<PriceDocument>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string AgentTitle { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public ICollection<PriceDocument> PriceDocuments { get; set; }
    }
}
