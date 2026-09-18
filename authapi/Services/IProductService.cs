using authapi.DTOs;

namespace authapi.Services;

public interface IProductService
{
    Task<ProductResponse> CreateAsync(ProductCreate dto);

    Task<ProductResponse?> GetByNameAsync(string name);

    Task<List<ProductResponse>> GetAllAsync();

    Task<bool> UpdateAsync(
        string id,
        ProductUpdate dto);

    Task<bool> DeleteAsync(string name);
}