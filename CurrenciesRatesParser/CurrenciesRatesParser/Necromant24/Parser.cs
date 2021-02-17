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
            //parse_9999d_ru_GPS();
            parse_9999d_ru_GPM();

        }


        static List<CoinsRates> parse_9999d_ru_GPM()
        {

            var site = @"https://9999d.ru/";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(site);

            var selector = "//div[@class='catalog item-views table catalog_table_2' and @data-slice='Y']";

            var node = htmlDoc.DocumentNode.SelectSingleNode(selector);


            var htmlDoc2 = new HtmlDocument();
            htmlDoc2.LoadHtml("<div>"+node.InnerHtml+"</div>");

            var nodesXPathGPM = "//div[@id='bx_651765591_51184']/div[@class='inner-wrap']/div[@class='text']";

            var nodesXPathGPS = "//div[@id='bx_651765591_51290']/div[@class='inner-wrap']/div[@class='text']";

            bool fExists = File.Exists("some.html");

            

            var nodeGPM = htmlDoc2.DocumentNode.SelectSingleNode(nodesXPathGPM);

            var nodeGPS = htmlDoc2.DocumentNode.SelectSingleNode(nodesXPathGPS);

            File.WriteAllText("some.html", nodeGPM.InnerText);

            var innerTextGPM = Regex.Replace(nodeGPM.InnerText, @"\s+", " ");

            var innerTextGPS = Regex.Replace(nodeGPS.InnerText, @"\s+", " ");

            var pricesGPM = GetBetweenTwoWords("ПРОДАЖА", "Цена за грамм", innerTextGPM);

            var pricesGPS = GetBetweenTwoWords("ПРОДАЖА", "Цена за грамм", innerTextGPS);





            var pricePairGPM = pricesGPM.Split(new string[1] { "ПОКУПКА" },StringSplitOptions.RemoveEmptyEntries);

            var pricePairGPS = pricesGPS.Split(new string[1] { "ПОКУПКА" }, StringSplitOptions.RemoveEmptyEntries);

            pricePairGPM = pricePairGPM.Select(x => x.Replace("₽", "")).ToArray();

            pricePairGPS = pricePairGPS.Select(x => x.Replace("₽", "")).ToArray();

            List<CoinsRates> coins = new List<CoinsRates>();

            CoinsRates coinGPM = new CoinsRates()
            {
                Date = DateTime.Now,
                Site = site,
                Acronim = "GPM"
            };

            coinGPM.Sell = double.Parse(pricePairGPM[0]);
            coinGPM.Buy = double.Parse(pricePairGPM[1]);


            CoinsRates coinGPS = new CoinsRates()
            {
                Date = DateTime.Now,
                Site = site,
                Acronim = "GPS"
            };

            coinGPS.Sell = double.Parse(pricePairGPS[0]);
            coinGPS.Buy = double.Parse(pricePairGPS[1]);






            coins.Add(coinGPM);
            coins.Add(coinGPS);

            var a = 3;




            //var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");

            //Console.WriteLine("Node Name: " + node.Name + "\n" + node.OuterHtml);
            return coins;
        }


        //static async void parse_9999d_ru_GPS()
        //{
        //    var url = @"https://9999d.ru/product/element/georgiy_pobedonosets_zoloto_2018/";

        //    var config = Configuration.Default.WithDefaultLoader();
        //    var context = BrowsingContext.New(config);
        //    var document = await context.OpenAsync(url);
        //    //var mySelector = " div > div.item > div > div > div:nth-child(2) > div > div.row > div > div.bottom-wrapper > div".Replace(" ","");
        //    var mySelector = "div";


        //    var cells = document.QuerySelector(mySelector);
        //    //var titles = cells.Select(m => m.TextContent);



        //    var s = 7;

        //}




        static async void parse_9999d_ru_GPS()
        {
            var url = @"https://9999d.ru/product/element/georgiy_pobedonosets_zoloto_2018/";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(url);

            var xpath = "//span[@class=\"price_val\"]";

            var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);

            // подозрение на динамический рендер

            var s = 7;

        }




        public static string GetBetweenTwoWords(string firstWord, string secondWord, string str)
        {
            var firstWordIndex = str.IndexOf(firstWord) + firstWord.Length;
            var secondWordIndex = str.IndexOf(secondWord);
            return str.Substring(firstWordIndex, secondWordIndex - firstWordIndex);
        }


        static CoinsRates parseCoin(string html)
        {
            CoinsRates coin = new CoinsRates();





            return coin;
        }





    }
}
