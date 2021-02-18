using AngleSharp;
using CurrenciesRatesParser.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CurrenciesRatesParser.Necromant24
{
    public static class Parser
    {

        //@Necromant24 источники:
        //9999d.ru
        //ricgold.com
        //rshb.ru

        public static void Parse()
        {
            Get_9999d_ru_coins();
        }

        public static async Task<List<CoinsRates>> Get_9999d_ru_coins()
        {
            List<CoinsRates> coins = new List<CoinsRates>();

            var nodesXPathGPM = "//div[@id='bx_651765591_51184']/div[@class='inner-wrap']/div[@class='text']";

            var nodesXPathGPS = "//div[@id='bx_651765591_51290']/div[@class='inner-wrap']/div[@class='text']";


            coins.Add(parseCoin(nodesXPathGPM).Result);
            coins.Add(parseCoin(nodesXPathGPS).Result);

            return coins;
        }



        static async Task<CoinsRates> parseCoin(string xpath)
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
                Acronim = "GPM"
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
