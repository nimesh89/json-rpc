using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var formatter = new JsonMessageFormatter(Encoding.UTF8);
                using (ClientWebSocket ws = new ClientWebSocket())
                using (var wsh = new WebSocketMessageHandler(ws, formatter))
                using (JsonRpc rpc = new JsonRpc(wsh))
                {
                    var consoleTracer = new ConsoleTraceListener();
                    consoleTracer.Filter = new EventTypeFilter(SourceLevels.All);

                    var tr = new TraceSource("test");
                    tr.Listeners.Clear();
                    tr.Listeners.Add(consoleTracer);
                    rpc.TraceSource = tr;

                    Uri serverUri = new Uri("wss://app.sensemetrics.com:4201");
                    ws.ConnectAsync(serverUri, CancellationToken.None).Wait();

                    rpc.Disconnected += Rpc_Disconnected;
                    rpc.StartListening();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            Console.ReadLine();
        }

        private static void Rpc_Disconnected(object sender, JsonRpcDisconnectedEventArgs e)
        {
            Console.WriteLine(e.Reason);
            Console.WriteLine(e.LastMessage);
            Console.WriteLine(e.Description);
            if(e.Exception != null)
            {
                Console.WriteLine(e.Exception.Message);
                Console.WriteLine(e.Exception.StackTrace);
            }
        }
    }
}
