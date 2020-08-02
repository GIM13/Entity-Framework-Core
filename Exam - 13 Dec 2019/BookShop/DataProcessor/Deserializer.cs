namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ProductShop.XMLHelper;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var rootAttributeName = "Books";

            var booksDTO = XMLConverter.Deserializer<ImportBookDTO>(xmlString, rootAttributeName);

            var books = new List<Book>();

            var result = string.Empty;

            foreach (var bookDTO in booksDTO)
            {
                if (!IsValid(bookDTO))
                {
                    result += ErrorMessage;
                    continue;
                }

                books.Add(new Book
                {
                    Name = bookDTO.Name,
                    Genre = (Genre)bookDTO.Genre,
                    Price = bookDTO.Price,
                    Pages = bookDTO.Pages,
                    PublishedOn = DateTime.ParseExact(bookDTO.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture)
                });

                result += $"Successfully imported book {bookDTO.Name} for {bookDTO.Price:F2}.";
            };

            context.Books.AddRange(books);

            context.SaveChanges();

            return result;
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            throw new NotImplementedException();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}