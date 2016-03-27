using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public class SeasonTitleAlter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string TitleAlterValue { get; set; }
        public virtual SeasonTitle SeasonTitle { get; set; }
    }
}
