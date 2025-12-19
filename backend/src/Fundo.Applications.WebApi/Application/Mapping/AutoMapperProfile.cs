using AutoMapper;
using Fundo.Applications.WebApi.Application.DTOs;
using Fundo.Applications.WebApi.Domain.Entities;

namespace Fundo.Applications.WebApi.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, CustomerDto>();

            CreateMap<CreateCustomerRequestDto, Customer>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Loans, opt => opt.Ignore());

            CreateMap<Loan, LoanListItemDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString().ToLowerInvariant()));

            CreateMap<Loan, LoanDetailDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString().ToLowerInvariant()));

            CreateMap<CreateLoanRequestDto, Loan>()
                .ForMember(d => d.OriginalAmount, opt => opt.MapFrom(s => s.Amount))
                .ForMember(d => d.CurrentBalance, opt => opt.Ignore())
                .ForMember(d => d.Status, opt => opt.Ignore())
                .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(d => d.Customer, opt => opt.Ignore())
                .ForMember(d => d.Id, opt => opt.Ignore());
        }
    }
}
