using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Linq;

namespace LifeCounter
{
    public class WebServer : IDisposable
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, HttpListenerResponse, string> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerRequest, HttpListenerResponse, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException(nameof(prefixes));

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method ?? throw new ArgumentException(nameof(method));
            try
            {
                _listener.Start();
            }
            catch (HttpListenerException c)
            {
                var pre = prefixes.Single();
                Process.Start(new ProcessStartInfo("netsh", $"http add urlacl url={pre} user=everyone")
                {
                    Verb = "runas"
                });
            }
        }

        public WebServer(Func<HttpListenerRequest, HttpListenerResponse, string> method, params string[] prefixes)
            : this(prefixes, method) { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            ctx.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                            if (ctx.Request.HttpMethod == "OPTIONS")
                            {
                                ctx.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
                                ctx.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
                                ctx.Response.AddHeader("Access-Control-Max-Age", "1728000");
                            }
                            try
                            {
                                var rstr = _responderMethod(ctx.Request, ctx.Response);
                                if (rstr == null)
                                    return;
                                var buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch (Exception exception)
                            {
                                ctx.Response.StatusCode = 500;
                                var buf = Encoding.UTF8.GetBytes(exception.ToString());
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);

                            } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
            ThreadPool.QueueUserWorkItem((c) =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
                try
                {
                    var p = Process.Start("ngrok", "http 5000");
                }
                catch (Exception)
                {
                    MessageBox.Show("Couldn't create ngrok tunnel");
                }
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
