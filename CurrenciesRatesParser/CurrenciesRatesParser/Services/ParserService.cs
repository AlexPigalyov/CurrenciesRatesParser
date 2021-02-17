using CurrenciesRatesParser.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ratesRatesParser.Services
{
    public static class ParserService
    {
        public static async Task<List<Rates>> GetMetalRates()
        {
            string site = "https://www.moex.com/ru/derivatives/commodity/gold/";
            List<Rates> rates = new List<Rates>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var htmlCode = await client.DownloadStringTaskAsync(site);
                var matches = Regex.Matches(htmlCode, "(?<=>)([1-9]+.[\\d,]+)+|([\\d,])(?=<)");
                int index = 0;

                foreach (Match item in matches)
                {
                    if (index == 8)
                    {
                        double price = double.Parse(item.Value.Replace(" ", ""));
                        rates.Add(new Rates()
                        {
                            Acronim = "XAU",
                            Sell = price,
                            Buy = price,
                            Date = DateTime.Now,
                            Site = site
                        });
                    }
                    else if (index == 16)
                    {
                        double price = double.Parse(item.Value.Replace(" ", ""));
                        rates.Add(new Rates()
                        {
                            Acronim = "PAL",
                            Sell = price,
                            Buy = price,
                            Date = DateTime.Now,
                            Site = site
                        });
                    }
                    else if (index == 23)
                    {
                        double price = double.Parse(item.Value.Replace(" ", ""));
                        rates.Add(new Rates()
                        {
                            Acronim = "PL",
                            Sell = price,
                            Buy = price,
                            Date = DateTime.Now,
                            Site = site
                        });
                    }
                    else if (index == 30)
                    {
                        double price = double.Parse(item.Value.Replace(" ", ""));
                        rates.Add(new Rates()
                        {
                            Acronim = "XAG",
                            Sell = price,
                            Buy = price,
                            Date = DateTime.Now,
                            Site = site
                        });
                    }
                    index++;
                }
            }
            return rates;
        }

        public static async Task<List<Rates>> GetExchangeRates()
        {
            DateTime parsingStartTime = DateTime.Now;

            List<Rates> rates = new List<Rates>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                List<string> urls = new List<string>()
                {
                    "https://ru.exchange-rates.org/currentRates/A/USD",
                    "https://ru.exchange-rates.org/currentRates/P/USD",
                    "https://ru.exchange-rates.org/currentRates/E/USD",
                    "https://ru.exchange-rates.org/currentRates/M/USD",
                    "https://ru.exchange-rates.org/currentRates/F/USD"
                };

                foreach (string currentUrl in urls)
                {
                    var htmlCode = await client.DownloadStringTaskAsync(currentUrl);
                    var ratesMatches = Regex.Matches(htmlCode, "(?<a>)([0-9]+,[0-9]+)+(?=<)");
                    var codeOfRatesMatches = Regex.Matches(htmlCode, "(?<a>)(Обмен.*?)+(?=</a>)");
                    for (int i = 0; i < ratesMatches.Count; i++)
                    {
                        double price = double.Parse(ratesMatches[i].Value);
                        rates.Add(new Rates()
                        {
                            Acronim = codeOfRatesMatches[i].Value.Split()[3],
                            Sell = price,
                            Buy = price,
                            Date = parsingStartTime,
                            Site = currentUrl
                        });
                    }
                }
            }
            return rates;
        }

        public static async Task<List<CoinsRates>> GetCoinsRatesZolotoyZapas()
        {
            var web = new HtmlWeb();

            string site = "https://www.zolotoy-zapas.ru/";

            var doc = await web.LoadFromWebAsync(site);

            List<double> prices = doc.DocumentNode
                .SelectNodes("//div[@class='coins-tile__price-val js-only-currency-rur']").ToList()
                .Take(4)
                .Select(x =>
                    double.Parse(
                        x.InnerText
                        .Replace("\n", "")
                        .Trim()))
                .ToList();

            var mmd = prices.Take(2).ToList();
            var spmd = prices.Skip(2).ToList();

            DateTime parseDate = DateTime.Now;

            return new List<CoinsRates>
            {
                new CoinsRates()
                {
                    Acronim = "GPM",
                    Sell = mmd[0],
                    Buy = mmd[1],
                    Date = parseDate,
                    Site = site
                },
                new CoinsRates()
                {
                    Acronim = "GPS",
                    Sell = spmd[0],
                    Buy = spmd[1],
                    Date = parseDate,
                    Site = site
                }
            };
        }

        public static async Task<List<CoinsRates>> GetCoinsRatesZolotoyClub()
        {
            var web = new HtmlWeb();

            string site = "https://www.zolotoy-club.ru/";

            var doc = await web.LoadFromWebAsync(site);

            double sellPriceSPMD = double.Parse(doc.DocumentNode
                .SelectNodes("//span[@style='color: rgb(253, 0, 0);']")
                .First().InnerText
                    .Replace("RU", "")
                    .Trim());

            double sellPriceMMD = double.Parse(doc.DocumentNode
                .SelectNodes("//span[@style='color: rgb(255, 3, 3);']")
                .First().InnerText
                    .Replace("RU", "")
                    .Trim());

            double buyPriceSPMD = double.Parse(doc.DocumentNode
                .SelectNodes("//span[@style='color: rgb(251, 0, 0);']")
                .First().InnerText
                    .Replace("RU", "")
                    .Trim());

            double buyPriceMMD = double.Parse(doc.DocumentNode
                .SelectNodes("//span[@style='color: rgb(255, 0, 0);']")
                .First().InnerText
                    .Replace("RU", "")
                    .Trim());

            DateTime parseDate = DateTime.Now;

            return new List<CoinsRates>
            {
                new CoinsRates()
                {
                    Acronim = "GPM",
                    Sell = sellPriceMMD,
                    Buy = buyPriceMMD,
                    Date = parseDate,
                    Site = site
                },
                new CoinsRates()
                {
                    Acronim = "GPS",
                    Sell = sellPriceSPMD,
                    Buy = buyPriceSPMD,
                    Date = parseDate,
                    Site = site
                }
            };
        }

        public static async Task<List<CoinsRates>> GetCoinsRatesZolotoMD()
        {
            var web = new HtmlWeb();

            string site = "https://zoloto-md.ru/bullion-coins";

            var doc = await web.LoadFromWebAsync(site);

            List<double> sellPrices = doc.DocumentNode
                .SelectNodes("//span[@class='js-price-club']").ToList()
                .Take(3)
                .Select(x =>
                    double.Parse(
                        x.InnerText
                        .Replace("Руб.", "")
                        .Trim()))
                .ToList();

            List<double> buyPrices = doc.DocumentNode
                .SelectNodes("//span[@class='js-price-buyout']").ToList()
                .Take(3)
                .Select(x =>
                    double.Parse(
                        x.InnerText
                        .Replace("Руб.", "")
                        .Trim()))
                .ToList();

            DateTime parseDate = DateTime.Now;

            return new List<CoinsRates>
            {
                new CoinsRates()
                {
                    Acronim = "GPM",
                    Sell = sellPrices[2],
                    Buy = buyPrices[2],
                    Date = parseDate,
                    Site = site
                },
                new CoinsRates()
                {
                    Acronim = "GPS",
                    Sell = sellPrices[0],
                    Buy = buyPrices[2],
                    Date = parseDate,
                    Site = site
                }
            };
        }
    }
}
