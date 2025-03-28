using OllamaSharp;
using OllamaSharp.Models;
using OllamaUi.Interfaces;
using OllamaUi.Utils;

/// <summary>
/// Ollama handler class to configure the OllamaApiClient
/// </summary>
public class OllamaService : IOllamaService
{
    private readonly IOllamaResponseHandler _responseHandler;
    private readonly OllamaApiClient _oClient;
    
    private Dictionary<string, Model> _ollamaModels = new();

    public OllamaService()
    {
        var uri = new Uri("http://localhost:11434");
        _oClient = new OllamaApiClient(uri);
    }
    
    public OllamaService(IOllamaResponseHandler responseHandler)
    {
        var uri = new Uri("http://localhost:11434");
        _oClient = new OllamaApiClient(uri);
        _responseHandler = responseHandler;
        
    }
    
    /// <summary>
    /// return the OllamaApiClient object
    /// </summary>
    /// <returns></returns>
    public OllamaApiClient GetClient() => _oClient;

    /// <summary>
    /// Checks if Ollama exists on the system
    /// </summary>
    /// <returns>true if exists</returns>
    public bool IsOllamaInstalled() => Util.ExecExistsOnPath("ollama.exe");

    /// <summary>
    /// Get list of models available in Ollama
    /// </summary>
    /// <returns>names of items in array</returns>
    public async Task<string[]> GetModelListAsync()
    {
        var list = await _oClient.ListLocalModelsAsync();
        _ollamaModels = list.ToDictionary(x => x.Name, x => x);
        return _ollamaModels.Keys.ToArray();
    }

    /// <summary>
    /// Select the model from the list
    /// </summary>
    /// <param name="index"></param>
    public void SelectModel(int index)
    {
        var selectedModel = _ollamaModels.Values.ElementAt(index);
        _oClient.SelectedModel = selectedModel.Name;
    }
    
    /// <summary>
    /// Get the model selected by the user
    /// </summary>
    /// <returns>empty if not selected else the name</returns>
    public string GetSelectedModel() => _oClient.SelectedModel;

    /// <summary>
    /// Use this to send prompt to the Ollama API
    /// </summary>
    /// <param name="prompt"></param>
    public async Task GenerateResponseAsync(string prompt)
    {
        if (string.IsNullOrWhiteSpace(GetSelectedModel()))
        {
            _responseHandler.OnError("No Model Selected");
            return;
        }

        if (prompt == string.Empty)
        {
            _responseHandler.OnError("Prompt is empty");
            return;
        }
        
        await foreach (var stream in _oClient.GenerateAsync(prompt))
        {
            _responseHandler.OnResponse(stream?.Response ?? string.Empty);
        }
    }
}