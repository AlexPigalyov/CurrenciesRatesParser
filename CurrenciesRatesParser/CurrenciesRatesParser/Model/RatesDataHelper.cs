using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrenciesRatesParser.Model
{
    public static class RatesDataHelper
    {
        public static void AddRates(Rates rates)
        {
            using(var ctx = new RentooloEntities())
            {
                ctx.Rates.Add(rates);
            }
        }
    }
}
