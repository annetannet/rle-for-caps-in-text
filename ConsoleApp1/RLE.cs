using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1;

public static class RleDeltaEncoder
{
    // Кодирование битового массива с использованием RLE + Delta Code
    public static string Encode(bool[] bits)
    {
        var rlePairs = RunLengthEncode(bits);
        var encoded = new StringBuilder();

        foreach (var (length, value) in rlePairs)
        {
            // Преобразуем пару (length, value) в число
            int n = value ? 2 * length : 2 * length - 1;
            encoded.Append(DeltaEncode(n));
        }

        return encoded.ToString();
    }

    // Декодирование обратно в битовый массив
    public static bool[] Decode(string deltaCode)
    {
        var rlePairs = new List<(int length, bool value)>();
        int pos = 0;

        while (pos < deltaCode.Length)
        {
            // Декодируем Delta-код
            var (n, bitsRead) = DeltaDecode(deltaCode, pos);
            pos += bitsRead;

            // Преобразуем число обратно в пару (length, value)
            bool value = n % 2 == 0;
            int length = value ? n / 2 : (n + 1) / 2;
            rlePairs.Add((length, value));
        }

        return RunLengthDecode(rlePairs);
    }

    // RLE-кодирование
    public static List<(int length, bool value)> RunLengthEncode(bool[] bits)
    {
        var pairs = new List<(int, bool)>();
        if (bits.Length == 0) return pairs;

        bool current = bits[0];
        int count = 1;

        for (int i = 1; i < bits.Length; i++)
        {
            if (bits[i] == current)
            {
                count++;
            }
            else
            {
                pairs.Add((count, current));
                current = bits[i];
                count = 1;
            }
        }

        pairs.Add((count, current));

        return pairs;
    }

    // RLE-декодирование
    private static bool[] RunLengthDecode(List<(int length, bool value)> pairs)
    {
        var result = new List<bool>();
        foreach (var (length, value) in pairs)
        {
            for (int i = 0; i < length; i++)
            {
                result.Add(value);
            }
        }

        return result.ToArray();
    }

    // Delta-кодирование числа
    public static string DeltaEncode(int n)
    {
        if (n <= 0) throw new ArgumentException("n must be positive");

        // N = 2^exp + remainder
        int exp = (int)Math.Log(n, 2);
        string expGammaEncoded = GammaEncode(exp + 1);
        string remainder = Convert.ToString(n, 2)[1..];

        return expGammaEncoded + remainder;
    }

    public static string GammaEncode(int n)
    {
        if (n <= 0) throw new ArgumentException("n must be positive");

        // N = 2^exp + remainder
        int exp = (int)Math.Log(n, 2);
        string gamma = new string('0', exp) + Convert.ToString(n, 2);

        return gamma;
    }

    public static (int number, int bitsRead) DeltaDecode(string code, int startPos)
    {
        // 1. Декодируем exp числа из Gamma-кода
        var (expPlusOne, gammaBits) = GammaDecode(code, startPos);
        var afterExpPos = startPos + gammaBits;

        var length = expPlusOne - 1;
        if (length == 0)
            return (number: 1, bitsRead: 1);
        // 2. Читаем остаток числа (length бит)
        if (afterExpPos + length > code.Length)
            throw new FormatException("Invalid Delta code");

        string remainder = code.Substring(afterExpPos, length);
        var endPos = afterExpPos + length;

        // 3. Восстанавливаем исходное число: добавляем старшую единицу
        int number = (1 << length) | Convert.ToInt32(remainder, 2);

        return (number, endPos - startPos);
    }

    public static (int number, int bitsRead) GammaDecode(string code, int startPos)
    {
        int zeros = 0;
        while (startPos + zeros < code.Length && code[startPos + zeros] == '0')
            zeros++;

        if (startPos + zeros + (zeros + 1) > code.Length)
            throw new FormatException("Invalid Gamma code");

        string binary = code.Substring(startPos + zeros, zeros + 1);
        int number = Convert.ToInt32(binary, 2);

        return (number, zeros + 1 + zeros);
    }
}

public class ProgramTest
{
    public static void MainTest()
    {
        // Пример использования
        bool[] original = new bool[]
        {
            false, false, false, false, false, false, false, false,
            true, true, true,
            false, false, false, false, false, false,
            true,
            false, false, false, false
        };

        Console.WriteLine("Original bits:");
        Console.WriteLine(string.Join("", original.Select(b => b ? "1" : "0")));

        string encoded = RleDeltaEncoder.Encode(original);
        Console.WriteLine("\nEncoded Delta:");
        Console.WriteLine(encoded);

        bool[] decoded = RleDeltaEncoder.Decode(encoded);
        Console.WriteLine("\nDecoded bits:");
        Console.WriteLine(string.Join("", decoded.Select(b => b ? "1" : "0")));

        Console.WriteLine("\nOriginal and decoded are equal: " +
                          original.SequenceEqual(decoded));
    }
}