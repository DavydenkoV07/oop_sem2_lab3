using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WordAnalyzer.Core
{
    /// <summary>
    /// Паралельний (мультипотоковий) аналізатор частоти слів.
    /// Використовує TPL (Task Parallel Library) для розподілу навантаження на ядра процесора.
    /// </summary>
    public class ParallelAnalyzer : IWordFrequencyAnalyzer
    {
        public IDictionary<string, int> Analyze(IEnumerable<string> filePaths)
        {
            // ПОТОКОБЕЗПЕЧНИЙ словник. Звичайний Dictionary тут видасть помилку під час одночасного запису!
            var wordCounts = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            
            char[] separators = { ' ', '.', ',', ';', ':', '!', '?', '\n', '\r', '\t', '—', '-', '\"', '\'' };

            // Головна магія розпаралелювання: файли розкидаються по доступних ядрах процесора
            Parallel.ForEach(filePaths, path =>
            {
                foreach (var line in File.ReadLines(path))
                {
                    var words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words)
                    {
                        string cleanWord = word.ToLower();
                        
                        // Потокобезпечне додавання або оновлення лічильника
                        wordCounts.AddOrUpdate(cleanWord, 1, (key, oldValue) => oldValue + 1);
                    }
                }
            });

            return wordCounts;
        }
    }
}