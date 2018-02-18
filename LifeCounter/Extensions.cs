using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LifeCounter
{
    static class Extensions
    {
        public static string GetRequestPostData(this HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }
            using (System.IO.Stream body = request.InputStream) // here we have data
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static LifeButton GetLifeButton(this Grid playerGrid)
        {
            if (!playerGrid.Dispatcher.CheckAccess())
            {
                var task = playerGrid.Dispatcher.InvokeAsync(() => GetLifeButton(playerGrid));
                task.Wait();
                return task.Result;
            }
            return playerGrid.Children.OfType<LifeButton>().Single(b => !(b is CommanderDamage));
        }

        public static string GetPlayerName(this Grid playerGrid)
        {
            if (!playerGrid.Dispatcher.CheckAccess())
            {
                var task = playerGrid.Dispatcher.InvokeAsync(() => GetPlayerName(playerGrid));
                task.Wait();
                return task.Result;
            }
            return playerGrid.Children.OfType<TextBox>().First().Text;
        }
    }
}
