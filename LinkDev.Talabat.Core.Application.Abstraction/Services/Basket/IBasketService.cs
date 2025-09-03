﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Basket;

namespace LinkDev.Talabat.Core.Application.Abstraction.Services.Basket
{
    public interface IBasketService
    {
        Task<CustomerBasketDto?> GetCustomerBasketAsync(string basketId);
        Task<CustomerBasketDto> UpdateCustomerBasketAsync(CustomerBasketDto basketDto);
        Task DeleteCustomerBasketAsync(string basketId);
    }
}
