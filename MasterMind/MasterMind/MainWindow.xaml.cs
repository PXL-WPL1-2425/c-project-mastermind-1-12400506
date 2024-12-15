using System;
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
        private int maxAttemps = 10;
        private DispatcherTimer timer;
        private bool isDebugMode = false;
        private string[] highscores = new string[15];
        private int highscoreCount = 0;
        private List<string> playerNames = new List<string>();
        private int currentPlayerIndex = 0;
        private int totalPenaltyPoints = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitialiseerComboBoxes();
            GenerateSecretCode();
            InitializeTimer();
            UpdateDebugTextBox();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Weet je zeker dat je het spel wilt afsluiten?",
                "Bevestiging",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void MnuNieuwSpel_Click(object sender, RoutedEventArgs e)
        {
            if (playerNames == null || playerNames.Count == 0)
            {
                AskForPlayerNames();
            }

            ResetGame();
        }

            private void MnuHighscores_Click(object sender, RoutedEventArgs e)
        {
            var validHighscores = highscores.Where(h => !string.IsNullOrEmpty(h)).ToArray();

            if (validHighscores.Length == 0)
            {
                MessageBox.Show("Er zijn nog geen highscores.", "Highscores", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string highscoreList = string.Join("\n", validHighscores.Select((entry, index) => $"{index + 1}. {entry}"));
            MessageBox.Show(highscoreList, "Highscores", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MnuAfsluiten_Click(object sender, RoutedEventArgs e)
        {
            Close(); 
        }

        private void MnuAantalPogingen_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Voer het maximaal aantal pogingen in (3-20):",
                "Aantal Pogingen Instellen",
                maxAttemps.ToString() 
            );

            if (int.TryParse(input, out int newMaxAttempts) && newMaxAttempts >= 3 && newMaxAttempts <= 20)
            {
                maxAttemps = newMaxAttempts;
                MessageBox.Show($"Het maximaal aantal pogingen is ingesteld op {maxAttemps}.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ongeldige invoer! Het aantal pogingen blijft op " + maxAttemps + ".", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MnuHintCorrectColor_Click(object sender, RoutedEventArgs e)
        {
            if (attempts >= maxAttemps)
            {
                MessageBox.Show("Je hebt geen pogingen meer over. Je kunt geen hints meer kopen.", "Geen hints beschikbaar", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Random rand = new Random();
            string hintColor = code[rand.Next(code.Length)];

            MessageBox.Show($"Een van de kleuren in de geheime code is: {hintColor}", "Hint", MessageBoxButton.OK, MessageBoxImage.Information);

            UpdateScore(15); 
        }

        private void MnuHintCorrectPosition_Click(object sender, RoutedEventArgs e)
        {
            if (attempts >= maxAttemps)
            {
                MessageBox.Show("Je hebt geen pogingen meer over. Je kunt geen hints meer kopen.", "Geen hints beschikbaar", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Random rand = new Random();
            int position = rand.Next(code.Length);

            MessageBox.Show($"De kleur op positie {position + 1} is: {code[position]}", "Hint", MessageBoxButton.OK, MessageBoxImage.Information);

            UpdateScore(25); 
        }

        private void AskForPlayerNames()
        {
            playerNames.Clear(); 

            bool addMorePlayers = true;

            while (addMorePlayers)
            {
                string playerName = Microsoft.VisualBasic.Interaction.InputBox(
                    "Voer de naam van de speler in:",
                    "Speler toevoegen",
                    "");

                if (string.IsNullOrWhiteSpace(playerName))
                {
                    MessageBox.Show("Naam mag niet leeg zijn. Probeer opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    continue;
                }

                playerNames.Add(playerName);

                MessageBoxResult result = MessageBox.Show(
                    "Wil je nog een speler toevoegen?",
                    "Speler toevoegen",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                addMorePlayers = (result == MessageBoxResult.Yes);
            }
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
                    ToolTip = "witte rand: Juiste kleur, foute positie\n rode rand: Juiste kleur, juiste positie\n geen kleur: Foute kleur",
                    StrokeThickness = 5
                };
                attemptPanel.Children.Add(colorBox);
            }


            historyPanel.Children.Add(attemptPanel);
        }
        private void UpdateScore(int penaltyPoints)
        {
            totalPenaltyPoints += penaltyPoints; 
            int totalScore = 100 - totalPenaltyPoints; 
            if (totalScore < 0) totalScore = 0; 

            scoreLabel.Content = $"Score: {totalScore} (Strafpunten: {totalPenaltyPoints})";
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
        private void AddHighscore(string playerName, int attempts, int score)
        {
            string highscoreEntry = $"{playerName} - [{attempts} pogingen] - [score: {score}/100]";

            if (highscoreCount < highscores.Length)
            {
                highscores[highscoreCount] = highscoreEntry;
                highscoreCount++;
            }
            else
            {
                highscores[highscores.Length - 1] = highscoreEntry;
            }

            highscores = highscores
                .Where(h => !string.IsNullOrEmpty(h)) 
                .OrderByDescending(h => int.Parse(h.Split(':')[1].Split('/')[0])) 
                .Take(15) 
                .ToArray();
        }

        private void EndGame(bool isWinner)
        {
            string currentPlayer = playerNames[currentPlayerIndex];
            string message = isWinner
                ? $"Gefeliciteerd, {currentPlayer}! Je hebt de code gekraakt in {attempts} pogingen!"
                : $"Helaas, {currentPlayer}, je hebt de code niet gekraakt. De geheime code was: {string.Join(", ", code)}.";

            int totalScore = 100 - (attempts * 10);
            if (totalScore < 0) totalScore = 0;

            if (isWinner)
            {
                AddHighscore(currentPlayer, attempts, totalScore);
            }

            MessageBox.Show(message, "Spel Einde", MessageBoxButton.OK, MessageBoxImage.Information);

            currentPlayerIndex = (currentPlayerIndex + 1) % playerNames.Count;

            ResetGame();
        }

        private void ResetGame()
        {
            if (playerNames.Count == 0)
            {
                MessageBox.Show("Geen spelers beschikbaar. Voeg spelers toe om het spel te starten.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            string currentPlayer = playerNames[currentPlayerIndex];
            MessageBox.Show($"Het is nu de beurt van {currentPlayer}. Veel succes!", "Nieuwe Beurt", MessageBoxButton.OK, MessageBoxImage.Information);


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
    if (playerNames == null || playerNames.Count == 0)
    {
        this.Title = "Mastermind - Geen spelers - Poging {attempts}/{maxAttemps}";
        return; 
    }
    if (currentPlayerIndex < 0 || currentPlayerIndex >= playerNames.Count)
    {
        currentPlayerIndex = 0; 
    }

    if (playerNames.Count == 1)
    {
        this.Title = $"{playerNames[0]}'s Mastermind - Poging {attempts}/{maxAttemps}";
    }
    else
    {
        this.Title = $"Mastermind - Speler: {playerNames[currentPlayerIndex]} - Poging {attempts}/{maxAttemps}";
    }
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