using Microsoft.Xna.Framework.Content;
using System;
using System.IO;

namespace BinksFarm;

public class LogManager
{
    private static string _filename { get; set; }
    public static void InitializeLoggingFile()
    {
        if (!Directory.Exists(".\\logs"))
        {
            Directory.CreateDirectory(".\\logs");
        }
        var dateTime = DateTime.Now;
        _filename = $"logs\\BinksFarm [{((DateTimeOffset)dateTime).ToUnixTimeSeconds()}].txt";
    }
    public static string Log(string message, string type = "LOG")
    {
        var dateTime = DateTime.Now;
        var logMessage = $"{dateTime.ToString("MM/dd/yyyy H:mm:ss.fff")} [{type}] -> {message}";

        var file = new StreamWriter(_filename, append: true);
        file.WriteLineAsync(logMessage);
        file.Close();

        return logMessage;
    }

    public static void Error(string message)
    {
        Log(message, "ERROR");
    }

    public static void Debug(string message)
    {
        var logMessage = Log(message, "DEBUG");
        Console.WriteLine(logMessage);
    }

    public static void LoadContent(string filename)
    {
        Log($"Loading Content File [{filename}.xnb]", "CONTENT");
    }

    public static T Load<T>(string filename, ContentManager content)
    {
        try
        {
            var msg = $"Loading Content File [{filename}.xnb]";
            Log(msg, "CONTENT");
            Console.WriteLine(msg);
            return content.Load<T>(filename);
        }
        catch (Exception e)
        {
            _ = Log($"Failed to load file [{filename}]: {e.Message}");
            throw;
        }
    }
}
