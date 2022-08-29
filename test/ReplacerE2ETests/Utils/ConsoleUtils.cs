namespace ReplacerE2ETests.Utils;

using System;
using System.IO;

public class ConsoleOutput : IDisposable
{
    private bool disposedValue;
    private readonly TextWriter originalOutput;
    private readonly StringWriter stringWriter;

    public ConsoleOutput()
    {
        stringWriter = new StringWriter();
        originalOutput = Console.Out;
        Console.SetOut(stringWriter);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public string GetOuput()
        => stringWriter.ToString();

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
}

public class ConsoleInput : IDisposable
{
    private bool disposedValue;
    private readonly TextReader? input;
    private readonly TextReader originalInput;

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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public static ConsoleInput FromFile(string filePath)
    {
        var reader = File.OpenText(filePath);
        return new ConsoleInput(reader);
    }

    public static ConsoleInput FromString(string text)
        => new ConsoleInput(text);

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
}
