namespace OllamaUi.Interfaces;

public interface IOllamaService
{
    Task<string[]> GetModelListAsync();
    Task GenerateResponseAsync(string prompt);
    bool IsOllamaInstalled();
    void SelectModel(int index);
    string GetSelectedModel();
}