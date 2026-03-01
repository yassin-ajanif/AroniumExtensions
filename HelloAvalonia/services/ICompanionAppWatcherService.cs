namespace AroniumFactures.Services;

public interface ICompanionAppWatcherService
{
    void Start(int watchedProcessId);
    void Stop();
}
