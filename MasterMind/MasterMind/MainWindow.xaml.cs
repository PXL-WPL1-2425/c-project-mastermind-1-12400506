using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            generateSecretCode();
        }

        private void InitialiseerComboBoxes()
        {
            comboBox1.ItemsSource = allColors;
            comboBox2.ItemsSource = allColors;
            comboBox3.ItemsSource = allColors;
            comboBox4.ItemsSource = allColors;
        }


        private void generateSecretCode()
        {
            Random number = new Random();
            code = Enumerable.Range(0, 4)
                                    .Select(_ => allColors[number.Next(allColors.Length)])
                                    .ToArray();

            // Toon de geheime code in het venstertitel (voor testen)
            this.Title = $"secret Code: {string.Join(", ", code)}";
        }

}
}