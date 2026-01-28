using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.UI.Helpers
{
    public class WebSocketHelper
    {
        private ClientWebSocket _ws;
        private readonly Uri _serverUri = new Uri("ws://localhost:8080");

        public WebSocketHelper()
        {
            _ws = new ClientWebSocket();
        }

        public async Task ConnectAsync()
        {
            if (_ws.State != WebSocketState.Open)
            {
                // Re-create if aborted/closed
                if (_ws.State == WebSocketState.Aborted || _ws.State == WebSocketState.Closed)
                    _ws = new ClientWebSocket();
                    
                await _ws.ConnectAsync(_serverUri, CancellationToken.None);
            }
        }

        public async Task SendMessageAsync(string message)
        {
            await ConnectAsync();
            var bytes = Encoding.UTF8.GetBytes(message);
            await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task StartListening(Action<string> onMessageReceived)
        {
            await ConnectAsync();
            var buffer = new byte[1024 * 4];
            
            _ = Task.Run(async () =>
            {
                while (_ws.State == WebSocketState.Open)
                {
                    try
                    {
                        var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            onMessageReceived?.Invoke(msg);
                        }
                    }
                    catch (Exception)
                    {
                        // Handle disconnect/error
                        break;
                    }
                }
            });
        }
    }
}
