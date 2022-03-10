//AnumehaSrivastava
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public partial class Currency
    {
        public string CurrencyName { get; set; }
        public string Name { get { return CurrencyName; } }
    }
}
