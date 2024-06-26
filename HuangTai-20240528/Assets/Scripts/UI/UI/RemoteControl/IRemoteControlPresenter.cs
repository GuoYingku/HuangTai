public interface IRemoteControlPresenter : IUIPresenter
{
    void StartBelt();
    void StopBelt();
    void BeltRotateClockwise(float deltaTime);
    void BeltRotateCounterclockwise(float deltaTime);
    void StartScraper();
    void StopScraper();
    void ScraperRotateClockwise(float deltaTime);
    void ScraperRotateCounterclockwise(float deltaTime);
    void ScraperPitchUp(float deltaTime);
    void ScraperPitchDown(float deltaTime);
}