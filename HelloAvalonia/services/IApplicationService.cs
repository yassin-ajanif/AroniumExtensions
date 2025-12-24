using System.Threading.Tasks;

namespace HelloAvalonia.Services;

public interface IApplicationService
{
    Task<string?> GetPropertyValueAsync(string propertyName);
    Task<string> GetCompanyEmailAsync();
    Task<string> GetCompanyNameAsync();
    Task<string> GetCompanyPhoneAsync();
}










