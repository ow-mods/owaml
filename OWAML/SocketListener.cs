using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace OWAML
{
	public class SocketListener
	{
		private const string Separator = "\n--------------------------------";
		private const int BufferSize = 262144;
		private static int _port;
		private static TcpListener _server;
		private bool _hasReceivedFatalMessage;

		public int Init()
		{
			var listener = new TcpListener(IPAddress.Loopback, 0);
			listener.Start();
			_port = ((IPEndPoint)listener.LocalEndpoint).Port;
			listener.Stop();

			new Task(SetupSocketListener).Start();
			return _port;
		}

		private void SetupSocketListener()
		{
			try
			{
				ListenToSocket();
			}
			catch (SocketException ex)
			{
				ConsoleUtils.WriteByType($"Error in socket listener: {ex}", MessageType.Error);
			}
			catch (Exception ex)
			{
				ConsoleUtils.WriteByType($"Error while listening: {ex}", MessageType.Error);
			}
			finally
			{
				_server?.Stop();
			}
		}

		private void ListenToSocket()
		{
			var localAddress = IPAddress.Parse("127.0.0.1");

			_server = new TcpListener(localAddress, _port);
			_server.Start();

			var bytes = new byte[BufferSize];

			while (true)
			{
				var client = _server.AcceptTcpClient();

				ConsoleUtils.WriteByType("Console connected to socket!", MessageType.Success);

				var stream = client.GetStream();

				int i;

				while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
				{
					ProcessMessage(bytes, i);
				}

				ConsoleUtils.WriteByType("Closing client!", MessageType.Success);
				client.Close();
			}
		}

		private void ProcessMessage(byte[] bytes, int count)
		{
			var message = Encoding.UTF8.GetString(bytes, 0, count);
			var jsons = message.Split('\n');
			foreach (var json in jsons)
			{
				if (string.IsNullOrWhiteSpace(json))
				{
					continue;
				}

				ModSocketMessage data;
				try
				{
					data = JsonConvert.DeserializeObject<ModSocketMessage>(json);
				}
				catch (Exception ex)
				{
					ConsoleUtils.WriteByType($"Failed to process following message:{Separator}\n{json}{Separator}", MessageType.Warning);
					ConsoleUtils.WriteByType($"Reason: {ex.Message}", MessageType.Warning);
					continue;
				}

				if (data.Type == MessageType.Quit && !_hasReceivedFatalMessage)
				{
					Environment.Exit(0);
				}
				else if (data.Type == MessageType.Fatal)
				{
					_hasReceivedFatalMessage = true;
				}

				var nameTypePrefix = $"[{data.SenderName}.{data.SenderType}] : ";

				var messageData = data.Message;
				messageData = messageData.Replace("\n", $"\n{new string(' ', nameTypePrefix.Length)}");

				ConsoleUtils.WriteByType($"{nameTypePrefix}{messageData}", data.Type);
			}
		}
	}
}