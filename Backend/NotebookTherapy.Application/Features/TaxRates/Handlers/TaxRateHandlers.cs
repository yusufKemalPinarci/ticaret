using System.Linq;
using AutoMapper;
using MediatR;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.TaxRates.Commands;
using NotebookTherapy.Application.Features.TaxRates.Queries;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;

namespace NotebookTherapy.Application.Features.TaxRates.Handlers;

public class TaxRateHandlers :
    IRequestHandler<GetAllTaxRatesQuery, IEnumerable<TaxRateDto>>,
    IRequestHandler<CreateTaxRateCommand, TaxRateDto>,
    IRequestHandler<UpdateTaxRateCommand, bool>,
    IRequestHandler<DeleteTaxRateCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public TaxRateHandlers(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TaxRateDto>> Handle(GetAllTaxRatesQuery request, CancellationToken cancellationToken)
    {
        var rates = await _uow.TaxRates.GetAllAsync();
        return rates.Select(r => _mapper.Map<TaxRateDto>(r));
    }

    public async Task<TaxRateDto> Handle(CreateTaxRateCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<TaxRate>(request.Rate);
        await _uow.TaxRates.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return _mapper.Map<TaxRateDto>(entity);
    }

    public async Task<bool> Handle(UpdateTaxRateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.TaxRates.GetByIdAsync(request.Id);
        if (entity == null) return false;

        _mapper.Map(request.Rate, entity);
        await _uow.TaxRates.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Handle(DeleteTaxRateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _uow.TaxRates.GetByIdAsync(request.Id);
        if (entity == null) return false;

        await _uow.TaxRates.DeleteAsync(entity);
        await _uow.SaveChangesAsync();
        return true;
    }
}
