using AutoMapper.QueryableExtensions;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using ProductShop.XMLHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var inputXml = File.ReadAllText(@"Datasets/categories-products.xml");

            Console.WriteLine(GetUsersWithProducts(context));
        }

        //01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var rootAttributeName = "Users";

            var usersDTO = XMLConverter.Deserializer<ImportUserDTO>(inputXml, rootAttributeName);

            var users = usersDTO.Select(u => new Models.User
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Age = u.Age
            });

            context.Users.AddRange(users);

            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        //02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var rootAttributeName = "Products";

            var productsDTO = XMLConverter.Deserializer<ImportProductDTO>(inputXml, rootAttributeName);

            var products = productsDTO.Select(p => new Product
            {
                Name = p.Name,
                Price = p.Price,
                SellerId = p.SellerId,
                BuyerId = p.BuyerId
            });

            context.Products.AddRange(products);

            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        //03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var rootAttributeName = "Categories";

            var categoriesDTO = XMLConverter.Deserializer<ImportCategoryDTO>(inputXml, rootAttributeName);

            var categories = categoriesDTO.Select(c => new Category
            {
                Name = c.Name
            });

            context.Categories.AddRange(categories);

            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        //04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var rootAttributeName = "CategoryProducts";

            var catProdDTO = XMLConverter.Deserializer<ImportCategoryProductsDTO>(inputXml, rootAttributeName);

            var categoriesProducts = catProdDTO.Select(x => new CategoryProduct
            {
                CategoryId = x.CategoryId,
                ProductId = x.ProductId
            });

            context.CategoryProducts.AddRange(categoriesProducts);

            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count()}";
        }

        //05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsDTO = context.Products
                .Where(x => 500 <= x.Price && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Take(10)
                .Select(x => new ExportProductsDTO
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = $"{x.Buyer.FirstName} {x.Buyer.LastName}"
                })
                .ToArray();

            var rootAttributeName = "Products";

            return XMLConverter.Serialize(productsDTO, rootAttributeName);
        }

        //06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersDTO = context.Users
                .Where(x => x.ProductsSold.Any())
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .Select(x => new ExportUsersDTO
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(y => new ExportProductsDTO
                    {
                        Name = y.Name,
                        Price = y.Price
                    })
                    .ToArray()
                })
                .ToArray();

            var rootAttributeName = "Users";

            return XMLConverter.Serialize(usersDTO, rootAttributeName);
        }

        //07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categoriesDTO = context.Categories
                .Select(x => new ExportCatecoryDTO
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count(),
                    AveragePrice = x.CategoryProducts.Average(y => y.Product.Price),
                    TotalRevenue = x.CategoryProducts.Sum(y => y.Product.Price)
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToArray();

            var rootAttributeName = "Categories";

            return XMLConverter.Serialize(categoriesDTO, rootAttributeName);
        }

        //08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersDTO = context.Users
                // for judge with toarray
                //.ToArray() 
                .Where(x => x.ProductsSold.Any())
                .Select(x => new Dtos.Export.User
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProducts = new ExportSoldProductsDTO
                    {
                        Count = x.ProductsSold.Count,
                        Products = x.ProductsSold
                                    .Select(y => new ExportProductsDTO
                                    {
                                        Name = y.Name,
                                        Price = y.Price
                                    })
                                    .OrderByDescending(p => p.Price)
                                    .ToArray()
                    }
                })
               .ToArray();

            var result = new Users
            {
                Count = usersDTO.Length,
                Userss = usersDTO
                .OrderByDescending(x => x.SoldProducts.Count)
                .Take(10)
                .ToArray()
            };

            var rootAttributeName = "Users";

            return XMLConverter.Serialize(result, rootAttributeName);
        }
    }
}