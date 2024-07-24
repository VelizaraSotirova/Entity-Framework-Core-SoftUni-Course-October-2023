namespace FastFood.Core.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Core.ViewModels.Categories;
    using FastFood.Core.ViewModels.Employees;
    using FastFood.Core.ViewModels.Items;
    using FastFood.Core.ViewModels.Orders;
    using FastFood.Models;
    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Categories
            CreateMap<CreateCategoryInputModel, Category>()
                .ForMember(c => c.Name, i => i.MapFrom(x => x.CategoryName));
           
            CreateMap<Category, CategoryAllViewModel>();

            //Positions 
            CreateMap<Position, RegisterEmployeeViewModel>()
                .ForMember(rvm => rvm.PositionId, p => p.MapFrom(x => x.Id));

            //Employee
            CreateMap<RegisterEmployeeInputModel, Employee>();
           
            CreateMap<Employee, EmployeesAllViewModel>()
                .ForMember(evm => evm.Position, e => e.MapFrom(x => x.Position.Name));

            //Items
            CreateMap<Category, CreateItemViewModel>()
                .ForMember(ci => ci.CategoryId, c => c.MapFrom(x => x.Id));

            CreateMap<CreateItemInputModel, Item>();
            CreateMap<Item, ItemsAllViewModels>()
                .ForMember(ivm => ivm.Category, i => i.MapFrom(x => x.Category.Name));

            //Orders
            CreateMap<Order, CreateOrderViewModel>();
            CreateMap<CreateOrderInputModel, Order>();
            CreateMap<Order, OrderAllViewModel>();
        }
    }
}
