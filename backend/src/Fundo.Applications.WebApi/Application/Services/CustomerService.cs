using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fundo.Applications.WebApi.Application.DTOs;
using Fundo.Applications.WebApi.Domain.Entities;
using Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories;

namespace Fundo.Applications.WebApi.Application.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository customers;
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public CustomerService(ICustomerRepository customers, IUnitOfWork uow, IMapper mapper)
        {
            this.customers = customers;
            this.uow = uow;
            this.mapper = mapper;
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerRequestDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ArgumentException("FullName is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.");

            var entity = mapper.Map<Customer>(dto);

            await customers.AddAsync(entity, ct);
            await uow.SaveChangesAsync(ct);

            return mapper.Map<CustomerDto>(entity);
        }

        public async Task<List<CustomerDto>> GetAllAsync(CancellationToken ct = default)
        {
            var list = await customers.GetAllAsync(ct);
            return mapper.Map<List<CustomerDto>>(list);
        }
    }
}
