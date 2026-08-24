
using AutoMapper;
using SuperMarket.Application.DTOs.Dashboard;
using SuperMarket.Domain.Enums;
using SuperMarket.Web.Areas.Admin.ViewModels.Dashboard;
using SuperMarket.Web.Areas.Admin.ViewModels.Orders;

namespace SuperMarket.Web.Mapping
{
    public sealed class DashboardMappingProfile : Profile
    {
        public DashboardMappingProfile()
        {
            CreateMap<DashboardDto, DashboardViewModel>();
            CreateMap<DashboardStatisticsDto, DashboardStatisticsViewModel>();
            CreateMap<DashboardSalesDto, DashboardSalesViewModel>();

            CreateMap<RecentOrderDto, DashboardRecentOrderViewModel>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => MapOrderStatus(src.OrderStatus)))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => MapPaymentStatus(src.PaymentStatus)));

            CreateMap<RecentUserDto, DashboardRecentUserViewModel>();
            CreateMap<LowStockProductDto, DashboardLowStockViewModel>();
        }

        private static OrderStatusViewModel MapOrderStatus(OrderStatus domainStatus)
        {
            return domainStatus switch
            {
                OrderStatus.Pending => OrderStatusViewModel.Pending,
                OrderStatus.Processing => OrderStatusViewModel.Pending,
                OrderStatus.Shipped => OrderStatusViewModel.Shipped,
                OrderStatus.Delivered => OrderStatusViewModel.Delivered,
                OrderStatus.Canceled => OrderStatusViewModel.Cancelled,
                OrderStatus.Returned => OrderStatusViewModel.Refunded,
                _ => OrderStatusViewModel.Pending
            };
        }

        private static PaymentStatusViewModel MapPaymentStatus(PaymentStatus domainStatus)
        {
            return domainStatus switch
            {
                PaymentStatus.Pending => PaymentStatusViewModel.Pending,
                PaymentStatus.Processing => PaymentStatusViewModel.Pending,
                PaymentStatus.Paid => PaymentStatusViewModel.Paid,
                PaymentStatus.Failed => PaymentStatusViewModel.Failed,
                PaymentStatus.Refunded => PaymentStatusViewModel.Refunded,
                PaymentStatus.Canceled => PaymentStatusViewModel.Pending,
                _ => PaymentStatusViewModel.Pending
            };
        }
    }
}
