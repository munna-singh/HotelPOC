using HotelBeds.ServiceCatalogues.HotelCatalog.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBeds.Models
{
    [Serializable]
    public struct TCurrency
    {
        public int CurrencyId { get; set; }

        public static bool operator ==(TCurrency m1, TCurrency m2)
        {
            return m1.CurrencyId == m2.CurrencyId;

        }

        public static bool operator !=(TCurrency m1, TCurrency m2)
        {
            return m1.CurrencyId != m2.CurrencyId;

        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string CurrencyCode { get; set; }

        public string Symbol { get; set; }

        private static List<TCurrency> Currencies;

        public static TCurrency Get(int currencyId)
        {
            /*if (currencyId == 0)
                currencyId = 1;*/
            return Currencies.Single(t => t.CurrencyId == currencyId);
        }
        internal static TCurrency Get(string isoCurrencyCode)
        {
            if (!string.IsNullOrEmpty(isoCurrencyCode))
                return Currencies.Single(t => t.CurrencyCode == isoCurrencyCode);
            else
                return new TCurrency();
        }
        internal static TCurrency Get(CurrencyTypes currencyType)
        {
            return Currencies.Single(t => t.CurrencyCode == currencyType.ToString());
        }
        public static void Init(List<TCurrency> currencyList)
        {
            Currencies = currencyList;
        }
    }
}
