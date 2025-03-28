namespace OllamaUi.Utils;

public class Util
{
    /// <summary>
    /// Checks if an executable exists on the Environment PATH <br />
    /// make sure to send the executable name WITH the extension
    /// </summary>
    /// <param name="execName"></param>
    /// <returns>boolean</returns>
    public static bool ExecExistsOnPath(string execName)
    {
        return GetFullPath(execName) != string.Empty;
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
}