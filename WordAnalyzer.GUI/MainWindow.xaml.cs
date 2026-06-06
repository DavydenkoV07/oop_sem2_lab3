using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WordAnalyzer.Core; // Підключаємо нашу логіку

namespace WordAnalyzer.GUI
{
    public partial class MainWindow : Window
    {
        private string[] _selectedFiles;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Обробник вибору файлів
        private void BtnSelectFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Оберіть файли для аналізу"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFiles = openFileDialog.FileNames;
                TxtSelectedFiles.Text = $"Обрано файлів: {_selectedFiles.Length} шт.";
            }
        }

        // Обробник кнопки СТАРТ
        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFiles == null || _selectedFiles.Length == 0)
            {
                MessageBox.Show("Будь ласка, спочатку оберіть текстові файли!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Блокуємо кнопку на час виконання, щоб не запустили двічі
            BtnStart.IsEnabled = false;
            TxtResults.Text = "Аналіз тексту... Будь ласка, зачекайте.";
            TxtTime.Text = "Час виконання: рахуємо...";

            // Використовуємо патерн Strategy: інтерфейс один, а реалізацію беремо залежно від обраної кнопки
            IWordFrequencyAnalyzer analyzer;
            if (RbParallel.IsChecked == true)
            {
                analyzer = new ParallelAnalyzer();
            }
            else
            {
                analyzer = new SequentialAnalyzer();
            }

            // Таймер для заміру часу (вимога лаби)
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // ВАЖЛИВО: Виконуємо важкий процес у фоновому потоці, щоб не заморозити GUI
            IDictionary<string, int> results = null;
            await Task.Run(() =>
            {
                results = analyzer.Analyze(_selectedFiles);
            });

            stopwatch.Stop();
            TxtTime.Text = $"Час виконання: {stopwatch.ElapsedMilliseconds} мс";

            // Сортуємо словник і беремо топ 20 слів
            var topWords = results
                .OrderByDescending(kv => kv.Value)
                .Take(20)
                .Select(kv => $"{kv.Key} - {kv.Value} разів");

            // Виводимо результат на екран
            TxtResults.Text = string.Join(Environment.NewLine, topWords);

            // Розблоковуємо кнопку
            BtnStart.IsEnabled = true;
        }
    }
}