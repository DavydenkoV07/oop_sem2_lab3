using System;
using System.Collections.Generic;
using System.IO;

namespace WordAnalyzer.Core
{
    /// <summary>
    /// Послідовний (однопотоковий) аналізатор частоти слів.
    /// </summary>
    public class SequentialAnalyzer : IWordFrequencyAnalyzer
    {
        public IDictionary<string, int> Analyze(IEnumerable<string> filePaths)
        {
            // Звичайний словник для одного потоку. Ігноруємо регістр слів.
            var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            
            // Роздільники слів (пробіли, розділові знаки тощо)
            char[] separators = { ' ', '.', ',', ';', ':', '!', '?', '\n', '\r', '\t', '—', '-', '\"', '\'' };

            foreach (var path in filePaths)
            {
                // Читаємо файл по рядках, щоб не завантажувати гігантський файл у пам'ять цілком
                foreach (var line in File.ReadLines(path))
                {
                    var words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words)
                    {
                        string cleanWord = word.ToLower();
                        if (wordCounts.ContainsKey(cleanWord))
                        {
                            wordCounts[cleanWord]++;
                        }
                        else
                        {
                            wordCounts[cleanWord] = 1;
                        }
                    }
                }
            }

            return wordCounts;
        }
    }
}