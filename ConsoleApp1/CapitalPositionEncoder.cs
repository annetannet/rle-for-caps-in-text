using System.Collections.Generic;

namespace ConsoleApp1;

public static class CapitalPositionEncoder
{
    public static (string, List<bool>) EncodeCapitalPositions(string text)
    {
        // Правила капитализации:
        // 1) Первая буква текста
        // 2) Буква после ". "
        // 3) Буква после двух заглавных подряд
        var expectedCaps = new HashSet<int> { 0 }; // Первая буква всегда заглавная

        // Определяем ожидаемые заглавные буквы по правилам
        for (int i = 1; i < text.Length; i++)
        {
            // Проверка на ". "
            if (i >= 2 && text.Substring(i - 2, 2) == ". ")
            {
                expectedCaps.Add(i);
            }

            // Проверка на две заглавные подряд
            if (i >= 2 && char.IsUpper(text[i - 2]) && char.IsUpper(text[i - 1]))
            {
                expectedCaps.Add(i);
            }
        }

        // Создаем битовый массив
        var bits = new List<bool>();

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            bool isExpectedUpper = expectedCaps.Contains(i);

            if (char.IsUpper(c) && !isExpectedUpper)
            {
                // Лишняя заглавная буква
                bits.Add(true);
            }
            else
            {
                // Строчная буква или ожидаемая заглавная
                bits.Add(false);
            }
        }

        return (text.ToLower(), bits);
    }
}