using System.Net.Http.Json;
using BankBlazor.Client.Models;
using BankBlazor.Shared;

namespace BankBlazor.Client.Services;

public class CustomersApi
{
    private readonly HttpClient _http;

    public CustomersApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<PagedResult<CustomerDto>> GetCustomers(int page, int pageSize)
    {
        var result = await _http.GetFromJsonAsync<PagedResult<CustomerDto>>(
            $"api/customers/paginated?page={page}&pageSize={pageSize}");

        return result ?? new PagedResult<CustomerDto>();
    }

    public async Task<CustomerDto?> GetCustomerById(int id)
    {
        return await _http.GetFromJsonAsync<CustomerDto>($"api/customers/{id}");
    }
}