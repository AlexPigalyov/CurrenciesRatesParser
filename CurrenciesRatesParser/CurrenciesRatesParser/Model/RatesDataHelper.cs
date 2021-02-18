using System;
using System.Collections.Generic;

namespace CurrenciesRatesParser.Model
{
    public static class RatesDataHelper
    {
        public static void AddRatesRange(List<Rates> rates)
        {
            try
            {
                using (var ctx = new Model.Entities())
                {
                    ctx.Rates.AddRange(rates);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception information: {0}", ex.Message);
            }
        }
    }
}
