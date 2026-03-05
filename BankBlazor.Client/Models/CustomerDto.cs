namespace BankBlazor.Client.Models;

public class CustomerDto
{
    public int CustomerId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }

    public List<AddressDto> Addresses { get; set; } = new();
    public List<OrderDto> Orders { get; set; } = new();
}

public class AddressDto
{
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? CountryRegion { get; set; }
}

public class OrderDto
{
    public int SalesOrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalDue { get; set; }
}