using LifeCounter.Behavior;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                string playerNumber = $"Player_{i + 1}";
                File.WriteAllText(playerNumber + "_Life.txt", "40");
                for (int j = 0; j < 4; j++)
                {
                    File.WriteAllText($"{playerNumber}_cmdrDmg_{j + 1}.txt", "0");
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
            string playerNumber = GetPlayerNumber(sender);
            int total = (sender as LifeButton).LifeTotal;
            if (sender is CommanderDamage cmdr)
            {
                var all = (cmdr.Parent as Grid).Children.OfType<CommanderDamage>().ToArray();
                int cmdrNumber = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (all[i] == sender)
                        cmdrNumber = i + 1;
                }
                File.WriteAllText($"{playerNumber}_cmdrDmg_{cmdrNumber}.txt", total.ToString());

            }
            else
            {
                File.WriteAllText(playerNumber + "_Life.txt", total.ToString());
            }
        }


        private void playerNameChanged(object sender, TextChangedEventArgs e)
        {
            string playerNumber = GetPlayerNumber(sender);
            File.WriteAllText(playerNumber + "_Name.txt", (sender as TextBox).Text);
            
        }
    }
}
