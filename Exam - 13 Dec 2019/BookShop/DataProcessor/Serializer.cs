namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using ProductShop.XMLHelper;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors
                                 .Select(x => new
                                 {
                                     AuthorName = x.FirstName + " " + x.LastName,
                                     Books = x.AuthorsBooks
                                              .Select(ab => ab.Book)
                                              .OrderByDescending(y => y.Price)
                                              .Select(b => new
                                              {
                                                  BookName = b.Name,
                                                  BookPrice = b.Price.ToString("f2")
                                              })
                                              .ToArray()

                                 })
                                 .ToArray()
                                 .OrderByDescending(x => x.Books.Count())
                                 .ThenBy(x => x.AuthorName)
                                 .ToArray();

            return JsonConvert.SerializeObject(authors, Formatting.Indented);
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var booksDTO = context.Books
                               .Where(x => x.Genre == Genre.Science && x.PublishedOn < date)
                               .Select(x => new ExportBooksDTO
                               {
                                   Name = x.Name,
                                   Date = x.PublishedOn.ToString("d",CultureInfo.InvariantCulture),
                                   Pages = x.Pages
                               })
                               .ToArray()
                               .OrderByDescending(x => x.Pages)
                               .ThenBy(x => x.Date)
                               .Take(10)
                               .ToArray();

            var rootAttributeName = "Books";

            return XMLConverter.Serialize(booksDTO, rootAttributeName);
        }
    }
}