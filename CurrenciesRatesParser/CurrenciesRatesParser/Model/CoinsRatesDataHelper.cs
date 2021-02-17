using System;
using System.Collections.Generic;

namespace CurrenciesRatesParser.Model
{
    public static class CoinsRatesDataHelper
    {
        public static void AddCoinsRatesRange(List<CoinsRates> rates)
        {
            try
            {
                using (var ctx = new Model.Entities())
                {
                    ctx.CoinsRates.AddRange(rates);
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
