using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AroniumFactures.Services;

public class InternetCheckerService : IInternetChecker
{
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(3)
    };

    public async Task<bool> HasInternetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Lightweight connectivity probe; returns 204 when online.
            using var response = await _httpClient.GetAsync(
                "https://www.google.com/generate_204",
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch(Exception ex)
        {
            return false;
        }
    }
}

