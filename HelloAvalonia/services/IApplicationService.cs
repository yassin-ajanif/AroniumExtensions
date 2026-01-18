using System.Threading.Tasks;

namespace AroniumFactures.Services;

public interface IApplicationService
{
    Task<string?> GetPropertyValueAsync(string propertyName);
    Task<string> GetCompanyEmailAsync();
    Task<string> GetCompanyNameAsync();
    Task<string> GetCompanyPhoneAsync();
}















































