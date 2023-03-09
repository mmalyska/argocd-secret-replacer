namespace ReplacerE2ETests.Utils;

using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xunit;

public static class FileAssert
{
    private static string GetFileHash(string filename)
    {
        Assert.True(File.Exists(filename));
        var clearBytes = File.ReadAllBytes(filename);
        var hashedBytes = SHA512.HashData(clearBytes);
        return ConvertBytesToHex(hashedBytes);
    }

    private static string GetStringHash(string text)
    {
        var clearBytes = Encoding.ASCII.GetBytes(text);
        var hashedBytes = SHA512.HashData(clearBytes);
        return ConvertBytesToHex(hashedBytes);
    }

    private static string ConvertBytesToHex(byte[] bytes)
    {
        var sb = new StringBuilder();

        foreach (var t in bytes)
        {
            sb.Append(t.ToString("x", CultureInfo.InvariantCulture));
        }

        return sb.ToString();
    }

    public static void IsEqualToFile(string file, string output)
    {
        var hash1 = GetFileHash(file);
        var hash2 = GetStringHash(output);

        Assert.Equal(hash1, hash2);
    }
}
