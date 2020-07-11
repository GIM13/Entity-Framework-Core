namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            // DbInitializer.ResetDatabase(db);

            string command = Console.ReadLine();

            Console.WriteLine(GetBooksReleasedBefore(db, command));
        }

        //1. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var result = string.Empty;

            var check = new AgeRestriction();

            switch (command.ToLower())
            {
                case "minor": check = (AgeRestriction)0; break;

                case "teen": check = (AgeRestriction)1; break;

                case "adult": check = (AgeRestriction)2; break;
            }

            var titles = context.Books
                .Where(x => x.AgeRestriction.Equals(check))
                .OrderBy(x => x.Title)
                .Select(x => new
                {
                    x.Title
                })
                .ToList();

            foreach (var title in titles)
            {
                result += $"{title.Title}" + Environment.NewLine;
            }

            return result.TrimEnd();
        }

        //2. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            var result = string.Empty;

            var titles = context.Books
                .Where(x => x.EditionType.Equals(EditionType.Gold)
                         && x.Copies < 5000)
                .OrderBy(x => x.BookId)
                .Select(x => new
                {
                    x.Title
                })
                .ToList();

            foreach (var title in titles)
            {
                result += $"{title.Title}" + Environment.NewLine;
            }

            return result.TrimEnd();
        }

        //3. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var result = new StringBuilder();

            var titles = context.Books
                .Where(x => x.Price > 40)
                .OrderByDescending(x => x.Price)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                })
                .ToList();

            foreach (var title in titles)
            {
                result.AppendLine($"{title.Title} - ${title.Price:f2}");
            }

            return result.ToString().Trim();
        }

        //4. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var titles = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToList();

            return string.Join(Environment.NewLine, titles);
        }

        //5. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categoriesNemes = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToLower())
                .ToArray();

            var titles = context.BooksCategories
                .Where(bc => categoriesNemes.Contains(bc.Category.Name.ToLower()))
                .OrderBy(x => x.Book.Title)
                .Select(x =>  x.Book.Title)
                .ToList();

            return string.Join(Environment.NewLine, titles);
        }

        //6. Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var result = new StringBuilder();

            var dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var titles = context.Books
                .Where(b => b.ReleaseDate.Value.Date < dateTime)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new
                {
                    x.Title,
                    x.EditionType,
                    x.Price
                })
                .ToList();

            foreach (var book in titles)
            {
                result.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }

            return result.ToString().Trim();
        }
    }
}
