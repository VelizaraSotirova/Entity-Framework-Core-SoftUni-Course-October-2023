﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            // Query 9. Import Suppliers
            //string inputSuppliersXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            //Console.WriteLine(ImportSuppliers(context, inputSuppliersXml));

            // Query 10. Import Parts
            //string inputPartsXml = File.ReadAllText("../../../Datasets/parts.xml");
            //Console.WriteLine(ImportParts(context, inputPartsXml));

            // Query 11. Import Cars
            //string inputCarsXml = File.ReadAllText("../../../Datasets/cars.xml");
            //Console.WriteLine(ImportCars(context, inputCarsXml));

            // Query 12. Import Customers
            //string inputCustomersXml = File.ReadAllText("../../../Datasets/customers.xml");
            //Console.WriteLine(ImportCustomers(context, inputCustomersXml));

            // Query 13. Import Sales
            //string inputSalesXml = File.ReadAllText("../../../Datasets/sales.xml");
            //Console.WriteLine(ImportSales(context, inputSalesXml));

            // Query 14. Export Cars With Distance
            //Console.WriteLine(GetCarsWithDistance(context));

            // Query 15. Export Cars from Make BMW
            //Console.WriteLine(GetCarsFromMakeBmw(context));

            // Query 16. Export Local Suppliers
            //Console.WriteLine(GetLocalSuppliers(context));

            //Query 17. Export Cars with Their List of Parts
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));

            // Query 18. Export Total Sales by Customer
            //Console.WriteLine(GetTotalSalesByCustomer(context));

            // Query 19. Export Sales with Applied Discount
            //Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        private static Mapper GetMapper ()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<CarDealerProfile>());
            
            return new Mapper (cfg);
        }

        // Query 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            // 1.Create an xml serializer
            XmlSerializer serializer = new XmlSerializer(typeof(ImportSupplierDTO[]), 
                new XmlRootAttribute("Suppliers"));

            // 2.Deserialize
            using var reader = new StringReader(inputXml);

            ImportSupplierDTO[] importSupplierDTOs = (ImportSupplierDTO[])serializer
                .Deserialize(reader);

            // 3.Map
            var mapper = GetMapper();
            Supplier[] suppliers = mapper.Map<Supplier[]>(importSupplierDTOs);

            // 4.Add to EF context
            context.AddRange(suppliers);

            // 5.Commit changes to the database
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }

        // Query 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = 
                new XmlSerializer(typeof(ImportPartsDTO[]), 
                new XmlRootAttribute("Parts"));

            using var reader = new StringReader(inputXml);
            ImportPartsDTO[] importPartDTOs = (ImportPartsDTO[])serializer.Deserialize(reader);

            var supplierIds = context.Suppliers
                .Select(x => x.Id)
                .ToArray();

            var mapper = GetMapper();
            Part[] parts = mapper.Map<Part[]>(importPartDTOs
                .Where(p => supplierIds.Contains(p.SupplierId)));

            context.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}";
        }

        // Query 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportCarsDTO[]), new XmlRootAttribute("Cars"));

            using StringReader stringReader = new StringReader(inputXml);

            ImportCarsDTO[] importCarDTOs = (ImportCarsDTO[])xmlSerializer.Deserialize(stringReader);

            var mapper = GetMapper();
            List<Car> cars = new List<Car>();

            foreach (var carDTO in importCarDTOs)
            {
                Car car = mapper.Map<Car>(carDTO);

                int[] carPartIds = carDTO.PartsIds
                    .Select(x => x.Id)
                    .Distinct()
                    .ToArray();

                var carParts = new List<PartCar>();

                foreach (var id in carPartIds)
                {
                    carParts.Add(new PartCar
                    {
                        Car = car,
                        PartId = id
                    });
                }

                car.PartsCars = carParts;
                cars.Add(car);
            }

            context.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        // Query 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = 
                new XmlSerializer(typeof(ImportCustomersDTO[]), new XmlRootAttribute("Customers"));

            var reader = new StringReader(inputXml);

            ImportCustomersDTO[] importCustomersDTOs = (ImportCustomersDTO[])serializer.Deserialize(reader);
        
            var mapper = GetMapper(); 
            Customer[] customers = mapper.Map<Customer[]>(importCustomersDTOs);

            context.AddRange (customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }

        // Query 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportSaleDTO[]), new XmlRootAttribute("Sales"));

            var reader = new StringReader(inputXml);
            ImportSaleDTO[] importSaleDTOs = (ImportSaleDTO[])serializer.Deserialize(reader);

            var mapper = GetMapper();

            int[] carIds = context.Cars
                .Select(x => x.Id).ToArray();

            Sale[] sales = mapper.Map<Sale[]>(importSaleDTOs)
                .Where(s => carIds.Contains(s.CarId)).ToArray();

            context.AddRange (sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";
        }

        // Query 14. Export Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var mapper = GetMapper();

            ExportCarsWithDistance[] carsWithDistance = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<ExportCarsWithDistance>(mapper.ConfigurationProvider)
                .ToArray();

            // Wrap in the CarCollection class
            var carCollection = new CarCollection
            {
                Cars = carsWithDistance
            };

            return SerializeToXml(carsWithDistance, "cars");
        }

        // Query 15. Export Cars from Make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var bmws = context.Cars
                .Where(c => c.Make == "BMW")
                .Select(c => new ExportBmwCars()
                {
                    Id = c.Id,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ToArray();

            return SerializeToXml(bmws, "cars", true);
        }

        // Query 16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSuppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new LocalSupplierExportDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                }).ToArray();

            return SerializeToXml(localSuppliers, "suppliers");
        }

        // Query 17. Export Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context.Cars
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .Select(c => new CarWithParts()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                    Parts = c.PartsCars
                        .OrderByDescending(p => p.Part.Price)
                        .Select(pc => new PartsForCarsDto()
                        {
                            Name = pc.Part.Name,
                            Price = pc.Part.Price,
                        }).ToArray()
                }).ToArray();

            return SerializeToXml(carsWithParts, "cars");
        }

        // Query 18. Export Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var temp = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SalesInfo = c.Sales.Select(s => new
                    {
                        Prices = c.IsYoungDriver
                        ? s.Car.PartsCars.Sum(pc => Math.Round((double)pc.Part.Price * 0.95, 2))
                        : s.Car.PartsCars.Sum(pc => (double)pc.Part.Price)
                    }).ToArray()
                }).ToArray();

            var customerSalesInfo = temp
                .OrderByDescending(x =>
                    x.SalesInfo.Sum(y => y.Prices))
                .Select(a => new CustomerExportDto()
                {
                    FullName = a.FullName,
                    CarsBought = a.BoughtCars,
                    MoneySpent = a.SalesInfo.Sum(b => (decimal)b.Prices)
                })
                .ToArray();

            return SerializeToXml(customerSalesInfo, "customers");
        }

        // Query 19. Export Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new SaleWithDiscount()
                {
                    Car = new CarDto()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    Discount = (int)s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartsCars
                        .Sum(pc => pc.Part.Price),
                    PriceWithDiscount = Math.Round(
                        (double)(s.Car.PartsCars.Sum(p => p.Part.Price)
                                 * (1 - (s.Discount / 100))), 4)
                }).ToArray();

            return SerializeToXml(sales, "sales");
        }


        //SerializeToXml
        private static string SerializeToXml<T>(T dto, string xmlRootAttribute, bool omitDeclaration = false)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootAttribute));
            StringBuilder stringBuilder = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = omitDeclaration,
                Encoding = new UTF8Encoding(false),
                Indent = true
            };

            using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
            {
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);

                try
                {
                    xmlSerializer.Serialize(xmlWriter, dto, xmlSerializerNamespaces);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return stringBuilder.ToString();
        }
    }
}