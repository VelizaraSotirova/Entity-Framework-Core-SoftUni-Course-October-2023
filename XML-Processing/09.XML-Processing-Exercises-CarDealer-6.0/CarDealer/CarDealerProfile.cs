using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<ImportSupplierDTO, Supplier>();
            CreateMap<ImportPartsDTO, Part>();
            CreateMap<ImportCarsDTO, Car>();
            CreateMap<Car, ExportCarsWithDistance>();
            CreateMap<ImportCustomersDTO, Customer>();
            CreateMap<ImportSaleDTO, Sale>();

            //CreateMap<Car, ExportBmwCars>();
        }
    }
}
