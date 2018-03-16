using AutoMapper;

namespace Kodhier
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<Areas.Admin.ViewModels.PizzaViewModel, Models.Pizza>().ReverseMap();
                cfg.CreateMap<ViewModels.OrderViewModel, Models.Order>().ReverseMap();
            });
        }
    }
}
