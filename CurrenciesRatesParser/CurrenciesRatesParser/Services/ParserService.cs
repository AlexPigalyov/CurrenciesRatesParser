using CurrenciesRatesParser.Helpers;
using CurrenciesRatesParser.Mappers;
using CurrenciesRatesParser.Model;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ratesRatesParser.Services
{
    public static class ParserService
    {
        public static async Task<List<CoinsRate>> GetCoinsRatesSberbank()
        {
            try
            {
                List<CoinsRate> rates = new List<CoinsRate>();

                using (WebClient client = new WebClient())
                {
                    string json = await client.DownloadStringTaskAsync(UrlParseHelper.SberbankCoinsUrl);

                    JObject o = JObject.Parse(json);
                    string priceBuy = (string)o["priceBuy"];
                    string priceSell = (string)o["price"];
                    DateTime parseDate = DateTime.Now;

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

                Console.WriteLine("Parse coins rates Sberbank OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));

                return rates;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parse coins rates Sberbank. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);

                return null;
            }
        }

        public static async Task<List<Rate>> GetMetalRates()
        {
            try
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

                Console.WriteLine("Parse metal rates OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));

                return rates;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parse metal rates. Time: {0}. Error: {1}.", DateTime.Now.ToString("HH:mm:ss"),
                    e.Message);

                return null;
            }
        }

        public static async Task<List<Rate>> GetExchangeRates()
        {
            try
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

                Console.WriteLine("Parse exchange rates OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));

                return rates;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parse exchange rates. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);

                return null;
            }
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesZolotoyZapas()
        {
            try
            {
                var web = new HtmlWeb();

                var docSPMD = await web.LoadFromWebAsync(UrlParseHelper.ZolotoyZapasSPMD);
                var docMMD = await web.LoadFromWebAsync(UrlParseHelper.ZolotoyZapasMMD);

                var sellSelect = "//table[@class='card__table-purchase']/tbody/tr/td[@class='card__cell-cost']";
                var buySelect = "//table[@class='card__table-sale']/tbody/tr/td[@class='card__cell-cost']";

                double sellSPMDPrice = 0;
                try
                {
                    sellSPMDPrice = docSPMD.DocumentNode.SelectSingleNode(sellSelect).InnerHtml.Replace("<i class=\"icon-rouble\"></i>\n", "").ParseToDoubleFormat();
                }
                catch
                {
                    sellSPMDPrice = 0;
                }

                double sellMMDPrice = 0;
                try
                {
                    sellMMDPrice = docMMD.DocumentNode.SelectSingleNode(sellSelect).InnerHtml.Replace("<i class=\"icon-rouble\"></i>\n", "").ParseToDoubleFormat();
                }
                catch
                {
                    sellMMDPrice = 0;
                }

                double buySPMDPrice = 0;
                try
                {
                    buySPMDPrice = docSPMD.DocumentNode.SelectSingleNode(buySelect).InnerHtml.Replace("<i class=\"icon-rouble\"></i>\n", "").ParseToDoubleFormat();
                }
                catch
                {
                    buySPMDPrice = 0;
                }
                double buyMMDPrice = 0;
                try
                {
                    buyMMDPrice = docMMD.DocumentNode.SelectSingleNode(buySelect).InnerHtml.Replace("<i class=\"icon-rouble\"></i>\n", "").ParseToDoubleFormat();
                }
                catch
                {
                    buyMMDPrice = 0;
                }



                DateTime parseDate = DateTime.Now;

                Console.WriteLine("Parse coins rates zolotoy zapas OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));

                return new List<CoinsRate>
                {
                    new CoinsRate()
                    {
                        Acronim = "GPM",
                        Sell = sellMMDPrice,
                        Buy = buyMMDPrice,
                        Date = parseDate,
                        Site = UrlParseHelper.ZolotoyZapas
                    },
                    new CoinsRate()
                    {
                        Acronim = "GPS",
                        Sell = sellSPMDPrice,
                        Buy = buySPMDPrice,
                        Date = parseDate,
                        Site = UrlParseHelper.ZolotoyZapas
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parse coins rates zolotoy zapas. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);

                return null;
            }
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesZolotoyClub()
        {
            try
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

                Console.WriteLine("Parse coins rates Zolotoy Club OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));

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
            catch (Exception e)
            {
                Console.WriteLine("Error parse coins rates zolotoy Club. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);
                return null;
            }
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesZolotoMD()
        {
            try
            {
                var web = new HtmlWeb();

                var docSMPD = await web.LoadFromWebAsync(UrlParseHelper.ZolotoMDSPMD);
                var docMMD = await web.LoadFromWebAsync(UrlParseHelper.ZolotoMDMMD);
                double sellPricesSPMD = docSMPD.DocumentNode
                        .SelectNodes("//div[@class = 'product_price']/span/div").First().InnerHtml.ParseToDoubleFormat();
                double buyPricesSPMD = docSMPD.DocumentNode
                    .SelectNodes(
                        "//div[@class = 'product_price product_price__buyout']/span/div[@class = 'js-price-buyout']")
                    .First().InnerHtml.ParseToDoubleFormat();
                double sellPricesMMD = docMMD.DocumentNode
                    .SelectNodes("//div[@class = 'product_price']/span/div").First().InnerHtml.ParseToDoubleFormat();
                double buyPricesMMD = docMMD.DocumentNode
                    .SelectNodes(
                        "//div[@class = 'product_price product_price__buyout']/span/div[@class = 'js-price-buyout']")
                    .First().InnerHtml.ParseToDoubleFormat();
                DateTime parseDate = DateTime.Now;
                Console.WriteLine("Parse coins rates Zoloto MD OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));

                return new List<CoinsRate>
                {
                    new CoinsRate()
                    {
                        Acronim = "GPM",
                        Sell = sellPricesMMD,
                        Buy = buyPricesMMD,
                        Date = parseDate,
                        Site = UrlParseHelper.ZolotoMD
                    },
                    new CoinsRate()
                    {
                        Acronim = "GPS",
                        Sell = sellPricesSPMD,
                        Buy = buyPricesSPMD,
                        Date = parseDate,
                        Site = UrlParseHelper.ZolotoMD
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parse coins rates Zoloto MD. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);
                return null;

            }
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesMonetaInvest()
        {
            try
            {
                List<CoinsRate> rates = new List<CoinsRate>();
                var web = new HtmlWeb();

                var docSpmd = await web.LoadFromWebAsync(UrlParseHelper.MonetaInvestSPMD);
                var docMmd = await web.LoadFromWebAsync(UrlParseHelper.MonetaInvestMMD);

                var sellSelect = "//span[@class='mon-buy']";
                var buySelect = "//div[@class='infoblock block-1']/div[11]";


                double sellSPMDPrice = 0;
                try
                {
                    sellSPMDPrice = docSpmd.DocumentNode.SelectSingleNode(sellSelect).InnerText.Replace("Продаем:", "").Replace(" р", "").ParseToDoubleFormat();
                }
                catch
                {
                    sellSPMDPrice = 0;
                }

                double sellMMDPrice = 0;
                try
                {
                    sellMMDPrice = docMmd.DocumentNode.SelectSingleNode(sellSelect).InnerText.Replace("Продаем:", "").Replace(" р", "").ParseToDoubleFormat();

                }
                catch
                {
                    sellMMDPrice = 0;
                }

                double buySPMDPrice = 0;
                try
                {
                    buySPMDPrice = docSpmd.DocumentNode.SelectSingleNode(buySelect).InnerHtml.Replace("Покупаем: ", "").Replace(" \n\t", "").Replace(" р", "").ParseToDoubleFormat();

                }
                catch
                {
                    buySPMDPrice = 0;
                }
                
                double buyMMDPrice;
                try
                {
                    buyMMDPrice = docMmd.DocumentNode.SelectSingleNode(buySelect).InnerHtml.Replace("Покупаем: ", "").Replace(" \n\t", "").Replace(" р", "").ParseToDoubleFormat();
                }
                catch
                {
                    buyMMDPrice = 0;
                }

                DateTime parseDate = DateTime.Now;
                Console.WriteLine("Parse coins rates MonetaInvest OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));

                rates.Add(
                    new CoinsRate
                    {
                        Acronim = "GPM",
                        Sell = sellMMDPrice,
                        Buy = buyMMDPrice,
                        Date = parseDate,
                        Site = UrlParseHelper.MonetaInvest
                    });

                rates.Add(
                    new CoinsRate
                    {
                        Acronim = "GPS",
                        Sell = sellSPMDPrice,
                        Buy = buySPMDPrice,
                        Date = parseDate,
                        Site = UrlParseHelper.MonetaInvest
                    });
                return rates;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parse coins rates Moneta Invest. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);

                return null;
            }
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesVFBank()
        {
            try
            {
                List<CoinsRate> rates = new List<CoinsRate>();
                var web = new HtmlWeb();

                var doc = await web.LoadFromWebAsync(UrlParseHelper.VfBankCoinUrl);

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
                Console.WriteLine("Parse coins rates VF Bank OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));
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
            catch (Exception e)
            {
                Console.WriteLine("Error parse coins rates VF Bank. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);

                return null;
            }
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesRshbRu()
        {
            try
            {
                List<CoinsRate> coins = new List<CoinsRate>();

                var selectorGPM = "//a[@id='detailCoin_202657' and @class='b-coins-items-item-head-lnk']";

                var selectorGPS = "//a[@id='detailCoin_17891' and @class='b-coins-items-item-head-lnk']";

                coins.Add(await getRshbRuCoin(selectorGPM, "GPM"));
                coins.Add(await getRshbRuCoin(selectorGPS, "GPS"));

                Console.WriteLine("Parse coins rates rshb ru OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));

                return coins;
            }

            catch (Exception e)
            {
                Console.WriteLine("Error parse coins rates rshb ru. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);

                return null;
            }
        }

        static async Task<CoinsRate> getRshbRuCoin(string xpath, string acronim)
        {
            HtmlWeb web = new HtmlWeb();

            var htmlDoc = await web.LoadFromWebAsync(UrlParseHelper.RshbCoinUrl);

            var idLink = htmlDoc.DocumentNode.SelectSingleNode(xpath);

            var coinNode = idLink.ParentNode.ParentNode.ParentNode;

            var coinHtmlDoc = new HtmlDocument();
            coinHtmlDoc.LoadHtml("<div>" + coinNode.InnerHtml + "</div>");

            string buySelect = "//span[@class='b-coins-items-item-cost-b']";
            string sellSelect = "//div[@class='b-coins-items-item-quotes-price  ']";

            string buyPrice = "0";
            try
            {
                buyPrice = coinHtmlDoc.DocumentNode.SelectSingleNode(buySelect).InnerText.Replace("Р", "");
            }
            catch (Exception e)
            {
                buyPrice = "0";
            }

            string sellPrice = "0";
            try
            {
                sellPrice = coinHtmlDoc.DocumentNode.SelectSingleNode(sellSelect).InnerText.Replace("Р", "");

            }
            catch (Exception e)
            {
                sellPrice = "0";
            }

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
                Site = UrlParseHelper.RicGold
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
            try
            {
                var web = new HtmlWeb();

                var htmlDoc = await web.LoadFromWebAsync(UrlParseHelper.LantaRuCoinsUrl);

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

                DateTime dateOfParse = DateTime.Now;
                Console.WriteLine("Parse coins rates LantaRu OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));
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
            catch (Exception e)
            {
                Console.WriteLine("Error parse coins rates LantaRu. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);

                return null;
            }
        }

        private static List<double> GetCoinPricesLantaRu(HtmlNode coinHtml)
        {
            List<double> coinPrices = new List<double>();

            var localPrices = coinHtml.ChildNodes
                .Where(x => x.Name != "#text")
                .FirstOrDefault(x =>
                    x.Attributes["class"].Value == "coinList-price" ||
                    x.Attributes["class"].Value == "coinList-price out").InnerText
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
            try
            {
                var web = new HtmlWeb();

                var htmlDocSPMD = await web.LoadFromWebAsync(UrlParseHelper.CoinsTsbnkSPMD);
                var htmlDocMMD = await web.LoadFromWebAsync(UrlParseHelper.CoinsTsbnkMMD);

                var SPMDSell = htmlDocSPMD.DocumentNode
                    .SelectNodes("//div[@class='product-price']/div[@class='price aligner']/span[@itemprop='price']")
                    .First().InnerText.Replace("руб.", "").ParseToDoubleFormat();
                var SPMDBuy = htmlDocSPMD.DocumentNode
                    .SelectNodes(
                        "//div[@class='product-action']/div[@class='redemption-price']/p/span[@class='r-price']")
                    .First().InnerText.Replace("руб.", "").ParseToDoubleFormat();

                var MMDSell = htmlDocMMD.DocumentNode
                    .SelectNodes("//div[@class='product-price']/div[@class='price aligner']/span[@itemprop='price']")
                    .First().InnerText.Replace("руб.", "").ParseToDoubleFormat();
                var MMDByu = htmlDocMMD.DocumentNode
                    .SelectNodes(
                        "//div[@class='product-action']/div[@class='redemption-price']/p/span[@class='r-price']")
                    .First().InnerText.Replace("руб.", "").ParseToDoubleFormat();
                DateTime parseDate = DateTime.Now;

                Console.WriteLine("Parse coins rates Ts Bnk OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));

                return new List<CoinsRate>()
                {
                    new CoinsRate()
                    {
                        Acronim = "GPM",
                        Sell = MMDSell,
                        Buy = MMDByu,
                        Date = parseDate,
                        Site = UrlParseHelper.CoinsTsbnk
                    },
                    new CoinsRate()
                    {
                        Acronim = "GPS",
                        Sell = SPMDSell,
                        Buy = SPMDBuy,
                        Date = parseDate,
                        Site = UrlParseHelper.CoinsTsbnk
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parse coins rates Ts Bnk. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);

                return null;
            }
        }

        public static async Task<List<CoinsRate>> GetCoinsRatesZolotoyDvor()
        {
            try
            {
                HtmlWeb web = new HtmlWeb();

                var htmlDoc = await web.LoadFromWebAsync(UrlParseHelper.ZolotoyDvorCoinUrl);

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

                DateTime parseDate = DateTime.Now;
                Console.WriteLine("Parse coins rates ZolotoyDvor OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));

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
            catch (Exception e)
            {
                Console.WriteLine("Error parse coins rates ZolotoyDvor. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);

                return null;
            }
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

        public static async Task<List<CoinsRate>> GetCoinsRatesMkdRu()
        {
            try
            {
                var option = new ChromeOptions();
                option.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

                IWebDriver driver = new ChromeDriver(
                    Path.Combine(
                        Directory.GetCurrentDirectory(), "SeleniumChromeWebDriver")
                    , option);

                driver.Navigate().GoToUrl((UrlParseHelper.MkbRu));

                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);

                Task.Delay(5000).Wait();


                string innerHtml = driver.PageSource;

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();


                doc.LoadHtml(innerHtml);

                var coinPrice = doc.DocumentNode.SelectNodes("//div[@class='cols cols3']")
                    .FirstOrDefault().ChildNodes
                    .Where(x => x.InnerText.Contains("Георгий Победоносец"))
                    .Where(x => x.Name != "#text")
                    .FirstOrDefault()
                    .LastChild.InnerHtml.Split()[0];
                DateTime parseDate = DateTime.Now;

                Console.WriteLine("Parse coins rates MkdRu OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));
                driver.Close();
                return new List<CoinsRate>
                {
                    new CoinsRate()
                    {
                        Date = DateTime.Now,
                        Site = UrlParseHelper.MkbRu,
                        Acronim = "GPM",
                        Buy = double.Parse(coinPrice),
                        Sell = 0,
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parse coins rates MkdRu. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);

                return null;
            }


        }

        public static async Task<List<CoinsRate>> GetCoinsRatesTkbbank()
        {
            try
            {
                HtmlWeb web = new HtmlWeb();

                var htmlDoc = await web.LoadFromWebAsync(UrlParseHelper.TkbbankCoinUrl);
                double coinsBuy;
                try
                {
                    coinsBuy = htmlDoc.DocumentNode.SelectNodes("//table[@class='tbl-switcher']/tbody/tr/td[6]")
                        .First().InnerHtml.ParseToDoubleFormat();
                }
                catch (Exception e)
                {
                    coinsBuy = 0;
                }

                double coinsSell;
                try
                {
                     coinsSell = htmlDoc.DocumentNode.SelectNodes("//table[@class='tbl-switcher']/tbody/tr/td[7]")
                        .First().InnerHtml.Replace("\r\n\t", "").Replace("\t", "").ParseToDoubleFormat();
                }
                catch (Exception e)
                {
                    coinsSell = 0;
                }
                DateTime parseDate = DateTime.Now;
                Console.WriteLine("Parse coins rates Tkbbank OK. Time: {0}.", DateTime.Now.ToString("HH:mm:ss"));
                return new List<CoinsRate>
                {
                    new CoinsRate()
                    {
                        Acronim = "GPM",
                        Sell = coinsSell,
                        Buy = coinsBuy,
                        Date = parseDate,
                        Site = UrlParseHelper.Tkbbank
                    },
                    new CoinsRate()
                    {
                        Acronim = "GPS",
                        Sell = coinsSell,
                        Buy = coinsBuy,
                        Date = parseDate,
                        Site = UrlParseHelper.Tkbbank
                    }
                };



            }
            catch (Exception e)
            {
                Console.WriteLine("Error parse coins rates Tkbbank. Time: {0}. Error: {1}.",
                    DateTime.Now.ToString("HH:mm:ss"), e.Message);
            }

            return null;
        }

    }
}


