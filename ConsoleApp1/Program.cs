namespace ConsoleApp1;

public static class Program
{
    public static void Main()
    {
        var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        var consoleAppDir = currentDir.Parent!.Parent!.Parent;

        var filePath = Path.Combine(consoleAppDir!.FullName, "1.txt");
        using StreamReader reader = new(filePath);
        string text = reader.ReadToEnd();

        // bits указывает те позиции в тексте, в которых заглавные буквы стоят не по правилам
        var (lowerText, bits) = CapitalPositionEncoder.EncodeCapitalPositions(text);

        var result = RleDeltaEncoder.Encode(bits.ToArray());

        // Получилось почти в 5 раз меньше, чем исходный битовый массив

        Console.WriteLine(result.Length); // 132522 bits
        Console.WriteLine(bits.Count); // 574980 bits

        // если бы кодировали позицию каждого исключения 20 битами, получили бы 9016*20 = 180320 bits
        Console.WriteLine(bits.Count(x => x)); // 9016 исключений
        Console.WriteLine(text.Length); // длина текста 574980 -> каждую позицию в тексте можно кодировать 20 битами
    }

    // Этот метод был нужен, чтобы понять, каких длин больше встречается в rle-коде.
    // По результатам запуска, больше чисел в диапазоне > 32, => выгоднее использовать дельта-кодирование.
    private static void ChooseBetweenGammaAndDeltaEncodings(List<(int length, bool value)> rle)
    {
        var lensLessOrEqualTo31 = 0;
        var lensMoreThan31 = 0;

        foreach (var (len, val) in rle)
        {
            if (len == 1 || len is >= 16 and <= 31 || len is >= 4 and <= 7)
                continue;
            if (len < 32)
            {
                lensLessOrEqualTo31++;
            }
            else
            {
                lensMoreThan31++;
            }
        }

        Console.WriteLine(lensLessOrEqualTo31);
        Console.WriteLine(lensMoreThan31);
    }
}