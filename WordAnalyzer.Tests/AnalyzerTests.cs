using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using WordAnalyzer.Core;

namespace WordAnalyzer.Tests
{
    [TestFixture]
    public class AnalyzerTests
    {
        private string _testFilePath;

        // Цей метод виконується перед кожним тестом. 
        // Він створює тимчасовий файл із тестовим текстом.
        [SetUp]
        public void Setup()
        {
            _testFilePath = Path.GetTempFileName();
            string testText = "Привіт світ! Світ програмування — це круто. Привіт усім, привіт світ.";
            File.WriteAllText(_testFilePath, testText);
        }

        // Цей метод виконується після тесту. Він прибирає за собою (видаляє тимчасовий файл).
        [TearDown]
        public void Teardown()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [Test]
        public void Analyzers_ShouldReturnIdenticalResults()
        {
            // Arrange (Підготовка)
            var sequentialAnalyzer = new SequentialAnalyzer();
            var parallelAnalyzer = new ParallelAnalyzer();
            var filePaths = new List<string> { _testFilePath };

            // Act (Виконання)
            var seqResult = sequentialAnalyzer.Analyze(filePaths);
            var parResult = parallelAnalyzer.Analyze(filePaths);

            // Assert (Перевірка результатів)
            
            // 1. Перевіряємо, чи однакова кількість унікальних слів знайдена
            Assert.AreEqual(seqResult.Count, parResult.Count, "Кількість знайдених унікальних слів відрізняється!");

            // 2. Перевіряємо, чи частота кожного слова збігається в обох словниках
            foreach (var kvp in seqResult)
            {
                string word = kvp.Key;
                int expectedCount = kvp.Value;

                // Перевіряємо, чи є таке слово в паралельному словнику
                Assert.IsTrue(parResult.ContainsKey(word), $"Паралельний алгоритм загубив слово: '{word}'");
                
                // Перевіряємо, чи збігається кількість повторень
                Assert.AreEqual(expectedCount, parResult[word], $"Кількість повторень для слова '{word}' не збігається!");
            }
        }

        [Test]
        public void Analyzers_EmptyFile_ReturnsEmptyDictionary()
        {
            // Перезаписуємо тестовий файл, роблячи його абсолютно порожнім
            File.WriteAllText(_testFilePath, ""); 
            var filePaths = new List<string> { _testFilePath };

            var seqResult = new SequentialAnalyzer().Analyze(filePaths);
            var parResult = new ParallelAnalyzer().Analyze(filePaths);

            // Словники мають бути порожніми, програма не повинна впасти
            Assert.IsEmpty(seqResult, "Послідовний аналізатор не повернув порожній словник для порожнього файлу");
            Assert.IsEmpty(parResult, "Паралельний аналізатор не повернув порожній словник для порожнього файлу");
        }

        [Test]
        public void Analyzers_CaseInsensitivity_CountsWordsCorrectly()
        {
            // Перевіряємо, чи програма розуміє, що це одне й те саме слово
            File.WriteAllText(_testFilePath, "Слово слово СЛОВО СлОвО");
            var filePaths = new List<string> { _testFilePath };

            var seqResult = new SequentialAnalyzer().Analyze(filePaths);
            var parResult = new ParallelAnalyzer().Analyze(filePaths);

            // Має бути знайдено рівно 1 унікальне слово, яке повторюється 4 рази
            Assert.AreEqual(1, seqResult.Count, "Алгоритм не розпізнав слова з різним регістром як одне однакове");
            Assert.AreEqual(4, seqResult["слово"], "Неправильний підрахунок для послідовного алгоритму");
            Assert.AreEqual(4, parResult["слово"], "Неправильний підрахунок для паралельного алгоритму");
        }

        [Test]
        public void Analyzers_Punctuation_IgnoresSeparators()
        {
            // Перевіряємо брудний текст із купою розділових знаків
            File.WriteAllText(_testFilePath, "Привіт, світ! Привіт... світ?");
            var filePaths = new List<string> { _testFilePath };

            var seqResult = new SequentialAnalyzer().Analyze(filePaths);
            var parResult = new ParallelAnalyzer().Analyze(filePaths);

            // Очікуємо по 2 слова без знаків пунктуації
            Assert.AreEqual(2, seqResult["привіт"]);
            Assert.AreEqual(2, parResult["світ"]);
            Assert.AreEqual(2, seqResult.Count, "Алгоритм неправильно обробив розділові знаки і створив зайві слова");
        }

        [Test]
        public void Analyzers_MultipleFiles_CombinesResults()
        {
            // Створюємо другий тимчасовий файл спеціально для цього тесту
            string secondPath = Path.GetTempFileName();
            try 
            {
                File.WriteAllText(_testFilePath, "кіт собака");
                File.WriteAllText(secondPath, "собака птах");
                
                var filePaths = new List<string> { _testFilePath, secondPath };

                var parResult = new ParallelAnalyzer().Analyze(filePaths);

                // "собака" має підсумуватися з обох файлів
                Assert.AreEqual(2, parResult["собака"]);
                Assert.AreEqual(1, parResult["кіт"]);
                Assert.AreEqual(1, parResult["птах"]);
                Assert.AreEqual(3, parResult.Count);
            }
            finally
            {
                // Обов'язково видаляємо другий файл після тесту
                if (File.Exists(secondPath))
                {
                    File.Delete(secondPath);
                }
            }
        }
    }
}