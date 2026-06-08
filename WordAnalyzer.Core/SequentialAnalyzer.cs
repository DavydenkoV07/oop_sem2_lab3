using System;
using System.Collections.Generic;
using System.IO;

namespace WordAnalyzer.Core
{
    /// <summary>
    /// Sequential (single-threaded) word frequency analyzer.
    /// </summary>
    public class SequentialAnalyzer : IWordFrequencyAnalyzer
    {
        public IDictionary<string, int> Analyze(IEnumerable<string> filePaths)
        {
            // Звичайний словник для одного потоку
            var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            
            
            char[] separators = { ' ', '.', ',', ';', ':', '!', '?', '\n', '\r', '\t', '—', '-', '\"', '\'' };

            foreach (var path in filePaths)
            {
                // Читаємо файл по рядках
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