using System.Text;

namespace DragaliaBaasServer.Extensions;

public static class RandomExtensions
{
    private static readonly char[] Alphabet =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!$+-".ToCharArray();

    public static string NextString(this Random random, int length = 16)
    {
        var sb = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            sb.Append(Alphabet[random.Next(Alphabet.Length)]);
        }

        return sb.ToString();
    }
}