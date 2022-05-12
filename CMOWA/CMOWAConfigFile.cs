using Newtonsoft.Json;
namespace CMOWA
{
    public  class CMOWAConfigFile
    {
        [JsonProperty("gamePath")]
        public string GamePath { get; private set; }

        [JsonProperty("debugMode")]
        public bool DebugMode { get; private set; }

        [JsonProperty("forceExe")]
        public bool ForceExe { get; private set; }

        [JsonProperty("cmowaPath")]
        public string CMOWAPath { get; private set; }

        [JsonProperty("socketPort")]
        public int SocketPort { get; private set; }
    }
}
