using AutoMapper;
using MediatR;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotebookTherapy.Application.Features.Wishlist.Handlers;

public class WishlistHandlers :
    IRequestHandler<GetMyWishlistQuery, IEnumerable<WishlistDto>>,
    IRequestHandler<AddToWishlistCommand, bool>,
    IRequestHandler<RemoveFromWishlistCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public WishlistHandlers(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WishlistDto>> Handle(GetMyWishlistQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Wishlist.GetByUserIdAsync(request.UserId);
        return items.Select(i => new WishlistDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.Product.Name,
            ProductImageUrl = i.Product.ImageUrl,
            ProductPrice = i.Product.Price,
            ProductDiscountPrice = i.Product.DiscountPrice
        });
    }

    public async Task<bool> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
    {
        var existing = await _uow.Wishlist.GetByUserAndProductAsync(request.UserId, request.ProductId);
        if (existing != null) return true;

        var item = new WishlistItem
        {
            UserId = request.UserId,
            ProductId = request.ProductId
        };

        await _uow.Wishlist.AddAsync(item);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
    {
        var existing = await _uow.Wishlist.GetByUserAndProductAsync(request.UserId, request.ProductId);
        if (existing == null) return true;

        await _uow.Wishlist.DeleteAsync(existing);
        await _uow.SaveChangesAsync();
        return true;
    }
}
