using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CurrenciesRatesParser.Model
{
    public static class RatesDataHelper
    {
        public static void AddRates(Rates rates)
        {
            try
            {
                using (var ctx = new Model.Entities())
                {
                    ctx.Rates.Add(rates);
                    ctx.SaveChanges();
                }

                Console.WriteLine("Rates saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception information: {0}", ex.Message);
            }
        }

        public async static Task<Dictionary<string, double>> GetMetalRates()
        {
            Dictionary<string, double> currencies = new Dictionary<string, double>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var htmlCode = await client.DownloadStringTaskAsync("https://www.moex.com/ru/derivatives/commodity/gold/");
                var matches = Regex.Matches(htmlCode, "(?<=>)([1-9]+.[\\d,]+)+|([\\d,])(?=<)");
                int index = 0;
                foreach (Match item in matches)
                {
                    if (index == 8)
                    {
                        currencies.Add("XAU", double.Parse(item.Value.Replace(" ", "")));
                    }
                    else if (index == 16)
                    {
                        currencies.Add("PAL", double.Parse(item.Value.Replace(" ", "")));
                    }
                    else if (index == 23)
                    {
                        currencies.Add("PL", double.Parse(item.Value.Replace(" ", "")));
                    }
                    else if (index == 30)
                    {
                        currencies.Add("XAG", double.Parse(item.Value.Replace(" ", "")));
                    }
                    index++;
                }
            }
            return currencies;
        }
        public async static Task<Dictionary<string, double>> GetCurrencyRates()
        {
            Dictionary<string, double> currencies = new Dictionary<string, double>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var htmlCode = await client.DownloadStringTaskAsync("https://ru.exchange-rates.org/currentRates/E/USD");
                var ratesMatches = Regex.Matches(htmlCode, "(?<a>)([0-9]+,[0-9]+)+(?=<)");
                var codeOfRatesMatches = Regex.Matches(htmlCode, "(?<a>)(Обмен.*?)+(?=</a>)");
                for (int i = 0; i < ratesMatches.Count; i++)
                {
                    currencies.Add(codeOfRatesMatches[i].Value.Split()[3], double.Parse(ratesMatches[i].Value));
                }
            }
            return currencies;
        }
    }
}
