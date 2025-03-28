using OllamaUi.Interfaces;

namespace OllamaUi.Presenters;

public class MainFormPresenter
{
    private readonly IOllamaService _ollamaService;
    private readonly IMainFormView _mainFormView;

    public MainFormPresenter(IOllamaService ollamaService, IMainFormView mainFormView)
    {
        _ollamaService = ollamaService;
        _mainFormView = mainFormView;
    }
    
    public async Task LoadModels()
    {
        var models = await _ollamaService.GetModelListAsync();
        _mainFormView.UpdateModelList(models);
    }
    
    public async Task HandlePrompt(string prompt)
    {
        _mainFormView.ClearResponse();
        await _ollamaService.GenerateResponseAsync(prompt);
    }
    
    public void CheckOllamaInstallation()
    {
        var exists = _ollamaService.IsOllamaInstalled();
        _mainFormView.UpdateStatus(exists ? "Ollama exists on path" : "Ollama does not exist on path");
    }

    public void SelectModel(int index)
    {
        _ollamaService.SelectModel(index);
    }
}