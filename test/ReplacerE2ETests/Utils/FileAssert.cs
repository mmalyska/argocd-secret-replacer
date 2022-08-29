namespace ReplacerE2ETests.Utils;

using System.IO;
using System.Text;
using System.Security.Cryptography;
using Xunit;
using System.Globalization;

public static class FileAssert
{
    static string GetFileHash(string filename)
    {
        Assert.True(File.Exists(filename));

        using (var hash = SHA1.Create())
        {
            var clearBytes = File.ReadAllBytes(filename);
            var hashedBytes = hash.ComputeHash(clearBytes);
            return ConvertBytesToHex(hashedBytes);
        }
    }

    static string GetStringHash(string text)
    {
        using (var hash = SHA1.Create())
        {
            var clearBytes = Encoding.ASCII.GetBytes(text);
            var hashedBytes = hash.ComputeHash(clearBytes);
            return ConvertBytesToHex(hashedBytes);
        }
    }

    static string ConvertBytesToHex(byte[] bytes)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("x", CultureInfo.InvariantCulture));
        }
        return sb.ToString();
    }

    public static void IsEqualToFile(string file, string output)
    {
        string hash1 = GetFileHash(file);
        string hash2 = GetStringHash(output);

        Assert.Equal(hash1, hash2);
    }
}
