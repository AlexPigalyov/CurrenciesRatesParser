using CurrenciesRatesParser.Helpers;
using CurrenciesRatesParser.Mappers;
using CurrenciesRatesParser.Model;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
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
        public static async Task<List<CoinsRate>> GetCoinsRatesSberbank()
        {
            List<CoinsRate> rates = new List<CoinsRate>();

            using (WebClient client = new WebClient())
            {

                string json = await client.DownloadStringTaskAsync(UrlParseHelper.Sberbank);

                JObject o = JObject.Parse(json);
                string priceBuy = (string)o["priceBuy"];
                string priceSell = (string)o["price"];

                rates.Add(
                    new CoinsRate
                    {
                        Acronim = "GPM",
                        Sell = priceSell.ParseToDoubleFormat(),
                        Buy = priceBuy.ParseToDoubleFormat(),
                        Date = DateTime.Now,
                        Site = UrlParseHelper.Sberbank
                    });

                rates.Add(
                    new CoinsRate
                    {
                        Acronim = "GPS",
                        Sell = priceSell.ParseToDoubleFormat(),
                        Buy = priceBuy.ParseToDoubleFormat(),
                        Date = DateTime.Now,
                        Site = UrlParseHelper.Sberbank
                    });
            }

            return rates;
        }

        public static async Task<List<Rate>> GetMetalRates()
        {
            List<Rate> rates = new List<Rate>();

            var htmlCode = await LoadHtmlAsync(UrlParseHelper.Moex);
            var matches = Regex.Matches(htmlCode, "(?<=>)([1-9]+.[\\d,]+)+|([\\d,])(?=<)");
            int index = 0;

            foreach (Match item in matches)
            {
                if (index == 8)
                {
                    double price = item.Value.ParseToDoubleFormat();
                    rates.Add(new Rate()
                    {
                        Acronim = "XAU",
                        Sell = price,
                        Buy = price,
                        Date = DateTime.Now,
                        Site = UrlParseHelper.Moex
                    });
                }
                else if (index == 16)
                {
                    double price = item.Value.ParseToDoubleFormat();
                    rates.Add(new Rate()
                    {
                        Acronim = "PAL",
                        Sell = price,
                        Buy = price,
                        Date = DateTime.Now,
                        Site = UrlParseHelper.Moex
                    });
                }
                else if (index == 23)
                {
                    double price = item.Value.ParseToDoubleFormat();
                    rates.Add(new Rate()
                    {
                        Acronim = "PL",
                        Sell = price,
                        Buy = price,
                        Date = DateTime.Now,
                        Site = UrlParseHelper.Moex
                    });
                }
                else if (index == 30)
                {
                    double price = item.Value.ParseToDoubleFormat();
                    rates.Add(new Rate()
                    {
                        Acronim = "XAG",
                        Sell = price,
                        Buy = price,
                        Date = DateTime.Now,
                        Site = UrlParseHelper.Moex
                    });
                }
                else if (index > 30) break;

                index++;
            }

            return rates;
        }

        public static async Task<List<Rate>> GetExchangeRates()
        {
            DateTime parsingStartTime = DateTime.Now;

            List<Rate> rates = new List<Rate>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                List<string> urls = new List<string>()
                {
                    UrlParseHelper.RuExchangeRatesA,
                    UrlParseHelper.RuExchangeRatesP,
                    UrlParseHelper.RuExchangeRatesE,
                    UrlParseHelper.RuExchangeRatesM,
                    UrlParseHelper.RuExchangeRatesF
                };

                foreach (string currentUrl in urls)
                {
                    var htmlCode = await client.DownloadStringTaskAsync(currentUrl);
                    var ratesMatches = Regex.Matches(htmlCode, "(?<a>)([0-9]+,[0-9]+)+(?=<)");
                    var codeOfRatesMatches = Regex.Matches(htmlCode, "(?<a>)(Обмен.*?)+(?=</a>)");
                    for (int i = 0; i < ratesMatches.Count; i++)
                    {
                        double price = ratesMatches[i].Value.ParseToDoubleFormat();
                        rates.Add(new Rate()
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

        public static async Task<List<CoinsRate>> GetCoinsRatesZolotoyZapas()
        {
            var web = new HtmlWeb();

            var doc = await web.LoadFromWebAsync(UrlParseHelper.ZolotoyZapas);

            List<double> prices = doc.DocumentNode
                .SelectNodes("//div[@class='coins-tile__price-val js-only-currency-rur']").ToList()
                .Take(4)
                .Select(x =>
                    x.InnerText.ParseToDoubleFormat())
                .ToList();

            var mmd = prices.Take(2).ToList();
            var spmd = prices.Skip(2).ToList();

            DateTime parseDate = DateTime.Now;

            return new List<CoinsRate>
            {
                new CoinsRate()
                {
                    Acronim = "GPM",
                    Sell = mmd[0],
                    Buy = mmd[1],
                    Date = parseDate,
                    Site = UrlParseHelper.ZolotoyZapas
                },
                new CoinsRate()
                {
                    Acronim = "GPS",
                    Sell = spmd[0],
                    Buy = spmd[1],
                    Date = parseDate,
                    Site = UrlParseHelper.ZolotoyZapas
                }
            };
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesZolotoyClub()
        {
            var web = new HtmlWeb();

            var doc = await web.LoadFromWebAsync(UrlParseHelper.ZolotoyClub);

            double sellPriceSPMD = doc.DocumentNode
                .SelectNodes("//span[@style='color: rgb(253, 0, 0);']")
                .First().InnerText.Replace("RU", "").ParseToDoubleFormat();

            double sellPriceMMD = doc.DocumentNode
                .SelectNodes("//span[@style='color: rgb(255, 3, 3);']")
                .First().InnerText.Replace("RU", "").ParseToDoubleFormat();

            double buyPriceSPMD = doc.DocumentNode
                .SelectNodes("//span[@style='color: rgb(251, 0, 0);']")
                .First().InnerText.Replace("RU", "").ParseToDoubleFormat();

            double buyPriceMMD = doc.DocumentNode
                .SelectNodes("//span[@style='color: rgb(255, 0, 0);']")
                .First().InnerText.Replace("RU", "").ParseToDoubleFormat();

            DateTime parseDate = DateTime.Now;

            return new List<CoinsRate>
            {
                new CoinsRate()
                {
                    Acronim = "GPM",
                    Sell = sellPriceMMD,
                    Buy = buyPriceMMD,
                    Date = parseDate,
                    Site = UrlParseHelper.ZolotoyClub
                },
                new CoinsRate()
                {
                    Acronim = "GPS",
                    Sell = sellPriceSPMD,
                    Buy = buyPriceSPMD,
                    Date = parseDate,
                    Site = UrlParseHelper.ZolotoyClub
                }
            };
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesZolotoMD()
        {
            var web = new HtmlWeb();

            var doc = await web.LoadFromWebAsync(UrlParseHelper.ZolotoMD);

            List<double> sellPrices = doc.DocumentNode
                .SelectNodes("//span[@class='js-price-club']").ToList()
                .Take(3)
                .Select(x =>
                    x.InnerText.Replace("Руб.", "").ParseToDoubleFormat())
                .ToList();

            List<double> buyPrices = doc.DocumentNode
                .SelectNodes("//span[@class='js-price-buyout']").ToList()
                .Take(3)
                .Select(x =>
                    x.InnerText.Replace("Руб.", "").ParseToDoubleFormat())
                .ToList();

            DateTime parseDate = DateTime.Now;

            return new List<CoinsRate>
            {
                new CoinsRate()
                {
                    Acronim = "GPM",
                    Sell = sellPrices[2],
                    Buy = buyPrices[2],
                    Date = parseDate,
                    Site = UrlParseHelper.ZolotoMD
                },
                new CoinsRate()
                {
                    Acronim = "GPS",
                    Sell = sellPrices[0],
                    Buy = buyPrices[2],
                    Date = parseDate,
                    Site = UrlParseHelper.ZolotoMD
                }
            };
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesMonetaInvest()
        {
            List<CoinsRate> rates = new List<CoinsRate>();
            var web = new HtmlWeb();

            var doc = await web.LoadFromWebAsync(UrlParseHelper.MonetaInvest);

            var coints = doc.DocumentNode
                .SelectNodes("//div[@class='monet1']").ToList(); // выбираем блоки монет


            // фильтруем монеты по тексту
            var cointMMD = coints.FirstOrDefault(e =>
                e.InnerText.Contains("Георгий Победоносец") && e.InnerText.Contains("ММД"));
            var cointSPMD = coints.FirstOrDefault(e =>
                e.InnerText.Contains("Георгий Победоносец") && e.InnerText.Contains("СПМД"));

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
                new CoinsRate
                {
                    Acronim = "GPM",
                    Sell = saleCointMMD.ParseToDoubleFormat(),
                    Buy = buyCointMMD.ParseToDoubleFormat(),
                    Date = parseDate,
                    Site = UrlParseHelper.MonetaInvest
                });

            rates.Add(
                new CoinsRate
                {
                    Acronim = "GPS",
                    Sell = saleCointSPMD.ParseToDoubleFormat(),
                    Buy = buyCointSPMD.ParseToDoubleFormat(),
                    Date = parseDate,
                    Site = UrlParseHelper.MonetaInvest
                });

            return rates;
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesVFBank()
        {
            List<CoinsRate> rates = new List<CoinsRate>();
            var web = new HtmlWeb();

            var doc = await web.LoadFromWebAsync(UrlParseHelper.VfBank);

            var coints = doc.DocumentNode
                .SelectNodes("//div[@class='coin']").ToList(); // выбираем блоки монет


            // фильтруем монеты по тексту
            var cointMMD = coints.FirstOrDefault(e =>
                e.InnerText.Contains("Георгий Победоносец") && e.InnerText.Contains("ММД"));
            var cointSPMD = coints.FirstOrDefault(e =>
                e.InnerText.Contains("Георгий Победоносец") && e.InnerText.Contains("СПМД"));

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
                new CoinsRate
                {
                    Acronim = "GPM",
                    Sell = saleCointMMD.ParseToDoubleFormat(),
                    Buy = buyCointMMD.ParseToDoubleFormat(),
                    Date = parseDate,
                    Site = UrlParseHelper.VfBank
                });

            rates.Add(
                new CoinsRate
                {
                    Acronim = "GPS",
                    Sell = saleCointSPMD.ParseToDoubleFormat(),
                    Buy = buyCointSPMD.ParseToDoubleFormat(),
                    Date = parseDate,
                    Site = UrlParseHelper.VfBank
                });

            return rates;
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesRshbRu()
        {
            List<CoinsRate> coins = new List<CoinsRate>();

            var selectorGPM = "//a[@id='detailCoin_202657' and @class='b-coins-items-item-head-lnk']";

            var selectorGPS = "//a[@id='detailCoin_17891' and @class='b-coins-items-item-head-lnk']";

            coins.Add(await getRshbRuCoin(selectorGPM, "GPM"));
            coins.Add(await getRshbRuCoin(selectorGPS, "GPS"));

            return coins;
        }

        static async Task<CoinsRate> getRshbRuCoin(string xpath, string acronim)
        {
            HtmlWeb web = new HtmlWeb();

            var htmlDoc = await web.LoadFromWebAsync(UrlParseHelper.Rshb);

            var idLink = htmlDoc.DocumentNode.SelectSingleNode(xpath);

            var coinNode = idLink.ParentNode.ParentNode.ParentNode;

            var coinHtmlDoc = new HtmlDocument();
            coinHtmlDoc.LoadHtml("<div>" + coinNode.InnerHtml + "</div>");

            string buySelect = "//span[@class='b-coins-items-item-cost-b']";
            string sellSelect = "//div[@class='b-coins-items-item-quotes-price  ']";

            string buyPrice = coinHtmlDoc.DocumentNode.SelectSingleNode(buySelect).InnerText.Replace("Р", "");
            string sellPrice = coinHtmlDoc.DocumentNode.SelectSingleNode(sellSelect).InnerText.Replace("Р", "");

            CoinsRate coin = new CoinsRate()
            {
                Acronim = acronim,
                Date = DateTime.Now,
                Site = UrlParseHelper.Rshb
            };

            coin.Buy = buyPrice.ParseToDoubleFormat();
            coin.Sell = sellPrice.ParseToDoubleFormat();

            return coin;
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesRicgoldCom()
        {
            List<CoinsRate> coins = new List<CoinsRate>();

            coins.Add(await getCoinRateRicgoldCom(UrlParseHelper.RicGoldMMD, "GPM"));
            coins.Add(await getCoinRateRicgoldCom(UrlParseHelper.RicGoldSPMD, "GPS"));

            return coins;
        }

        static async Task<CoinsRate> getCoinRateRicgoldCom(string url, string acronim)
        {
            string loadedHtml = await LoadHtmlAsync(url);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(loadedHtml);

            string xpath = "//div[@class='bull_price']";

            var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);

            var nodeText = node.InnerText;

            var prices = Regex.Replace(nodeText, @"[^0-9,:]+", "", RegexOptions.ECMAScript);

            var priceList = prices.Split(':').ToArray();

            int whiteCount = 0;

            foreach (var str in priceList)
            {
                if (str == "")
                {
                    ++whiteCount;
                }
            }

            CoinsRate coin = new CoinsRate()
            {
                Acronim = acronim,
                Date = DateTime.Now,
                Site = url
            };

            if (whiteCount > 1)
            {
                if (priceList[1] != "")
                {
                    coin.Sell = priceList[1].ParseToDoubleFormat();
                }
                else
                {
                    coin.Sell = 0;
                }

                if (priceList[2] != "")
                {
                    coin.Buy = priceList[2].ParseToDoubleFormat();
                }
                else
                {
                    coin.Buy = 0;
                }
            }
            else
            {
                coin.Sell = priceList[1].ParseToDoubleFormat();
                coin.Buy = priceList[2].ParseToDoubleFormat();
            }

            return coin;
        }

        static async Task<string> LoadHtmlAsync(string url)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string htmlCode = await client.DownloadStringTaskAsync(url);
                return htmlCode;
            }
        }

        public static async Task<List<CoinsRate>> GetCoinsRates9999dRu()
        {
            List<CoinsRate> coins = new List<CoinsRate>();

            var nodesXPathMMD = "//div[@id='bx_651765591_51184']/div[@class='inner-wrap']/div[@class='text']";

            var nodesXPathSPDM = "//div[@id='bx_651765591_51290']/div[@class='inner-wrap']/div[@class='text']";

            coins.Add(await parseCoin(nodesXPathMMD, "GPM"));
            coins.Add(await parseCoin(nodesXPathSPDM, "GPS"));

            return coins;
        }

        static async Task<CoinsRate> parseCoin(string xpath, string acronim)
        {
            HtmlWeb web = new HtmlWeb();

            var htmlDoc = await web.LoadFromWebAsync(UrlParseHelper.Site9999d);

            var selector = "//div[@class='catalog item-views table catalog_table_2' and @data-slice='Y']";

            var node = htmlDoc.DocumentNode.SelectSingleNode(selector);

            var htmlDoc2 = new HtmlDocument();
            htmlDoc2.LoadHtml("<div>" + node.InnerHtml + "</div>");

            var nodeCoin = htmlDoc2.DocumentNode.SelectSingleNode(xpath);

            var innerText = Regex.Replace(nodeCoin.InnerText, @"\s+", " ");

            var prices = GetBetweenTwoWords("ПРОДАЖА", "Цена за грамм", innerText);

            var pricePair = prices.Split(new string[1] { "ПОКУПКА" }, StringSplitOptions.RemoveEmptyEntries);

            pricePair = pricePair.Select(x => x.Replace("₽", "")).ToArray();

            CoinsRate coin = new CoinsRate()
            {
                Date = DateTime.Now,
                Site = UrlParseHelper.Site9999d,
                Acronim = acronim
            };

            coin.Sell = pricePair[0].ParseToDoubleFormat();
            coin.Buy = pricePair[1].ParseToDoubleFormat();

            return coin;
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesLantaRu()
        {
            DateTime dateOfParse = DateTime.Now;

            var web = new HtmlWeb();

            var htmlDoc = await web.LoadFromWebAsync(UrlParseHelper.LantaRu);

            var coins = htmlDoc.DocumentNode.SelectNodes("//div[@class='coinList-cont']").Where(x =>
                x.InnerText.Contains("Георгий Победоносец"));

            List<double> mmdPrices = new List<double>();
            List<double> spmdPrices = new List<double>();

            foreach (var coinHtml in coins)
            {
                if (coinHtml.InnerText.Contains("ММД"))
                {
                    mmdPrices = GetCoinPricesLantaRu(coinHtml);
                }
                else if (coinHtml.InnerText.Contains("СПМД"))
                {
                    spmdPrices = GetCoinPricesLantaRu(coinHtml);
                }
            }

            return new List<CoinsRate>()
            {
                new CoinsRate()
                {
                    Acronim = "GPM",
                    Buy = mmdPrices[1],
                    Sell = mmdPrices[0],
                    Date = dateOfParse,
                    Site = UrlParseHelper.LantaRu
                },
                new CoinsRate()
                {
                    Acronim = "GPS",
                    Buy = spmdPrices[1],
                    Sell = spmdPrices[0],
                    Date = dateOfParse,
                    Site = UrlParseHelper.LantaRu
                }
            };
        }

        private static List<double> GetCoinPricesLantaRu(HtmlNode coinHtml)
        {
            List<double> coinPrices = new List<double>();

            var localPrices = coinHtml.ChildNodes
                .Where(x => x.Name != "#text")
                .FirstOrDefault(x => x.Attributes["class"].Value == "coinList-price" || x.Attributes["class"].Value == "coinList-price out").InnerText
                .Split('\n')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            foreach (var priceString in localPrices)
            {
                try
                {
                    var price = priceString.Replace("₽", "").Replace("покупка", "").ParseToDoubleFormat();

                    coinPrices.Add(price);
                }
                catch
                {
                    coinPrices.Add(0);
                }
            }

            return coinPrices;
        }

        public static string GetBetweenTwoWords(string firstWord, string secondWord, string str)
        {
            var firstWordIndex = str.IndexOf(firstWord) + firstWord.Length;
            var secondWordIndex = str.IndexOf(secondWord);
            return str.Substring(firstWordIndex, secondWordIndex - firstWordIndex);
        }

        public static async Task<List<CoinsRate>> GetCoinsRateTsBnk()
        {
            DateTime parseDate = DateTime.Now;

            var web = new HtmlWeb();

            var htmlDocSPMD = await web.LoadFromWebAsync(UrlParseHelper.CoinsTsbnkSPMD);
            var htmlDocMMD = await web.LoadFromWebAsync(UrlParseHelper.CoinsTsbnkMMD);

            var SPMDSell = htmlDocSPMD.DocumentNode
                .SelectNodes("//div[@class='product-price']/div[@class='price aligner']/span[@itemprop='price']")
                .First().InnerText.Replace("руб.", "").ParseToDoubleFormat();
            var SPMDBuy = htmlDocSPMD.DocumentNode
                .SelectNodes("//div[@class='product-action']/div[@class='redemption-price']/p/span[@class='r-price']")
                .First().InnerText.Replace("руб.", "").ParseToDoubleFormat();

            var MMDSell = htmlDocMMD.DocumentNode
                .SelectNodes("//div[@class='product-price']/div[@class='price aligner']/span[@itemprop='price']")
                .First().InnerText.Replace("руб.", "").ParseToDoubleFormat();
            var MMDByu = htmlDocMMD.DocumentNode
                .SelectNodes("//div[@class='product-action']/div[@class='redemption-price']/p/span[@class='r-price']")
                .First().InnerText.Replace("руб.", "").ParseToDoubleFormat();

            return new List<CoinsRate>()
            {
                new CoinsRate()
                {
                    Acronim = "GPM",
                    Sell = MMDSell,
                    Buy = MMDByu,
                    Date = parseDate,
                    Site = UrlParseHelper.CoinsTsbnkMMD
                },
                new CoinsRate()
                {
                    Acronim = "GPS",
                    Sell = SPMDSell,
                    Buy = SPMDBuy,
                    Date = parseDate,
                    Site = UrlParseHelper.CoinsTsbnkSPMD
                }
            };
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesZolotoyDvor()
        {
            DateTime parseDate = DateTime.Now;

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = await web.LoadFromWebAsync(UrlParseHelper.ZolotoyDvor);

            var spmdCoinHtml = htmlDoc.DocumentNode.SelectNodes("//tr")
                .FirstOrDefault(x =>
                    x.InnerHtml.Contains("Георгий Победоносец") && x.InnerText.Contains("СПМД") &&
                    x.InnerText.Contains("2021") && x.ChildNodes.Count == 6);

            var mmdCoinHtml = htmlDoc.DocumentNode.SelectNodes("//tr")
                .FirstOrDefault(x =>
                    x.InnerHtml.Contains("Георгий Победоносец") && x.InnerText.Contains("ММД") &&
                    x.InnerText.Contains("2021") && x.ChildNodes.Count == 6);

            var mmdPrices = GetCoinPricesZolotoyDvor(mmdCoinHtml);
            var spmdPrices = GetCoinPricesZolotoyDvor(spmdCoinHtml);

            return new List<CoinsRate>
            {
                new CoinsRate()
                {
                    Acronim = "GPM",
                    Sell = mmdPrices[1],
                    Buy = mmdPrices[0],
                    Date = parseDate,
                    Site = UrlParseHelper.ZolotoyDvor
                },
                new CoinsRate()
                {
                    Acronim = "GPS",
                    Sell = spmdPrices[1],
                    Buy = spmdPrices[0],
                    Date = parseDate,
                    Site = UrlParseHelper.ZolotoyDvor
                }
            };
        }

        private static List<double> GetCoinPricesZolotoyDvor(HtmlNode coinHtmlNode)
        {
            var coins = coinHtmlNode.ChildNodes.Where(x =>
                    x.Name != "#text" &&
                    x.Attributes["height"] != null &&
                    !string.IsNullOrEmpty(x.LastChild.InnerText) &&
                    x.InnerText != "&#10008;")
                .Select(x =>
                    x.LastChild.InnerText
                        .Replace("&nbsp;", "")
                        .Replace("руб.", "")
                        .Replace("\'", "")
                        .ParseToDoubleFormat())
                .ToList();

            if (coins.Count == 1)
            {
                coins.Add(0);
            }

            return coins;
        }
    }
}