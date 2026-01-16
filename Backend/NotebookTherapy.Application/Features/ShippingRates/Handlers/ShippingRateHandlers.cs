using AutoMapper;
using MediatR;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.ShippingRates.Commands;
using NotebookTherapy.Application.Features.ShippingRates.Queries;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;

namespace NotebookTherapy.Application.Features.ShippingRates.Handlers;

public class ShippingRateHandlers :
    IRequestHandler<GetAllShippingRatesQuery, IEnumerable<ShippingRateDto>>,
    IRequestHandler<CreateShippingRateCommand, ShippingRateDto>,
    IRequestHandler<UpdateShippingRateCommand, bool>,
    IRequestHandler<DeleteShippingRateCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ShippingRateHandlers(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShippingRateDto>> Handle(GetAllShippingRatesQuery request, CancellationToken cancellationToken)
    {
        var rates = await _uow.ShippingRates.GetAllAsync();
        return rates.Select(r => _mapper.Map<ShippingRateDto>(r));
    }

    public async Task<ShippingRateDto> Handle(CreateShippingRateCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ShippingRate>(request.Rate);
        await _uow.ShippingRates.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return _mapper.Map<ShippingRateDto>(entity);
    }

    public async Task<bool> Handle(UpdateShippingRateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.ShippingRates.GetByIdAsync(request.Id);
        if (entity == null) return false;

        _mapper.Map(request.Rate, entity);
        await _uow.ShippingRates.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Handle(DeleteShippingRateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.ShippingRates.GetByIdAsync(request.Id);
        if (entity == null) return false;

        await _uow.ShippingRates.DeleteAsync(entity);
        await _uow.SaveChangesAsync();
        return true;
    }
}
