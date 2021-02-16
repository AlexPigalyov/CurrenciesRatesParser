using CurrenciesRatesParser.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            parse_9999d_ru();

        }


        static void parse_9999d_ru()
        {

            var html = @"https://9999d.ru/";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            var selector = "//div[@class='catalog item-views table catalog_table_2' and @data-slice='Y']";

            var node = htmlDoc.DocumentNode.SelectSingleNode(selector);


            var htmlDoc2 = new HtmlDocument();
            htmlDoc2.LoadHtml("<div>"+node.InnerHtml+"</div>");

            var nodesXPath = "//div[@id='bx_651765591_51184']/div[@class='inner-wrap']/div[@class='text']";

            bool fExists = File.Exists("some.html");

            File.WriteAllText("some.html", htmlDoc2.DocumentNode.InnerHtml);

            var nodes = htmlDoc2.DocumentNode.SelectSingleNode(nodesXPath);


            var a = 3;


            

            //var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");

            //Console.WriteLine("Node Name: " + node.Name + "\n" + node.OuterHtml);

        }


        static CoinsRates parseCoin(string html)
        {
            CoinsRates coin = new CoinsRates();





            return coin;
        }





    }
}
