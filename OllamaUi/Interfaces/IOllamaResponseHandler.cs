namespace OllamaUi.Interfaces;

public interface IOllamaResponseHandler
{
    void OnError(string message);
    void OnResponse(string response);
}