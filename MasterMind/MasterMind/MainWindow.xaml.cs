using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MasterMind
{
    public partial class MainWindow : Window
    {
        private string[] allColors = { "Rood", "Geel", "Oranje", "Wit", "Groen", "Blauw" };
        private string[] code;

        public MainWindow()
        {
            InitializeComponent();
            InitialiseerComboBoxes();
            GenerateSecretCode();
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

            this.Title = $"Secret Code: {string.Join(", ", code)}"; // Alleen voor debug/testdoeleinden
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

            ResetLabelBorders();

            for (int i = 0; i < selectedColors.Length; i++)
            {
                if (selectedColors[i] == code[i])
                {
                    GetLabelByIndex(i).BorderBrush = Brushes.DarkRed;
                }
                else if (code.Contains(selectedColors[i]))
                {
                    GetLabelByIndex(i).BorderBrush = Brushes.Wheat;
                }
            }
        }

        private void ResetLabelBorders()
        {
            label1.BorderBrush = Brushes.Black;
            label2.BorderBrush = Brushes.Black;
            label3.BorderBrush = Brushes.Black;
            label4.BorderBrush = Brushes.Black;
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
    }
}