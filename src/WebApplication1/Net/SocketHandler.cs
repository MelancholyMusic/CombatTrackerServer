using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace CombatTrackerServer.Net
{
    public class SocketHandler
    {
		public const int BUFFER_SIZE = 4096;

		private WebSocket socket;

		SocketHandler(WebSocket socket)
		{
			this.socket = socket;
		}

		private async Task EchoLoop()
		{
			byte[] buffer = new byte[BUFFER_SIZE];
			ArraySegment<byte> seg = new ArraySegment<byte>(buffer);

			while(socket.State == WebSocketState.Open)
			{
				var incoming = await socket.ReceiveAsync(seg, CancellationToken.None);
				var outgoing = new ArraySegment<byte>(buffer, 0, incoming.Count);
				await socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
			}
		}

		static async Task Acceptor(HttpContext hc, Func<Task> n)
		{
			if(!hc.WebSockets.IsWebSocketRequest)
				return;

			var socket = await hc.WebSockets.AcceptWebSocketAsync();
			var h = new SocketHandler(socket);
			await h.EchoLoop();
		}

		public static void Map(IApplicationBuilder app)
		{
			app.UseWebSockets();
			app.Use(Acceptor);
		}
    }
}
