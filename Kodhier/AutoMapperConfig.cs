﻿using AutoMapper;
using Kodhier.ViewModels.Admin.PizzaViewModels;
using Kodhier.ViewModels.OrderViewModels;

namespace Kodhier
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<PizzaViewModel, Models.Pizza>().ReverseMap();
                cfg.CreateMap<OrderViewModel, Models.Order>().ReverseMap();
                cfg.CreateMap<ViewModels.PrepaidCardViewModel, Models.PrepaidCode>().ReverseMap();
            });
        }
    }
}
