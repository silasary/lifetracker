using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LifeCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 4; i++)
            {
                var prefix = $"Player_{i + 1}";
                File.WriteAllText(prefix + "_Life.txt", "40");
                for (int j = 0; j < 4; j++)
                {
                    File.WriteAllText($"{prefix}_cmdrDmg_{j + 1}.txt", "0");
                }
            }
        }

        private static string GetPlayerNumber(object sender)
        {
            var Parent = (sender as FrameworkElement);
            while (!Parent.Name.StartsWith("Player_"))
            {
                Parent = Parent.Parent as FrameworkElement;
            }
            return Parent.Name;
        }

        private void LifeButton_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var prefix = GetPlayerNumber(sender);
            var total = (sender as LifeButton).LifeTotal;
            if (sender is CommanderDamage cmdr)
            {
                var all = (cmdr.Parent as Grid).Children.OfType<CommanderDamage>().ToArray();
                var cmdrNumber = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (all[i] == sender)
                        cmdrNumber = i + 1;
                }
                File.WriteAllText($"{prefix}_cmdrDmg_{cmdrNumber}.txt", total.ToString());

            }
            else
            {
                File.WriteAllText(prefix + "_Life.txt", total.ToString());
            }
        }


        private void playerNameChanged(object sender, TextChangedEventArgs e)
        {
            var playerNumber = GetPlayerNumber(sender);
            var textBox = (sender as TextBox);
            var label = textBox.Tag as string;
            if (string.IsNullOrEmpty(label))
                return;
            File.WriteAllText($"{playerNumber}_{label}.txt", textBox.Text);
        }

        private async void textBox1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            const string FILENAME = "card.jpg";
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var textBox = (sender as TextBox);
                var image = await Scryfall.Scryfall.GetCardImageAsync(textBox.Text);
                if (image == null)
                {
                    if (File.Exists(FILENAME))
                        File.Delete(FILENAME);
                }
                else
                {
                    File.Copy(image, FILENAME, true);
                }
            }
        }
    }
}
