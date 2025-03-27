using System.Text;
using OllamaSharp;
using OllamaSharp.Models;

namespace OllamaUi;

public partial class Form1 : Form
{
    private Button button1;
    private Button button2;
    private Button button3;
    private Label label1;
    private TextBox textBox1;
    private TextBox textBox2;
    private ComboBox comboBox1;

    private Dictionary<string, Model> _ollamaModels = new();
    private readonly OllamaApiClient oClient;
    private readonly StringBuilder _stringBuilder = new();


    private int _clickCount = 0;
    private int _doubleClickCount = 0;

    private int _pad = 10;
    private int _buttonHeight = 30;

    public Form1()
    {
        InitializeComponent();
        DefaultOverrides();
        var uri = new Uri("http://localhost:11434");
        oClient = new OllamaApiClient(uri);
        AddControls();
    }

    private void DefaultOverrides()
    {
        Text = "OllamaUi";
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(500, 500);
        ClientSizeChanged += OnClientSizeChanged;
    }

    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        textBox1.Size = new Size(ClientSize.Width - _pad, 100);
        textBox2.Size = new Size(ClientSize.Width - _pad, 200);
        label1.Size = new Size(ClientSize.Width - _pad, 100);
    }

    private void AddControls()
    {
        label1 = new Label();
        label1.Text = $"you havent clicked the button yet";
        label1.Size = ClientSize with { Height = 20 };
        label1.Location = new Point(_pad, 10);

        button2 = new Button();
        button2.Text = "Check if Ollama Exists";
        button2.Size = new Size(button2.Text.Length * 5, _buttonHeight + button2.Text.Length);
        button2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        button2.Location = new Point(_pad, label1.Location.Y + label1.Size.Height);

        button3 = new Button();
        button3.Text = "Connection Test";
        button3.Size = new Size(button2.Text.Length * 5, _buttonHeight + button2.Text.Length);
        button3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        button3.Location = new Point(_pad + label1.Location.X + button2.Size.Width,
            _pad + label1.Size.Height);

        comboBox1 = new ComboBox();
        comboBox1.Size = new Size(200, 60);
        comboBox1.Location = new Point(_pad + button3.Location.X + button3.Size.Width,
            _pad + label1.Size.Height);
        comboBox1.SelectionChangeCommitted += ComboBox1OnSelectionChangeCommitted;

        textBox1 = new TextBox();
        textBox1.Size = new Size(ClientSize.Width - _pad, 100);
        textBox1.Multiline = true;
        textBox1.AcceptsReturn = true;
        textBox1.PlaceholderText = "Write your prompt here";
        textBox1.ScrollBars = ScrollBars.Vertical;
        textBox1.Location = new Point(_pad, _pad + button2.Location.Y + button2.Size.Height);

        button1 = new Button();
        button1.Text = "Ask !";
        button1.Size = new Size(80, _buttonHeight);
        button1.Location = new Point(_pad, _pad + textBox1.Location.Y + textBox1.Size.Height);

        textBox2 = new TextBox();
        textBox2.Text = "";
        textBox2.Size = new Size(ClientSize.Width - _pad, 200);
        textBox2.Location = new Point(_pad, _pad + button1.Location.Y + button1.Size.Height);
        textBox2.Multiline = true;
        textBox2.ScrollBars = ScrollBars.Vertical;
        textBox2.ReadOnly = true;


        Controls.Add(label1);
        Controls.Add(button1);
        Controls.Add(button2);
        Controls.Add(button3);
        Controls.Add(textBox1);
        Controls.Add(comboBox1);
        Controls.Add(textBox2);

        button1.Click += button1_AskLlm;
        button2.Click += button2_CheckOllamaExists;
        button3.Click += button2_ConnectionTest;
    }

    private void ComboBox1OnSelectionChangeCommitted(object? sender, EventArgs e)
    {
        var index = comboBox1.SelectedIndex;
        Console.WriteLine(index);
        var selectedModel = _ollamaModels.Values.ElementAt(index);
        Console.WriteLine(selectedModel.Name);
        SetModel(selectedModel);
    }

    private void SetModel(Model selectedModel)
    {
        oClient.SelectedModel = selectedModel.Name;
    }

    private async void button2_ConnectionTest(object? sender, EventArgs e)
    {
        var list = await oClient.ListLocalModelsAsync();
        _ollamaModels = list.ToDictionary(x => x.Name, x => x);
        var llmNameList = _ollamaModels.Keys.ToArray();
        Console.WriteLine(llmNameList[0]);
        comboBox1.Items.Clear();
        comboBox1.Items.AddRange(llmNameList);
    }

    private async void button1_AskLlm(object? sender, EventArgs e)
    {
        await MakeQuery(textBox1.Text);
        // UpdateLabel($"you clicked {_clickCount}");
    }

    private async Task MakeQuery(string prompt)
    {
        if (oClient == null) UpdateLabel("Ollama Client not Ready");
        if (string.IsNullOrWhiteSpace(oClient.SelectedModel))
        {
            UpdateLabel("No Model Selected");
            return;
        }

        if (prompt == string.Empty)
        {
            UpdateLabel("Prompt is empty");
            return;
        }

        textBox2.Text = string.Empty;
        await foreach (var stream in oClient.GenerateAsync(prompt))
        {
            _stringBuilder.Append(stream.Response);
            UpdateStreamingText(stream.Response);
        }

        // UpdateLabel(_stringBuilder.ToString());

        _stringBuilder.Clear();
    }

    private void UpdateStreamingText(string streamResponse)
    {
        textBox2.Text += streamResponse;
        textBox2.Refresh();
    }

    private void button2_CheckOllamaExists(object? sender, EventArgs e)
    {
        UpdateLabel(ExistsOnPath("ollama.exe")
            ? "Ollama exists on path"
            : "Ollama does not exist on path");
    }

    private static bool ExistsOnPath(string fileName)
    {
        return GetFullPath(fileName) != string.Empty;
    }

    private static string GetFullPath(string fileName)
    {
        if (File.Exists(fileName))
            return Path.GetFullPath(fileName);

        var values = Environment.GetEnvironmentVariable("PATH");
        foreach (var path in values.Split(Path.PathSeparator))
        {
            var fullPath = Path.Combine(path, fileName);
            if (File.Exists(fullPath))
                return fullPath;
        }

        return string.Empty;
    }


    private void UpdateLabel(string message)
    {
        label1.Text = message;
    }
}