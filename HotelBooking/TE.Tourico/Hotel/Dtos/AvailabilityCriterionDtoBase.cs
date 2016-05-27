using System;
using System.Linq;

namespace TE.Core.Tourico.Hotel.Dtos
{
    /// <summary>
    /// Base class for an availability criterion. This is service type agnostic.
    /// </summary>
    public class AvailabilityCriterionDtoBase
    {
        public AvailabilityCriterionDtoBase()
        {
            PageSize = 10;
        }

        public String CriteriaToken { get; set; }

        public int PageSize { get; set; }
    }
}