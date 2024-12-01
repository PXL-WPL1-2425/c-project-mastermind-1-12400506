﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Shapes;

namespace MasterMind
{
    public partial class MainWindow : Window
    {
        private string[] allColors = { "Rood", "Geel", "Oranje", "Wit", "Groen", "Blauw" };
        private string[] code;
        private int attempts = 0;
        private const int maxAttemps = 10;
        private DispatcherTimer timer;
        private bool isDebugMode = false;

        public MainWindow()
        {
            InitializeComponent();
            InitialiseerComboBoxes();
            GenerateSecretCode();
            InitializeTimer();
            UpdateDebugTextBox();
        }

        private void InitialiseerComboBoxes()
        {
            comboBox1.ItemsSource = allColors;
            comboBox2.ItemsSource = allColors;
            comboBox3.ItemsSource = allColors;
            comboBox4.ItemsSource = allColors;
        }

        private void GenerateSecretCode()
        {
            Random number = new Random();
            code = Enumerable.Range(0, 4)
                             .Select(_ => allColors[number.Next(allColors.Length)])
                             .ToArray();

            attempts = 0;
            UpdateTitle();
            ResetLabelBorders();
            this.Title = $"Secret Code: {string.Join(", ", code)}"; 
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == comboBox1)
                UpdateLabelColor(label1, comboBox1.SelectedItem?.ToString());
            else if (sender == comboBox2)
                UpdateLabelColor(label2, comboBox2.SelectedItem?.ToString());
            else if (sender == comboBox3)
                UpdateLabelColor(label3, comboBox3.SelectedItem?.ToString());
            else if (sender == comboBox4)
                UpdateLabelColor(label4, comboBox4.SelectedItem?.ToString());
        }

        private void UpdateLabelColor(Label label, string colorName)
        {
            if (colorName == null)
            {
                label.Background = Brushes.Transparent;
                return;
            }

            switch (colorName.ToLower())
            {
                case "rood":
                    label.Background = Brushes.Red;
                    break;
                case "geel":
                    label.Background = Brushes.Yellow;
                    break;
                case "oranje":
                    label.Background = Brushes.Orange;
                    break;
                case "wit":
                    label.Background = Brushes.White;
                    break;
                case "groen":
                    label.Background = Brushes.Green;
                    break;
                case "blauw":
                    label.Background = Brushes.Blue;
                    break;
                default:
                    label.Background = Brushes.Transparent;
                    break;
            }
        }

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            string[] selectedColors = {
        comboBox1.SelectedItem?.ToString(),
        comboBox2.SelectedItem?.ToString(),
        comboBox3.SelectedItem?.ToString(),
        comboBox4.SelectedItem?.ToString()
    };

            if (selectedColors.Any(c => c == null))
            {
                MessageBox.Show("Vul alle kleuren in voordat je de code controleert.", "Waarschuwing", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ResetLabelBorders();
            AddAttemptToHistory(selectedColors);
            attempts++;
            UpdateTitle();
            int attemptScore = CalculateScore(selectedColors);
            UpdateScore(attemptScore);

            if (IsCodeCracked(selectedColors))
            {
                EndGame(true);
            }
            else if (attempts >= maxAttemps)
            {
                EndGame(false);
            }
        }

        private void AddAttemptToHistory(string[] selectedColors)
        {
            StackPanel attemptPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(2)
            };

            for (int i = 0; i < selectedColors.Length; i++)
            {
                Rectangle colorBox = new Rectangle
                {
                    Width = 180,
                    Height = 20,
                    Margin = new Thickness(2),
                    Fill = GetBrushFromColorName(selectedColors[i]),
                    Stroke = GetFeedbackBorder(selectedColors[i], i),
                    StrokeThickness = 5
                };
                attemptPanel.Children.Add(colorBox);
            }


            historyPanel.Children.Add(attemptPanel);
        }
        private void UpdateScore(int penaltyPoints)
        {
            int totalScore = 100 - (attempts * penaltyPoints); 
            if (totalScore < 0) totalScore = 0; 
            scoreLabel.Content = $"Score: {totalScore} (Strafpunten: {penaltyPoints})";
        }

        private int CalculateScore(string[] selectedColors)
        {
            int totalPenalty = 0;

            for (int i = 0; i < selectedColors.Length; i++)
            {
                if (selectedColors[i] == code[i])
                {
                    continue;
                }
                else if (code.Contains(selectedColors[i]))
                {
                    totalPenalty += 1;
                }
                else
                {
                    totalPenalty += 2;
                }
            }

            return totalPenalty; 
        }


        private Brush GetFeedbackBorder(string color, int index)
        {
            if (color == code[index])
            {
                return Brushes.DarkRed; 
            }
            else if (code.Contains(color))
            {
                return Brushes.Wheat; 
            }
            else
            {
                return Brushes.Black; 
            }
        }

        private Brush GetBrushFromColorName(string colorName)
        {
            return colorName.ToLower() switch
            {
                "rood" => Brushes.Red,
                "geel" => Brushes.Yellow,
                "oranje" => Brushes.Orange,
                "wit" => Brushes.White,
                "groen" => Brushes.Green,
                "blauw" => Brushes.Blue,
                _ => Brushes.Transparent
            };
        }

        private bool IsCodeCracked(string[] selectedColors)
        {
            return selectedColors.SequenceEqual(code);
        }

        private void EndGame(bool isWinner)
        {
            string message = isWinner
                ? $"Gefeliciteerd! Je hebt de code gekraakt in {attempts} pogingen! Wil je opnieuw spelen?"
                : $"Helaas, je hebt de code niet gekraakt. De geheime code was: {string.Join(", ", code)}. Wil je opnieuw spelen?";

            MessageBoxResult result = MessageBox.Show(message, "Spel Einde", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ResetGame(); // Reset het spel als de speler opnieuw wil spelen
            }
            else
            {
                Application.Current.Shutdown(); // Sluit de applicatie als de speler niet opnieuw wil spelen
            }
        }

        private void ResetGame()
        {
            attempts = 0;
            historyPanel.Children.Clear();
            scoreLabel.Content = "Score: 100";
            GenerateSecretCode();
            ResetLabelBorders();
            UpdateTitle();
        }

        private void ResetLabelBorders()
        {
            label1.BorderBrush = Brushes.Black;
            label2.BorderBrush = Brushes.Black;
            label3.BorderBrush = Brushes.Black;
            label4.BorderBrush = Brushes.Black;
        }
        private void UpdateTitle()
        {
            this.Title = $"Poging {attempts}/{maxAttemps}";
        }

        private Label GetLabelByIndex(int index)
        {
            return index switch
            {
                0 => label1,
                1 => label2,
                2 => label3,
                3 => label4,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
        }
        private void InitializeTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            timer.Tick += Timer_Tick;
        }

        private void StartCountdown()
        {
            timer.Stop();
            timer.Start();
        }
        private void StopCountdown()
        {
            timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            MessageBox.Show("Je hebt te lang gewacht! Je beurt is voorbij.", "Tijd verstreken", MessageBoxButton.OK, MessageBoxImage.Warning);
            attempts++;
            UpdateTitle();
        }

        private void UpdateDebugTextBox()
        {
            if (isDebugMode)
            {
                debugTextBox.Text = $"geheime code:{string.Join(", ", code)}";
                debugTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                debugTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void DebugShortcut(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12 && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ToggleDebug();
            }
        }
        private void ToggleDebug()
        {
            isDebugMode = !isDebugMode;
            UpdateDebugTextBox();
        }

    }
}