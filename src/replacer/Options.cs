namespace replacer;

using CommandLine;

public class Options
{
    //[Option('f', "file", Required = true, HelpText = "File encrypted with sops containing json object.")]
    public string? FileName { get; set; }
}
