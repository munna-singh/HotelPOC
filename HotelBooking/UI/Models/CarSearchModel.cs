using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.carFlowSvc;

namespace UI.Models
{
    public class CarSearchModel
    {
        public SearchCarInfo searchCarInfo { get; set; }
        public CompanyRules companyrules { get; set; }

    }
}