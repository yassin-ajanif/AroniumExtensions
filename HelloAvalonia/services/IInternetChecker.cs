using System.Threading;
using System.Threading.Tasks;

namespace AroniumFactures.Services;

public interface IInternetChecker
{
    /// <summary>
    /// Returns true if Internet appears reachable (e.g. by probing a small URL).
    /// </summary>
    Task<bool> HasInternetAsync(CancellationToken cancellationToken = default);
}

