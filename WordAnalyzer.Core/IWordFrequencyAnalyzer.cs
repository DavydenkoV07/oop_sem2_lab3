using System.Collections.Generic;

namespace WordAnalyzer.Core
{
    /// <summary>
    /// Інтерфейс для алгоритмів підрахунку частоти слів. (Патерн Strategy)
    /// </summary>
    public interface IWordFrequencyAnalyzer
    {
        /// <summary>
        /// Аналізує список файлів та рахує частоту кожного слова.
        /// </summary>
        /// <param name="filePaths">Колекція шляхів до текстових файлів.</param>
        /// <returns>Словник, де ключ - слово, а значення - кількість повторень.</returns>
        IDictionary<string, int> Analyze(IEnumerable<string> filePaths);
    }
}