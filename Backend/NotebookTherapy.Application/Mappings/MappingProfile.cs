using AutoMapper;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product Mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        
        // Category Mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
        
        // Cart Mappings
        CreateMap<Cart, CartDto>()
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Items.Sum(i => i.Quantity)))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Items.Sum(i => i.Quantity * i.UnitPrice)));
        
        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
            .ForMember(dest => dest.ProductVariantId, opt => opt.MapFrom(src => src.ProductVariantId))
            .ForMember(dest => dest.VariantSku, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.SKU : null))
            .ForMember(dest => dest.VariantColor, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Color : null))
            .ForMember(dest => dest.VariantSize, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Size : null))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
        
        // Order Mappings
        CreateMap<Order, OrderDto>();

        // Users
        CreateMap<User, UserDto>();
        
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
            .ForMember(dest => dest.ProductVariantId, opt => opt.MapFrom(src => src.ProductVariantId))
            .ForMember(dest => dest.VariantSku, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.SKU : null))
            .ForMember(dest => dest.VariantColor, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Color : null))
            .ForMember(dest => dest.VariantSize, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Size : null));

        // Admin create/update DTOs
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Coupons
        CreateMap<Coupon, CouponDto>();
        CreateMap<CreateCouponDto, Coupon>();
        CreateMap<UpdateCouponDto, Coupon>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Shipping rates
        CreateMap<ShippingRate, ShippingRateDto>();
        CreateMap<CreateShippingRateDto, ShippingRate>();
        CreateMap<UpdateShippingRateDto, ShippingRate>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Tax rates
        CreateMap<TaxRate, TaxRateDto>();
        CreateMap<CreateTaxRateDto, TaxRate>();
        CreateMap<UpdateTaxRateDto, TaxRate>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    }
}
