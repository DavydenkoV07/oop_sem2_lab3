using System.Collections.Generic;

namespace WordAnalyzer.Core
{
    /// <summary>
    /// Interface for word frequency counting algorithms. (Strategy pattern)
    /// </summary>
    public interface IWordFrequencyAnalyzer
    {
        /// <summary>
        /// Analyzes a list of files and counts the frequency of each word.
        /// </summary>
        /// <param name="filePaths">A collection of paths to text files.</param>
        /// <returns>A dictionary where the key is a word and the value is the number of repetitions.</returns>
        IDictionary<string, int> Analyze(IEnumerable<string> filePaths);
    }
}