namespace ReplacerE2ETests.Utils;

using System;
using System.IO;

public class ConsoleOutput : IDisposable
{
    private StringWriter stringWriter;
    private TextWriter originalOutput;
    private bool disposedValue;

    public ConsoleOutput()
    {
        stringWriter = new StringWriter();
        originalOutput = Console.Out;
        Console.SetOut(stringWriter);
    }

    public string GetOuput()
    {
        return stringWriter.ToString();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Console.SetOut(originalOutput);
                stringWriter.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class ConsoleInput : IDisposable
{
    private TextReader originalInput;
    private TextReader? input;
    private bool disposedValue;

    private ConsoleInput(string text)
    {
        input = new StringReader(text);
        originalInput = Console.In;
        Console.SetIn(input);
    }

    private ConsoleInput(TextReader reader)
    {
        originalInput = Console.In;
        input = reader;
        Console.SetIn(reader);
    }

    public static ConsoleInput FromFile(string filePath){
        var reader = File.OpenText(filePath);
        return new ConsoleInput(reader);
    }

    public static ConsoleInput FromString(string text){
        return new ConsoleInput(text);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Console.SetIn(originalInput);
                input?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
