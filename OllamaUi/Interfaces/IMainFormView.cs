namespace OllamaUi.Presenters;

public interface IMainFormView
{
    void UpdateStatus(string message);
    void UpdateResponse(string response);
    void UpdateModelList(string[] models);
    void ClearResponse();
}