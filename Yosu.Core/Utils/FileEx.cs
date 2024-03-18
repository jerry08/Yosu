using System;
using System.Text;

namespace Yosu.Core.Utils;

public class FileEx
{
    // Checks if thumbnail's data is valid Latin1 encoding (ISO-8859-1)
    //private static bool IsValidISO(string input)
    //{
    //    //byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(input);
    //    //String result = Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
    //
    //    var bytes = Encoding.Latin1.GetBytes(input);
    //    var result = Encoding.Latin1.GetString(bytes);
    //    return String.Equals(input, result);
    //}

    /// <summary>
    /// Checks if thumbnail's data is valid Latin1 encoding (ISO-8859-1)
    /// </summary>
    /// <param name="input"></param>
    public static bool IsValidISO(string input) =>
        string.Equals(input, Encoding.Latin1.GetString(Encoding.Latin1.GetBytes(input)));

    /// <summary>
    /// Checks if thumbnail's data is valid Latin1 encoding (ISO-8859-1)
    /// </summary>
    /// <param name="input"></param>
    public static bool IsValidISO(byte[] input) => IsValidISO(BitConverter.ToString(input));
}
