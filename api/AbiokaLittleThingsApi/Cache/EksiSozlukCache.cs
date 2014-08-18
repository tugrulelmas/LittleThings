using AbiokaLittleThingsApi.Entity;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace AbiokaLittleThingsApi.Cache
{
    public class EksiSozlukCache
    {
        private const string url = "https://eksisozluk.com/istatistik/dunun-en-begenilen-entryleri";
        private static readonly object obj = new object();

        private static IEnumerable<Entry> entries;
        private static bool isNew;

        public static IEnumerable<Entry> Entries {
            get {
                try {
                    lock (obj) {
                        if (entries == null) {
                            entries = DununEnBegenilenleri();
                        }
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                }
                return entries;
            }
        }

        public static Entry GetEntry(int sorting) {
            var entry = Entries.Where(e => e.Sorting == sorting).FirstOrDefault();
            if (entry == null) throw new ArgumentNullException(string.Format("{0} nolu entry bulunamadı", sorting));

            lock (obj) {
                if (string.IsNullOrWhiteSpace(entry.Text)) {
                    SetEntryText(entry);
                }
            }
            return entry;
        }

        private static List<Entry> DununEnBegenilenleri() {
            var responseFromServer = GetResponseData(url);
            return ParseHtml(responseFromServer);
        }

        private static string GetResponseData(string url) {
            WebRequest request = WebRequest.Create(url);
            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            return responseFromServer;
        }

        private static void SetEntryText(Entry entry) {
            var html = GetResponseData(entry.Url);
            ParseEntryHtml(html, entry);
        }

        private static void ParseEntryHtml(string html, Entry entry) {
            var documentNode = GetHtmlNode(html);
            var node = documentNode.SelectSingleNode("//ol[@id='entry-list']//div[@class='content']");
            var entryDate = documentNode.SelectSingleNode("//ol[@id='entry-list']//span[@class='entry-date']");

            entry.Text = node.InnerHtml;
            entry.EntryDate = entryDate.InnerText;
        }

        private static HtmlNode GetHtmlNode(string html) {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            if (document.DocumentNode == null) throw new Exception("Ekşi Sözlük'e erişimde bir takım sıkıntılar mevcut.");

            return document.DocumentNode;
        }

        private static List<Entry> ParseHtml(string html) {
            var documentNode = GetHtmlNode(html);
            var nodes = documentNode.SelectNodes("//ol[@id='stats']//li//a");

            var entries = new List<Entry>();
            var sorting = 0;
            foreach (var nodeItem in nodes) {
                sorting++;
                var href = nodeItem.Attributes.Where(a => a.Name == "href").First().Value;
                var url = string.Format("https://eksisozluk.com{0}", href);
                var title = nodeItem.SelectSingleNode("//span[@class='caption']").InnerText;
                var autor = nodeItem.SelectSingleNode("//div[@class='detail']").InnerText;

                entries.Add(new Entry() {
                    Sorting = sorting,
                    Url = url,
                    Title = title,
                    Autor = autor
                });
            }
            return entries;
        }
    }
}