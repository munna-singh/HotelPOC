<<<<<<< HEAD
﻿//vaishak
=======
﻿//AnumehaSrivastava
>>>>>>> 8849d32aa5e4c30cd222cd0c18168b0bf25ca19a
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
//comment