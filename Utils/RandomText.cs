using System.Security.Cryptography;
using System.Text;

namespace MiniCtf.Utils;

public static class RandomText
{
    private static readonly char[] AlnumUnderscore =
        "abcdefghijklmnopqrstuvwxyz0123456789_".ToCharArray();

    public static string Token(int length)
    {
        if (length <= 0) length = 8;
        var sb = new StringBuilder(length);
        var buf = new byte[length];
        RandomNumberGenerator.Fill(buf);
        for (int i = 0; i < length; i++)
        {
            int idx = buf[i] % AlnumUnderscore.Length;
            sb.Append(AlnumUnderscore[idx]);
        }
        return sb.ToString();
    }
}
