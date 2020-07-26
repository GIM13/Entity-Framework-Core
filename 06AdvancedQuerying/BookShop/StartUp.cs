namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            string command = Console.ReadLine();

            Console.WriteLine(RemoveBooks(db));
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
                .Select(x => x.Book.Title)
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

        //7. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(x => x.FirstName + " " + x.LastName)
                .OrderBy(x => x)
                .ToList();

            return string.Join(Environment.NewLine, authors);
        }

        //8. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var titles = context.Books
                            .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                            .OrderBy(x => x.Title)
                            .Select(x => x.Title)
                            .ToList();

            return string.Join(Environment.NewLine, titles);
        }

        //9. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var titles = context.Books
                         .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                         .OrderBy(x => x.BookId)
                         .Select(x => x.Title + " (" + x.Author.FirstName + " " + x.Author.LastName + ")")
                         .ToList();

            return string.Join(Environment.NewLine, titles);
        }

        //10. Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var titlesCount = context.Books
                         .Where(b => b.Title.Length > lengthCheck)
                         .ToList();

            return titlesCount.Count;
        }

        //11. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var copies = context.Authors
                         .OrderByDescending(a => a.Books.Sum(b => b.Copies))
                         .Select(x => x.FirstName + " " + x.LastName + " - " + x.Books.Sum(b => b.Copies))
                         .ToList();

            return string.Join(Environment.NewLine, copies);
        }

        //12. Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var profit = context.Categories
                         .Select(c => new
                         {
                             Profit = c.CategoryBooks.Sum(bc => bc.Book.Copies * bc.Book.Price),
                             c.Name
                         })
                         .OrderByDescending(x => x.Profit)
                         .ThenBy(x => x.Name)
                         .Select(x => $"{x.Name} ${x.Profit:f2}")
                         .ToList();

            return string.Join(Environment.NewLine, profit);
        }

        //13. Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var result = new StringBuilder();

            var categories = context.Categories
                            .Select(c => new
                            {
                                c.Name,
                                Books = c.CategoryBooks.Select(x => x.Book).OrderByDescending(x => x.ReleaseDate).Take(3)
                            })
                            .OrderBy(x => x.Name);

            foreach (var categorie in categories)
            {
                result.AppendLine($"--{categorie.Name}");

                foreach (var book in categorie.Books)
                {
                    result.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return result.ToString().Trim();
        }

        //14. Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                               .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //15. Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books.Where(b => b.Copies < 4200);

            var result = books.Count();

            context.Books.RemoveRange(books);

            context.SaveChanges();

            return result;
        }
    }
}
