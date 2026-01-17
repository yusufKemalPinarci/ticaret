using MediatR;
using NotebookTherapy.Application.DTOs;
using System.Collections.Generic;

namespace NotebookTherapy.Application.Features.Wishlist;

public record GetMyWishlistQuery(int UserId) : IRequest<IEnumerable<WishlistDto>>;
public record AddToWishlistCommand(int UserId, int ProductId) : IRequest<bool>;
public record RemoveFromWishlistCommand(int UserId, int ProductId) : IRequest<bool>;
