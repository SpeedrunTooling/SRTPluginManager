using SRTPluginManager.TrainerHelper;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SRTPluginManager.MVVM.View
{
    public partial class TrainerView : UserControl
    {
        private List<GameIndexEntry> gameIndex;
        private MemoryAccess memoryAccess = new MemoryAccess();
        private string currentProcessName;

        public TrainerView()
        {
            InitializeComponent();
            LoadGameIndex();
        }

        private async void LoadGameIndex()
        {
            string indexPath = "https://raw.githubusercontent.com/SuperTrainerM/DMC3SETrainer/refs/heads/main/Games-Trainer.json";

            try
            {
                gameIndex = await ConfigManager.LoadGameIndex(indexPath);

                if (gameIndex == null || gameIndex.Count == 0)
                {
                    MessageBox.Show("No games found in the index.");
                    return;
                }

                GameListBox.Items.Clear();
                foreach (var entry in gameIndex)
                {
                    GameListBox.Items.Add(entry.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading game index: {ex.Message}");
            }
        }

        private async void GameListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GameListBox.SelectedItem == null)
                return;

            string selectedGameName = GameListBox.SelectedItem.ToString();
            var selectedIndexEntry = gameIndex?.Find(g => g.Name == selectedGameName);

            if (selectedIndexEntry != null)
            {
                try
                {
                    var gameConfig = await ConfigManager.LoadGameConfig(selectedIndexEntry.URL);

                    // Update the creator display
                    TrainerCreator.Text = $"Created by: {selectedIndexEntry.Creator}";

                    // Attempt to attach to the new process
                    if (memoryAccess.AttachToProcess(gameConfig.ProcessName))
                    {
                        currentProcessName = gameConfig.ProcessName;
                        ConnectionStatus.Text = $"Connected to {currentProcessName}";
                        ConnectionStatus.Foreground = System.Windows.Media.Brushes.Green;
                    }
                    else
                    {
                        currentProcessName = null;
                        ConnectionStatus.Text = "Not Connected";
                        ConnectionStatus.Foreground = System.Windows.Media.Brushes.Red;
                    }

                    // Always update trainer options
                    DisplayTrainerOptions(gameConfig);
                }
                catch (Exception ex)
                {
                    currentProcessName = null;
                    ConnectionStatus.Text = "Not Connected";
                    ConnectionStatus.Foreground = System.Windows.Media.Brushes.Red;
                    MessageBox.Show($"Error loading game configuration: {ex.Message}");
                }
            }
        }

        private void DisplayTrainerOptions(GameConfig game)
        {
            TrainerOptionsPanel.Children.Clear();

            foreach (var option in game.Options)
            {
                var panel = new Border
                {
                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(29, 28, 46)),
                    CornerRadius = new CornerRadius(8),
                    Margin = new Thickness(0, 10, 0, 10),
                    Padding = new Thickness(10)
                };

                var optionStack = new StackPanel();

                optionStack.Children.Add(new TextBlock
                {
                    Text = option.Name,
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.White,
                    Margin = new Thickness(0, 0, 0, 5)
                });

                optionStack.Children.Add(new TextBlock
                {
                    Text = option.Description,
                    FontSize = 16,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                var inputField = new TextBox
                {
                    Width = 250,
                    FontSize = 14,
                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(33, 32, 56)),
                    Foreground = System.Windows.Media.Brushes.White,
                    BorderBrush = System.Windows.Media.Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, 0, 0, 10),
                    Padding = new Thickness(5)
                };
                optionStack.Children.Add(inputField);

                var setButton = new Button
                {
                    Content = "Set",
                    Width = 100,
                    Height = 35,
                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(58, 58, 58)),
                    Foreground = System.Windows.Media.Brushes.White,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 5, 0, 0)
                };

                setButton.Click += async (s, e) =>
                {
                    if (short.TryParse(inputField.Text, out short value))
                    {
                        IntPtr finalAddress = GetFinalAddress(game.ProcessName, Convert.ToInt32(option.Address, 16));
                        if (finalAddress == IntPtr.Zero)
                        {
                            UpdateOperationStatus("Failed to calculate the memory address.", false);
                            return;
                        }

                        try
                        {
                            byte[] bytes = BitConverter.GetBytes(value);
                            memoryAccess.WriteMemoryBytes(finalAddress, bytes);

                            UpdateOperationStatus($"Successfully set {option.Name} to {value}.", true);
                        }
                        catch (Exception ex)
                        {
                            UpdateOperationStatus($"Error: {ex.Message}", false);
                        }
                    }
                    else
                    {
                        UpdateOperationStatus("Invalid input.", false);
                    }
                };



                optionStack.Children.Add(setButton);
                panel.Child = optionStack;
                TrainerOptionsPanel.Children.Add(panel);
            }
        }

        private async void UpdateOperationStatus(string message, bool isSuccess)
        {
            OperationStatus.Text = message;
            OperationStatus.Foreground = isSuccess ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
            await Task.Delay(5000);
            OperationStatus.Text = "";
            OperationStatus.Foreground = System.Windows.Media.Brushes.Gray;
        }


        private IntPtr GetFinalAddress(string processName, int offset)
        {
            var process = Process.GetProcessesByName(processName).FirstOrDefault();
            if (process == null)
            {
                MessageBox.Show($"Process {processName} not found.", "Error");
                return IntPtr.Zero;
            }

            IntPtr baseAddress = process.MainModule.BaseAddress;
            IntPtr finalAddress = IntPtr.Add(baseAddress, offset);

            return finalAddress;
        }
    }
}
