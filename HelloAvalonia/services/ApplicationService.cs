using System.Threading.Tasks;
using AroniumFactures.Repositories;

namespace AroniumFactures.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationPropertyRepository _propertyRepository;

    public ApplicationService(IApplicationPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<string?> GetPropertyValueAsync(string propertyName)
    {
        return await _propertyRepository.GetValueAsync(propertyName);
    }

    public async Task<string> GetCompanyEmailAsync()
    {
        return await _propertyRepository.GetValueAsync("Application.User.Email") ?? "contact@entreprise.ma";
    }

    public async Task<string> GetCompanyNameAsync()
    {
        return await _propertyRepository.GetValueAsync("company_name") ?? "Votre Entreprise";
    }

    public async Task<string> GetCompanyPhoneAsync()
    {
        return await _propertyRepository.GetValueAsync("company_phone") ?? "+212 522 00 00 00";
    }
}

