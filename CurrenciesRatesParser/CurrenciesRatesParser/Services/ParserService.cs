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
                    else if (index > 30) break;
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

        public static async Task<List<CoinsRates>> GetCoinsRatesMonetaInvest()
        {
            List<CoinsRates> rates = new List<CoinsRates>();
            var web = new HtmlWeb();

            string site = "https://msk.monetainvest.ru/";

            var doc = await web.LoadFromWebAsync(site);

            var coints = doc.DocumentNode
                .SelectNodes("//div[@class='monet1']").ToList(); // выбираем блоки монет


            // фильтруем монеты по тексту
            var cointMMD = coints.FirstOrDefault(e => e.InnerText.Contains("Георгий Победоносец") && e.InnerText.Contains("ММД"));
            var cointSPMD = coints.FirstOrDefault(e => e.InnerText.Contains("Георгий Победоносец") && e.InnerText.Contains("СПМД"));

            string saleCointMMD;
            string saleCointSPMD;

            try
            {
                saleCointMMD = cointMMD.InnerText
                              .Split(new string[] { "Продажа:" }, StringSplitOptions.None)[1]
                            .Split('р')[0]
                            .Trim();
            }
            catch
            {
                saleCointMMD = "0";
            }

            try
            {
                saleCointSPMD = cointSPMD.InnerText
                              .Split(new string[] { "Продажа:" }, StringSplitOptions.None)[1]
                            .Split('р')[0]
                            .Trim();
            }
            catch
            {
                saleCointSPMD = "0";
            }

            var buyCointMMD = cointMMD.InnerText
                .Split(new string[] { "Покупка:" }, StringSplitOptions.None)[1]
              .Split('р')[0]
              .Trim();

            var buyCointSPMD = cointSPMD.InnerText
               .Split(new string[] { "Покупка:" }, StringSplitOptions.None)[1]
             .Split('р')[0]
             .Trim();

            DateTime parseDate = DateTime.Now;

            rates.Add(
                    new CoinsRates
                    {
                        Acronim = "GPM",
                        Sell = double.Parse(saleCointMMD),
                        Buy = double.Parse(buyCointMMD),
                        Date = parseDate,
                        Site = site
                    });

            rates.Add(
                   new CoinsRates
                   {
                       Acronim = "GPS",
                       Sell = double.Parse(saleCointSPMD),
                       Buy = double.Parse(buyCointSPMD),
                       Date = parseDate,
                       Site = site
                   });

            return rates;
        }

        public static async Task<List<CoinsRates>> GetCoinsRatesVFBank()
        {
            List<CoinsRates> rates = new List<CoinsRates>();
            var web = new HtmlWeb();

            string site = "https://www.vfbank.ru/fizicheskim-licam/monety/";

            var doc = web.LoadFromWebAsync(site).Result;

            var coints = doc.DocumentNode
                .SelectNodes("//div[@class='coin']").ToList(); // выбираем блоки монет


            // фильтруем монеты по тексту
            var cointMMD = coints.FirstOrDefault(e => e.InnerText.Contains("Георгий Победоносец") && e.InnerText.Contains("ММД"));
            var cointSPMD = coints.FirstOrDefault(e => e.InnerText.Contains("Георгий Победоносец") && e.InnerText.Contains("СПМД"));

            string saleCointMMD;
            string saleCointSPMD;

            try
            {
                saleCointMMD = cointMMD.InnerText
                              .Split(new string[] { "Продажа:" }, StringSplitOptions.None)[1]
                            .Split('₽')[0]
                            .Trim();
            }
            catch
            {
                saleCointMMD = "0";
            }

            try
            {
                saleCointSPMD = cointSPMD.InnerText
                              .Split(new string[] { "Продажа:" }, StringSplitOptions.None)[1]
                            .Split('₽')[0]
                            .Trim();
            }
            catch
            {
                saleCointSPMD = "0";
            }

            var buyCointMMD = cointMMD.InnerText
                .Split(new string[] { "Покупка:" }, StringSplitOptions.None)[1]
              .Split('₽')[0]
              .Trim();

            var buyCointSPMD = cointSPMD.InnerText
               .Split(new string[] { "Покупка:" }, StringSplitOptions.None)[1]
             .Split('₽')[0]
             .Trim();

            DateTime parseDate = DateTime.Now;

            rates.Add(
                    new CoinsRates
                    {
                        Acronim = "GPM",
                        Sell = double.Parse(saleCointMMD),
                        Buy = double.Parse(buyCointMMD),
                        Date = parseDate,
                        Site = site
                    });

            rates.Add(
                   new CoinsRates
                   {
                       Acronim = "GPS",
                       Sell = double.Parse(saleCointSPMD),
                       Buy = double.Parse(buyCointSPMD),
                       Date = parseDate,
                       Site = site
                   });

            return rates;
        }


        public static async Task<List<CoinsRates>> GetCoinsRatesRicgoldCom()
        {
            string urlMMD = @"https://www.ricgold.com/shop/investitsionnye-monety-6/zolotaya--moneta-georgij-pobedonosets-mmd-2006-2020-gg-388/";
            string urlSPMD = "https://www.ricgold.com/shop/investitsionnye-monety-6/zolotaya-moneta-georgij-pobedonosets-spmd-2006-2021-gg-1830/";
            
            List<CoinsRates> coins = new List<CoinsRates>();

            coins.Add(getCoinRateRicgoldCom(urlMMD, "GPM").Result);
            coins.Add(getCoinRateRicgoldCom(urlSPMD, "GPS").Result);

            return coins;
        }

        static async Task<CoinsRates> getCoinRateRicgoldCom(string url,string acronim)
        {
            string loadedHtml = await loadHtml(url);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(loadedHtml);

            string xpath = "//div[@class='bull_price']";

            var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);

            var nodeText = node.InnerText;

            var prices = Regex.Replace(nodeText, @"[^0-9,:]+", "", RegexOptions.ECMAScript);

            var priceList = prices.Split(':').ToArray();

            int whiteCount = 0;

            foreach(var str in priceList)
            {
                if (str == "")
                {
                    ++whiteCount;
                }
            }

            CoinsRates coin = new CoinsRates()
            {
                Acronim = acronim,
                Date = DateTime.Now,
                Site = url
            };

            if (whiteCount>1)
            {
                if (priceList[1] != "")
                {
                    coin.Sell = double.Parse(priceList[1]);
                }
                else
                {
                    coin.Sell = 0;
                }
                if (priceList[2] != "")
                {
                    coin.Buy = double.Parse(priceList[2]);
                }
                else
                {
                    coin.Buy = 0;
                }

            }
            else
            {
                coin.Sell = double.Parse(priceList[1]);
                coin.Buy = double.Parse(priceList[2]);
            }

            return coin;
        }


        static async Task<string> loadHtml(string url)
        {
            using (WebClient client = new WebClient())
            {
                string htmlCode = await client.DownloadStringTaskAsync(url);
                return htmlCode;
            }
        }



        public static async Task<List<CoinsRates>> GetCoinsRates9999dRu()
        {
            List<CoinsRates> coins = new List<CoinsRates>();

            var nodesXPathMMD = "//div[@id='bx_651765591_51184']/div[@class='inner-wrap']/div[@class='text']";

            var nodesXPathSPDM = "//div[@id='bx_651765591_51290']/div[@class='inner-wrap']/div[@class='text']";


            coins.Add(parseCoin(nodesXPathMMD, "GPM").Result);
            coins.Add(parseCoin(nodesXPathSPDM, "GPS").Result);

            return coins;
        }

        static async Task<CoinsRates> parseCoin(string xpath, string acronim)
        {
            var site = @"https://9999d.ru/";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = await web.LoadFromWebAsync(site);

            var selector = "//div[@class='catalog item-views table catalog_table_2' and @data-slice='Y']";

            var node = htmlDoc.DocumentNode.SelectSingleNode(selector);


            var htmlDoc2 = new HtmlDocument();
            htmlDoc2.LoadHtml("<div>" + node.InnerHtml + "</div>");

            var nodeCoin = htmlDoc2.DocumentNode.SelectSingleNode(xpath);

            var innerText = Regex.Replace(nodeCoin.InnerText, @"\s+", " ");

            var prices = GetBetweenTwoWords("ПРОДАЖА", "Цена за грамм", innerText);

            var pricePair = prices.Split(new string[1] { "ПОКУПКА" }, StringSplitOptions.RemoveEmptyEntries);

            pricePair = pricePair.Select(x => x.Replace("₽", "")).ToArray();

            CoinsRates coin = new CoinsRates()
            {
                Date = DateTime.Now,
                Site = site,
                Acronim = acronim
            };

            coin.Sell = double.Parse(pricePair[0]);
            coin.Buy = double.Parse(pricePair[1]);

            return coin;
        }

        public static string GetBetweenTwoWords(string firstWord, string secondWord, string str)
        {
            var firstWordIndex = str.IndexOf(firstWord) + firstWord.Length;
            var secondWordIndex = str.IndexOf(secondWord);
            return str.Substring(firstWordIndex, secondWordIndex - firstWordIndex);
        }


    }
}
