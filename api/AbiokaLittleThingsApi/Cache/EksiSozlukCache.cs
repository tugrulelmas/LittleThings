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
        private const string errorMessage = "Ekşi Sözlük'e erişimde bir takım sıkıntılar mevcut.";
        private static readonly object obj = new object();

        private static IEnumerable<Entry> entries;
        private static DateTime lastLoadedDate;

        public static IEnumerable<Entry> Entries {
            get {
                try {
                    lock (obj) {
                        if (CanLoaded()) {
                            entries = DununEnBegenilenleri();
                            lastLoadedDate = DateTime.Now;
                        }
                    }
                } catch (Exception) {
                    throw;
                }
                return entries;
            }
        }

        private static bool CanLoaded() {
            if (entries == null || lastLoadedDate == null) return true;

            var now = DateTime.Now.TimeOfDay;
            //saat 6 ile 9 arasinda ise yarim saatte bir guncelle.
            if (now.CompareTo(new TimeSpan(6, 0, 0)) > 1 && now.CompareTo(new TimeSpan(9, 0, 0)) < 0
                && lastLoadedDate.AddMinutes(30).CompareTo(DateTime.Now) < 0) {
                return true;
            }

            return false;
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
            /*
            var proxy = new WebProxy("http://10.0.7.224:8080");
            request.Proxy = proxy;
            */
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
            var entryNumber = documentNode.SelectSingleNode("//ol[@id='entry-list']//li").Attributes.Where(a => a.Name == "value").First().Value;

            if (node == null || entryDate == null) throw new Exception(errorMessage);

            entry.Text = node.InnerHtml.Replace("href=\"/", "target='_blank' href=\"https://eksisozluk.com/");
            entry.EntryDate = entryDate.InnerText;
            entry.EntryNumber = entryNumber;
        }

        private static HtmlNode GetHtmlNode(string html) {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            if (document.DocumentNode == null) throw new Exception(errorMessage);

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
                var title = nodeItem.SelectSingleNode("span[@class='caption']").InnerText;
                var autor = nodeItem.SelectSingleNode("div[@class='detail']").InnerText;

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