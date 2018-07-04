using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace LifeCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WebServer srv;

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
            srv = new WebServer(HandleHttp, "http://+:5000/");
            srv.Run();
        }

        private string HandleHttp(HttpListenerRequest request, HttpListenerResponse response)
        {
            switch (request.Url.AbsolutePath)
            {
                case "/":
                    if (File.Exists("html/index.html"))
                        return File.ReadAllText("html/index.html");
                    response.Redirect($"http://mtglt.redpoint.games/?url={Uri.EscapeUriString(request.UserHostAddress)}");
                    return null;
                case "/totals.json":
                    break;
                case "/names.json":
                    return JsonConvert.SerializeObject(new {
                        player1 = Player_1.GetPlayerName(),
                        player2 = Player_2.GetPlayerName(),
                        player3 = Player_3.GetPlayerName(),
                        player4 = Player_4.GetPlayerName(),
                    });
                case "/set":
                    if (request.HasEntityBody)
                    {
                        var data = JsonConvert.DeserializeObject<SetRequest>(request.GetRequestPostData());
                        if (data.player == 0)
                        {
                            return JsonError("Missing key `player`");
                        }
                        var grid = GetPlayerGrids().ElementAt(data.player - 1);
                        if (!data.commander.HasValue || data.commander == 0)
                        {
                            Dispatcher.Invoke(() => grid.GetLifeButton().LifeTotal += data.change);
                            break;
                        }
                    }
                    else
                    {
                        return JsonError("POST json not found.");
                    }
                    break;
                case "/l1":
                    return Player_1.GetLifeButton().LifeTotal.ToString();
                case "/n1":
                    return Player_1.GetPlayerName();
                case "/l2":
                    return Player_2.GetLifeButton().LifeTotal.ToString();
                case "/n2":
                    return Player_2.GetPlayerName();
                case "/l3":
                    return Player_3.GetLifeButton().LifeTotal.ToString();
                case "/n3":
                    return Player_3.GetPlayerName();
                case "/l4":
                    return Player_4.GetLifeButton().LifeTotal.ToString();
                case "/n4":
                    return Player_4.GetPlayerName();
                case "/card.jpg":
                default:
                    if (File.Exists("html" + request.Url.AbsolutePath))
                    {
                        return File.ReadAllText("html" + request.Url.AbsolutePath);
                    }
                    return JsonError("Wat");
            }
            return JsonConvert.SerializeObject(new
            {
                player1 = Player_1.GetLifeButton().LifeTotal,
                player2 = Player_2.GetLifeButton().LifeTotal,
                player3 = Player_3.GetLifeButton().LifeTotal,
                player4 = Player_4.GetLifeButton().LifeTotal
            });
        }

        private static string JsonError(string error)
        {
            return JsonConvert.SerializeObject(new
            {
                error
            });
        }

        IEnumerable<Grid> GetPlayerGrids()
        {
            if (!Dispatcher.CheckAccess())
            {
                var task = Dispatcher.InvokeAsync(GetPlayerGrids);
                task.Wait();
                return task.Result;
            }
            return RootGrid.Children.OfType<Grid>().ToArray();
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

        private void PlayerNameChanged(object sender, TextChangedEventArgs e)
        {
            var playerNumber = GetPlayerNumber(sender);
            var textBox = (sender as TextBox);
            var label = textBox.Tag as string;
            if (string.IsNullOrEmpty(label))
                return;
            File.WriteAllText($"{playerNumber}_{label}.txt", textBox.Text);
        }

        private async void TextBox1_KeyDownAsync(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
