using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT_Market.Core.Domains
{
    public sealed class Season
    {
        public Season()
        {
            SeasonTitles = new List<SeasonTitle>();
            Models = new List<Model>();
        }

        [KeyAttribute]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public ICollection<SeasonTitle> SeasonTitles { get; set; }
        public ICollection<Model> Models { get; set; }
    }
}
