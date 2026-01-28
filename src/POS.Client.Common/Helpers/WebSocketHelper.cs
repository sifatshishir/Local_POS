using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace POS.Client.Common.Helpers
{
    public class WebSocketHelper
    {
        private ClientWebSocket _ws;
        private Uri _serverUri;

        public WebSocketHelper()
        {
            _ws = new ClientWebSocket();
            string host = EnvLoader.Get("WS_HOST", "localhost");
            int port = EnvLoader.GetInt("WS_PORT", 8080);
            _serverUri = new Uri($"ws://{host}:{port}");
        }

        public async Task ConnectAsync()
        {
            if (_ws.State == WebSocketState.Open) return;
            await _ws.ConnectAsync(_serverUri, CancellationToken.None);
        }

        public async Task SendMessageAsync(string message)
        {
            if (_ws.State != WebSocketState.Open) return;
            var buffer = Encoding.UTF8.GetBytes(message);
            await _ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task StartListening(Action<string> onMessageReceived)
        {
            var buffer = new byte[1024 * 4];
            while (_ws.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    }
                    else
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        onMessageReceived?.Invoke(message);
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
        }
    }
}
