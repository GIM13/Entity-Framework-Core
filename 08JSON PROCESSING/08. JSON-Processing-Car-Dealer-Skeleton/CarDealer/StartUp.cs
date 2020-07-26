using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{

    public class StartUp
    {
        public static void Main()
        {
            var carDealerContext = new CarDealerContext();

            //carDealerContext.Database.EnsureDeleted();
            //carDealerContext.Database.EnsureCreated();

            //string inputJson1 = File.ReadAllText(@"Datasets/suppliers.json");
            //string inputJson2 = File.ReadAllText(@"Datasets/parts.json");
            //string inputJson3 = File.ReadAllText(@"Datasets/cars.json");
            //string inputJson4 = File.ReadAllText(@"Datasets/customers.json");
            //string inputJson5 = File.ReadAllText(@"Datasets/sales.json");

            //Console.WriteLine(ImportSuppliers(carDealerContext, inputJson1));
            //Console.WriteLine(ImportParts(carDealerContext, inputJson2));
            //Console.WriteLine(ImportCars(carDealerContext, inputJson3));
            //Console.WriteLine(ImportCustomers(carDealerContext, inputJson4));
            //Console.WriteLine(ImportSales(carDealerContext, inputJson5));

            Mapper.Initialize(cfg => { });

            Console.WriteLine(GetSalesWithAppliedDiscount(carDealerContext));
        }

        //09. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);

            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var supplierIds = context.Suppliers
                .Select(x => x.Id);

            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson)
                .Where(x => supplierIds.Contains(x.SupplierId));

            context.Parts.AddRange(parts);

            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<Car[]>(inputJson);

            context.Cars.AddRange(cars);

            context.SaveChanges();

            return $"Successfully imported { cars.Length}.";
        }

        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.Customers.AddRange(customers);

            context.SaveChanges();

            return $"Successfully imported { customers.Count()}.";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(sales);

            context.SaveChanges();

            return $"Successfully imported { sales.Count()}.";
        }

        //14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .ProjectTo<CustomerDTO>()
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver);

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy",
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(customers, settings);
        }

        //15. Export Cars From Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var categories = context.Cars
                .Where(x => x.Make == "Toyota")
                .ProjectTo<CarDTO>()
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance);

            return JsonConvert.SerializeObject(categories, Formatting.Indented);
        }

        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .ProjectTo<SupplierDTO>();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        //17. Export Cars With Their List Of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsParts = context.Cars
                .Select(cp => new
                {
                    car = new
                    {
                        cp.Make,
                        cp.Model,
                        cp.TravelledDistance
                    },
                    parts = cp.PartCars
                    .Select(p => new
                    {
                        p.Part.Name,
                        Price = p.Part.Price.ToString("f2")
                    })
                });

            return JsonConvert.SerializeObject(carsParts, Formatting.Indented);
        }

        //18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customer = context.Customers
                .Where(x => x.Sales.Any())
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales
                    .Select(s => s.Car
                                  .PartCars
                                  .Sum(pc => pc.Part.Price))
                    .Sum()
                })
                .OrderByDescending(x => x.spentMoney)
                .ThenBy(x => x.boughtCars);

            return JsonConvert.SerializeObject(customer, Formatting.Indented);
        }

        //19. Export Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TravelledDistance
                    },
                    customerName = s.Customer.Name,
                    Discount = s.Discount.ToString("f2"),
                    price = s.Car
                             .PartCars
                             .Sum(pc => pc.Part.Price)
                             .ToString("f2"),
                    priceWithDiscount = (s.Car
                                          .PartCars
                                          .Sum(pc => pc.Part.Price) * ((100 - s.Discount) / 100))
                                          .ToString("f2")
                });

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }
    }
}