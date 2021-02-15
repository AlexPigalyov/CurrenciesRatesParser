﻿using CurrenciesRatesParser.Model;
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
        public static async Task<List<Rates>> GetCurrencyRates()
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
            List<Rates> rates = new List<Rates>();
            var web = new HtmlWeb();

            string site = "https://www.zolotoy-zapas.ru/";

            var doc = web.Load(site);

            List<double> prices = doc.DocumentNode
                .SelectNodes("//div[@class='coins-tile__price-val js-only-currency-rur']").ToList()
                .Select(x => 
                    double.Parse(
                        x.InnerText
                        .Replace("\n", "")
                        .Trim()))
                .Take(4).ToList();

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
    }
}