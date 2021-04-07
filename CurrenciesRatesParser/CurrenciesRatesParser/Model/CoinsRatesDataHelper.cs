using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrenciesRatesParser.Model
{
    public static class CoinsRatesDataHelper
    {
        public static void AddCoinsRatesRange(List<CoinsRate> rates)
        {
            try
            {
                using (var ctx = new Model.Entities())
                {
                    ctx.Database.Connection.Open();

                    rates.ForEach(x =>
                    {
                        var lastItemValue = ctx.CoinsRates.OrderByDescending(r => r.Date).FirstOrDefault(g => g.Site == x.Site && g.Acronim == x.Acronim).Sell;
                        if (x.Sell > lastItemValue)
                        {
                            x.IsUp = true;
                        }
                        else if (x.Sell < lastItemValue)
                        {
                            x.IsUp = false;
                        }
                        else if (x.Sell == lastItemValue)
                        {
                            x.IsUp = null;
                        }
                    });

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
