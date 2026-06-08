using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WordAnalyzer.Core
{
    /// <summary>
    /// Parallel (multi-threaded) word frequency analyzer.
    /// Uses TPL (Task Parallel Library) to distribute the load across processor cores.
    /// </summary>
    public class ParallelAnalyzer : IWordFrequencyAnalyzer
    {
        public IDictionary<string, int> Analyze(IEnumerable<string> filePaths)
        {
            
            var wordCounts = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            
            char[] separators = { ' ', '.', ',', ';', ':', '!', '?', '\n', '\r', '\t', '—', '-', '\"', '\'' };

            
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