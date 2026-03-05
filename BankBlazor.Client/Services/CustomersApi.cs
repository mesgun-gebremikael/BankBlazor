using System.Net.Http.Json;
using BankBlazor.Client.Models;

namespace BankBlazor.Client.Services;

public class CustomersApi
{
    private readonly HttpClient _http;

    public CustomersApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<PagedResult<CustomerDto>?> GetCustomers(int page, int pageSize)
    {
        return await _http.GetFromJsonAsync<PagedResult<CustomerDto>>(
            $"api/customers/paginated?page={page}&pageSize={pageSize}");
    }

    public async Task<CustomerDto?> GetCustomerById(int id)
    {
        return await _http.GetFromJsonAsync<CustomerDto>($"api/customers/{id}");
    }
}