using Newtonsoft.Json;

namespace OWAML
{
	public class ModSocketMessage
	{
		[JsonProperty("senderName")]
		public string SenderName { get; set; }

		[JsonProperty("senderType")]
		public string SenderType { get; set; }

		[JsonProperty("type")]
		public MessageType Type { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }
	}
}
