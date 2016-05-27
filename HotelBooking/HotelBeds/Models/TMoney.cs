using HotelBeds.ServiceCatalogues.HotelCatalog.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBeds.Models
{
    [Serializable]
    public class TMoney : IEquatable<TMoney>
    {
        public enum TMoneyDisplayType
        {
            None,
            CurrencyString,
            SymbolOnly
        }
        [JsonIgnore]
        public TCurrency Currency { get; set; }

        public decimal Amount { get; set; }

        [Required]
        public String CurrencyCode
        {
            get
            {
                return Currency.CurrencyCode;
            }
            set
            {
                Currency = TCurrency.Get(value);
            }
        }

        public TMoney()
        {
            Amount = 0m;
            Currency = new TCurrency
            {
                CurrencyId = 0,
                CurrencyCode = "NA",
                Symbol = ""
            };
        }

        /// <summary>
        /// Represents TMoney with the given currency and amount of 0
        /// </summary>
        /// <param name="currency"></param>
        public TMoney(TCurrency currency)
            : this(0m, currency)
        {
        }

        /// <summary>
        /// Represents an amount of TMoney in a specific currency.
        /// </summary>
        public TMoney(decimal amount, TCurrency currency)
            : this()
        {
            Currency = currency;
            Amount = amount;
        }

        /// <summary>
        /// Represents an amount of TMoney in a specific currency.
        /// </summary>
        public TMoney(decimal amount, int currencyId)
            : this(amount, TCurrency.Get(currencyId))
        {
        }

        /// <summary>
        /// Represents an amount of TMoney in a specific currency.
        /// </summary>
        public TMoney(decimal amount, String isoCurrencyCode)
            : this(amount, TCurrency.Get(isoCurrencyCode))
        { }


        /// <summary>
        /// Represents an amount of TMoney in a specific currency.
        /// </summary>
        public TMoney(decimal amount, CurrencyTypes currencyType)
            : this(amount, TCurrency.Get(currencyType))
        { }


        /// <summary>
        /// Represents an amount of TMoney in a specific currency.
        /// </summary>
        public TMoney(string amount, String isoCurrencyCode)
            : this(decimal.Parse(amount), TCurrency.Get(isoCurrencyCode))
        { }


        /// <summary>
        /// Represents an amount of TMoney in a specific currency.
        /// </summary>
        public TMoney(CurrencyTypes currencyType)
            : this(0m, TCurrency.Get(currencyType))
        { }

        #region Standard operator overloading

        public static TMoney operator -(TMoney m1, TMoney m2)
        {
            // Allow operations on NULLs for convenience
            if (m1 == null)
                return m2 * -1;
            if (m2 == null)
                return m1;

            CheckCurrencies(m1, m2);
            return new TMoney(m1.Amount - m2.Amount, m1.Currency);
        }

        public static TMoney operator -(TMoney m1, decimal d)
        {
            // Allow operations on NULLs for convenience
            if (m1 == null)
                throw new ArgumentNullException("m1", @"TMoney can not be null");

            return new TMoney(m1.Amount - d, m1.Currency);
        }

        public static TMoney operator +(TMoney m1, TMoney m2)
        {
            if (m1 == null)
                return m2;
            if (m2 == null)
                return m1;

            CheckCurrencies(m1, m2);
            return new TMoney(m1.Amount + m2.Amount, m1.Currency);
        }


        public static TMoney operator +(TMoney m1, decimal d)
        {
            if (m1 == null)
                throw new ArgumentNullException("m1", @"TMoney can not be null");

            return new TMoney(m1.Amount + d, m1.Currency);
        }

        public static TMoney operator *(TMoney m1, decimal d)
        {
            if (m1 == null)
                throw new ArgumentNullException("m1", @"TMoney can not be null");

            return new TMoney(m1.Amount * d, m1.Currency);
        }

        // e.g. multiplying by number of pax
        public static TMoney operator *(TMoney m1, int d)
        {
            if (m1 == null)
                throw new ArgumentNullException("m1", @"TMoney can not be null");

            return new TMoney(m1.Amount * d, m1.Currency);
        }

        public static TMoney operator *(int d, TMoney m1)
        {
            if (m1 == null)
                throw new ArgumentNullException("m1", @"TMoney can not be null");

            return new TMoney(m1.Amount * d, m1.Currency);
        }

        public static TMoney operator /(TMoney m1, decimal d)
        {
            if (m1 == null)
                throw new ArgumentNullException("m1", @"TMoney can not be null");

            return new TMoney(m1.Amount / d, m1.Currency);
        }

        public static decimal operator /(TMoney m1, TMoney m2)
        {
            if (m1 == null || m2 == null)
                throw new ApplicationException("Can only divide between two non-null money amounts");

            CheckCurrencies(m1, m2);
            return m1.Amount / m2.Amount;
        }

        public static bool operator ==(TMoney m1, TMoney m2)
        {
            if (Object.ReferenceEquals(m1, m2))
                return true;

            if (Object.ReferenceEquals(m1, null) || Object.ReferenceEquals(m2, null))
                return false;

            return (m1.Currency == m2.Currency && m1.Amount == m2.Amount);

        }

        public static bool operator !=(TMoney m1, TMoney m2)
        {
            return !(m1 == m2);
        }

        public static bool operator >(TMoney m1, TMoney m2)
        {
            CheckCurrencies(m1, m2);
            return m1.Amount > m2.Amount;
        }

        public static bool operator <(TMoney m1, TMoney m2)
        {
            CheckCurrencies(m1, m2);
            return m1.Amount < m2.Amount;
        }

        public static bool operator >=(TMoney m1, TMoney m2)
        {
            CheckCurrencies(m1, m2);
            return m1.Amount >= m2.Amount;
        }

        public static bool operator <=(TMoney m1, TMoney m2)
        {
            CheckCurrencies(m1, m2);

            return m1.Amount <= m2.Amount;
        }

        public static bool operator >(TMoney m1, decimal m2)
        {
            return m1.Amount > m2;
        }

        public static bool operator <(TMoney m1, decimal m2)
        {
            return m1.Amount < m2;
        }

        public static bool operator >=(TMoney m1, decimal m2)
        {
            return m1.Amount >= m2;
        }

        public static bool operator <=(TMoney m1, decimal m2)
        {
            if (m1 == null)
                return true;

            return m1.Amount <= m2;
        }

        public static bool operator ==(TMoney m1, decimal m2)
        {
            if (ReferenceEquals(m1, null))
                return false;
            return m1.Amount == m2;
        }

        public static bool operator !=(TMoney m1, decimal m2)
        {
            if (ReferenceEquals(m1, null))
                return true;

            return m1.Amount != m2;
        }

        public static bool operator ==(decimal m2, TMoney m1)
        {
            if (ReferenceEquals(m1, null))
                return false;
            return m1.Amount == m2;
        }
        public static bool operator !=(decimal m2, TMoney m1)
        {
            if (ReferenceEquals(m1, null))
                return true;

            return m1.Amount != m2;
        }

        public static TMoney operator -(TMoney m1)
        {
            return m1 * -1;
        }

        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;

            if (obj.GetType() != typeof(TMoney))
                return false;

            return this == (TMoney)obj;
        }

        public override int GetHashCode()
        {
            return Amount.GetHashCode() ^ Currency.GetHashCode();
        }

        #endregion

        private static void CheckCurrencies(TMoney m1, TMoney m2)
        {
            //check if the amount of both objects is more than zero
            if (m1 != null && m2 != null)
            {
                if (m1.Amount != 0 && m2.Amount != 0)
                {
                    if (!(m1.Currency == m2.Currency))
                        throw new InvalidOperationException(
                            String.Format("Cannot perform an operation on two different currencies ({0} and {1})", m1, m2));
                }
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(TMoney other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this == other;
        }

        public TMoney Ceiling()
        {
            return new TMoney(Math.Ceiling(this.Amount), this.Currency);
        }

        public TMoney Floor()
        {
            return new TMoney(Math.Floor(this.Amount), this.Currency);
        }

        public TMoney Round()
        {
            return new TMoney(Math.Round(this.Amount), this.Currency);
        }

        public TMoney Round(int decimals)
        {
            return new TMoney(Math.Round(this.Amount, decimals), this.Currency);
        }

        public static TMoney Min(TMoney m1, TMoney m2)
        {
            CheckCurrencies(m1, m2);

            if (m1.Amount < m2.Amount)
                return (TMoney)m1.MemberwiseClone();
            else
                return (TMoney)m2.MemberwiseClone();
        }

        public static TMoney Max(TMoney m1, TMoney m2)
        {
            CheckCurrencies(m1, m2);

            if (m1.Amount > m2.Amount)
                return (TMoney)m1.MemberwiseClone();
            else
                return (TMoney)m2.MemberwiseClone();
        }

        /// <summary>
        /// Will subtract b from a. If a or b is null, that value will be considered a 0.
        /// If both are NULL, NULL will be returned.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="nullStateMustBeSame">If true, it will throw an exception if one of a or b is NULL (but not both)</param>
        /// <exception cref="ApplicationException"></exception>
        /// <returns></returns>
        public static TMoney SubtractNull(TMoney a, TMoney b, bool nullStateMustBeSame = false)
        {
            if (nullStateMustBeSame)
            {
                if (a == null && b != null || a != null && b == null)
                    throw new ApplicationException("A or B is NULL - both must be either not null or null");
            }

            if (a == null)
                return b * -1;

            if (b == null)
                return a;

            return a - b;
        }

        /// <summary>
        /// Will add a to b. If a or b is null, that value will be considered a 0.
        /// If both are NULL, NULL will be returned.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="nullStateMustBeSame">If true, it will throw an exception if one of a or b is NULL (but not both)</param>
        /// <returns></returns>
        public static TMoney AddNull(TMoney a, TMoney b, bool nullStateMustBeSame = false)
        {
            if (nullStateMustBeSame)
            {
                if (a == null && b != null || a != null && b == null)
                    throw new ApplicationException("A or B is NULL - both must be either not null or null");
            }

            if (a == null)
                return b;

            if (b == null)
                return a;

            return a + b;
        }

        #region IComparable Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: 
        ///                     Value 
        ///                     Meaning 
        ///                     Less than zero 
        ///                     This object is less than the <paramref name="other"/> parameter.
        ///                     Zero 
        ///                     This object is equal to <paramref name="other"/>. 
        ///                     Greater than zero 
        ///                     This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public int CompareTo(TMoney other)
        {
            if (other == null)
            {
                return -1;
            }

            return Amount.CompareTo((other).Amount);
        }

        #endregion

        public static bool IsNullOrZero(TMoney TMoney)
        {
            return TMoney == null || TMoney == 0m;
        }

        /// <summary>
        /// Returns the absolute value of a <see cref="TMoney"/>.
        /// </summary>
        /// <returns></returns>
        public TMoney Abs()
        {
            return new TMoney(Math.Abs(Amount), Currency);
        }

        /// <summary>
        /// Returns the TMoney object as String.
        /// </summary>
        /// <returns>ex format: USD $500.00 </returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", this.CurrencyCode, this.Amount.ToString("c"));
        }

        /// <summary>
        /// Returns the TMoney object as String with a specific string format.
        /// </summary>
        /// <returns>ex format: USD $500.00 </returns>
        public string ToString(CurrencyFormat format)
        {
            string currencyFormat = string.Empty;

            if (format == CurrencyFormat.ClientItinerary)
            {
                if (this.CurrencyCode == CurrencyTypes.USD.ToString())
                    currencyFormat = string.Format("{0} {1}", this.CurrencyCode, this.Amount.ToString("c"));
                else
                    currencyFormat = string.Format("{0} {1}", this.CurrencyCode, this.Amount.ToString("c").Replace('$', ' '));
            }

            return currencyFormat;
        }
    }
}
