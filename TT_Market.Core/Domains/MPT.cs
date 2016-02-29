using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT_Market.Core.Domains
{
    public class MPT
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TireTitle { get; set; }
        public string ExtendedData { get; set; }
        public int Stock { get; set; }
        public double RegularPrice { get; set; }
        public double DiscountPrice { get; set; }
        public double SpecialPrice { get; set; }
        //public int BrandId { get; set; }
        //public int WidthId { get; set; }
        //public int HeightId { get; set; }
        //public int DiameterId { get; set; }
        //public int ModelId { get; set; }
        //public int PriceListId { get; set; }
        //public int SpeedIndexId { get; set; }
        //public int PressIndexId { get; set; }
        //public int ConvSignId { get; set; }
        //public int HomolId { get; set; }
        //public int SeasonId { get; set; }
        //public int AutoTypeId { get; set; }
        //public int CountryId { get; set; }
        //public int StockCityId { get; set; }
        //public int CurrencyId { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual Width Width { get; set; }
        public virtual Height Height { get; set; }
        public virtual Diameter Diameter { get; set; }
        public virtual Model Model { get; set; }
        public virtual PriceList PriceList { get; set; }
        public virtual SpeedIndex SpeedIndex { get; set; }
        public virtual PressIndex PressIndex { get; set; }
        public virtual ConvSign ConvSign { get; set; }
        public virtual HomolAttribute Homol { get; set; }
        public virtual Season Season { get; set; }
        public virtual AutoType AutoType { get; set; }
        public virtual Country Country { get; set; }
        public virtual StockCity StockCity { get; set; }
        public virtual Currency Currency { get; set; }
    }
}
