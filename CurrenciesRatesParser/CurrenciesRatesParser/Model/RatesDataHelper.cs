using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrenciesRatesParser.Model
{
    public static class RatesDataHelper
    {
        public static void AddRatesRange(List<Rate> rates)
        {
            try
            {
                using (var ctx = new Model.Entities())
                {
                    ctx.Database.Connection.Open();

                    rates.ForEach(x =>
                    {
                        x.IsUp = ctx.Rates.LastOrDefault(g => g.Site == x.Site).Buy <= x.Buy;
                    });

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
