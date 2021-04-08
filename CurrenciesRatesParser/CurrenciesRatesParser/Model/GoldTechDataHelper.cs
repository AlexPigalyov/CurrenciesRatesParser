using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrenciesRatesParser.Model
{
    public class GoldTechDataHelper
    {
        public static async Task UpdateCoinPrice(int id, decimal price)
        {
            using (var ctx = new Model.goldtechEntities())
            {
                var item = ctx.Products.FirstOrDefault(x => x.Id == id);
                item.Price = price;
                ctx.SaveChanges();
            }
        }
    }
}
