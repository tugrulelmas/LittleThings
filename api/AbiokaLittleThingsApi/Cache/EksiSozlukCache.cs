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
        private const string url = "https://eksisozluk.com/debe";
        private const string errorMessage = "Ekşi Sözlük'e erişimde bir takım sıkıntılar mevcut.";
        private static readonly object obj = new object();

        private static IEnumerable<Entry> entries;
        private static DateTime lastLoadedDate;

        public static IEnumerable<Entry> Entries {
            get {
                try {
                    lock (obj) {
                        if (CanLoaded()) {
                            lastLoadedDate = DateTime.Now;
                            entries = DununEnBegenilenleri();
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

            //yarim saatte bir guncelle.
            if (lastLoadedDate.AddMinutes(30).CompareTo(DateTime.Now) < 0) {
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
#if DEBUG
            var proxy = new WebProxy("http://10.0.7.224:8080");
            request.Proxy = proxy;
#endif
            var responseFromServer = string.Empty;
            using (var response = request.GetResponse())
            using (var dataStream = response.GetResponseStream())
            using (var reader = new StreamReader(dataStream)) {
                responseFromServer = reader.ReadToEnd();
            }
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
            var entryNumberNode = documentNode.SelectSingleNode("//ol[@id='entry-list']//li").Attributes.Where(a => a.Name == "value").FirstOrDefault();

            if (node == null || entryDate == null || entryNumberNode == null) throw new Exception(errorMessage);

            entry.Text = node.InnerHtml.Replace("href=\"/", "target='_blank' href=\"https://eksisozluk.com/");
            entry.EntryDate = entryDate.InnerText;
            entry.EntryNumber = entryNumberNode.Value;
        }

        private static HtmlNode GetHtmlNode(string html) {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            if (document.DocumentNode == null) throw new Exception(errorMessage);

            return document.DocumentNode;
        }

        private static List<Entry> ParseHtml(string html) {
            var documentNode = GetHtmlNode(html);
            var nodes = documentNode.SelectNodes("//ol[@class='stats topic-list']//li//a");
            if (nodes == null) 
                throw new Exception(errorMessage);

            var entries = new List<Entry>();
            var sorting = 0;
            foreach (var nodeItem in nodes) {
                var hrefNode = nodeItem.Attributes.Where(a => a.Name == "href").First();
                var titleNode = nodeItem.SelectSingleNode("span[@class='caption']");
                var authorNode = nodeItem.SelectSingleNode("div[@class='detail']");

                if (hrefNode == null || titleNode == null || authorNode == null) continue;
                
                sorting++;
                var url = string.Format("https://eksisozluk.com{0}", hrefNode.Value);

                entries.Add(new Entry() {
                    Sorting = sorting,
                    Url = url,
                    Title = titleNode.InnerText,
                    Author = authorNode.InnerText,
                    LoadedDate = lastLoadedDate
                });
            }
            return entries;
        }
    }
}