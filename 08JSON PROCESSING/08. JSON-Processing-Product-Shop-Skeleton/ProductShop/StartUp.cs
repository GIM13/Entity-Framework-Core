using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;
namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {

            var productShopContext = new ProductShopContext();

            //string inputJson = File.ReadAllText(@"Datasets/categories-products.json");

            Console.WriteLine(GetUsersWithProducts(productShopContext));
        }

        //01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);

            context.Users.AddRange(users);

            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        //02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context.Products.AddRange(products);

            context.SaveChanges();

            return $"Successfully imported { products.Count()}";
        }

        //03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
                                        .Where(x => x.Name != null)
                                        .ToArray();

            context.Categories.AddRange(categories);

            context.SaveChanges();

            return $"Successfully imported { categories.Count()}";
        }

        //04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            context.CategoryProducts.AddRange(categoryProducts);

            context.SaveChanges();

            return $"Successfully imported { categoryProducts.Count()}";
        }

        //05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = x.Seller.FirstName + " " + x.Seller.LastName
                }).ToArray();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }

        //06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(y => y.Buyer != null))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold
                                    .Select(z => new
                                    {
                                        name = z.Name,
                                        price = z.Price,
                                        buyerFirstName = z.Buyer.FirstName,
                                        buyerLastName = z.Buyer.LastName
                                    }).ToArray()
                }).ToArray();

            return JsonConvert.SerializeObject(users, Formatting.Indented);
        }

        //07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(x => x.CategoryProducts.Count())
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count(),
                    averagePrice = x.CategoryProducts.Average(y => y.Product.Price).ToString("f2"),
                    totalRevenue = x.CategoryProducts.Sum(y => y.Product.Price).ToString("f2")
                }).ToArray();

            return JsonConvert.SerializeObject(categories, Formatting.Indented);
        }

        //08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any())
                .OrderByDescending(x => x.ProductsSold.Count())
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    age = x.Age,
                    soldProducts = new
                    {
                        count = x.ProductsSold.Count(),
                        products = x.ProductsSold
                        .Select(y => new
                        {
                            name = y.Name,
                            price = y.Price
                        })
                    }
                });

            var usersCount = new
            {
                usersCount = users.Count(),
                users
            };

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(usersCount, settings);
        }
    }
}