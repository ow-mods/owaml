using Newtonsoft.Json;

namespace OWAML
{
    public class ConfigurationFileCheckData
	{
		[JsonProperty("commandHeaders")]
		public ConfigurationFileCommandHeaderData[] CommandHeaders { get; private set; } 
	}
	public class ConfigurationFileCommandHeaderData
	{
		[JsonProperty("commandHeader")]
		public string CommandHeader { get; private set; }

		[JsonProperty("commands")]
		public ConfigurationFileCommandData[] Commands { get; private set; }
	}
	public class ConfigurationFileCommandData
	{
		[JsonProperty("command")]
		public string Command { get; private set; }

		[JsonProperty("value")]
		public string Value { get; private set; }
	}
}
