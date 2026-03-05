using System.Net.Http.Json;
using BankBlazor.Client.Pages;

namespace BankBlazor.Client.Services
{
    public class CustomersApi
    {
        private readonly HttpClient _http;

        public CustomersApi(HttpClient http)
        {
            _http = http;
        }

        public async Task<Customers.razor.PagedResult<Customers.razor.Customer>?> GetCustomers(int page, int pageSize)
        {
            return await _http.GetFromJsonAsync<Customers.razor.PagedResult<Customers.razor.Customer>>(
                $"api/customers/paginated?page={page}&pageSize={pageSize}");
        }
    }
}