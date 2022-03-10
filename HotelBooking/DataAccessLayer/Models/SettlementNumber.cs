//second change
using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public partial class SettlementNumber : BaseModel
    {
        //public SettlementNumber()
        //{
        //    //this.AmadeusOffices = new List<AmadeusOffice>();
        //    //this.PnrDataElements = new List<PnrDataElement>();
        //    //this.PNRs = new List<PNR>();
        //    //this.SabrePseudoCityCodes = new List<HotelBedsPseudoCityCode>();
        //}

        [Key]
        public int SettlementNumberId { get; set; }

        public string ArcIataNumber { get; set; }

        public string Description { get; set; }

        public int CurrencyId { get; set; }

        public virtual Currency Currency { get; set; }

        public bool IsAirSearchDefault { get; set; }

        public Nullable<int> GlobalTramsDataSourceId { get; set; }

        //public virtual ICollection<AmadeusOffice> AmadeusOffices { get; set; }
        //public virtual ICollection<PnrDataElement> PnrDataElements { get; set; }
        //public virtual ICollection<PNR> PNRs { get; set; }
        //public virtual ICollection<SabrePseudoCityCode> SabrePseudoCityCodes { get; set; }
        //public virtual ICollection<SettlementNumberAgencyRule> SettlementNumberAgencyRules { get; set; }
    }
}
