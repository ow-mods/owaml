using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CMOWA
{
    public class FileVerificationAndInitialization
    {

        const string doorstop_configFileName = "doorstop_config.ini";
        const string winhttpFileName = "winhttp.dll";
        const string bepInExConfigFileName = "BepInEx.cfg";
        const string bepInExConfigFileRelativePath = @"BepInEx\config";
        const string OuterWilds_Alpha_1_2FileName = "OuterWilds_Alpha_1_2.exe";

        const string doorstop_configtargetAssemblyCommand = "targetAssembly";
        const string doorstop_configtargetAssemblyDefaultCommand = @"BepInEx\core\BepInEx.Preloader.dll";

        const string bepInExConfigEditFile = "BepinExConfigEdit.json";

        private string gameFolder;
        public bool CheckArguments(ArgumentHelper argumentHelper)
        {
            if (!argumentHelper.HasArgument("gamePath"))
            {
                ConsoleUtils.WriteByType($"gamePath argument wasn't found", MessageType.Error);
                return false;
            }

            gameFolder = argumentHelper.GetArgument("gamePath");

            if (!Directory.Exists(gameFolder))
            {
                ConsoleUtils.WriteByType($"Game folder ({gameFolder}) wasn't found", MessageType.Error);
                return false;
            }
            return true;
        }

        public bool CheckBepInExConfig()
        {
            #region bepinex_config_file_handling

            if (!File.Exists(bepInExConfigEditFile))
            {
                ConsoleUtils.WriteByType($"{bepInExConfigEditFile} file wasn't found", MessageType.Error);
                return false;
            }

            string bepInExConfigDataToCheckFile = File.ReadAllText(bepInExConfigEditFile);
            ConfigurationFileCheckData bepInExConfigDataToCheck = JsonConvert.DeserializeObject<ConfigurationFileCheckData>(bepInExConfigDataToCheckFile);

            bool wasBepInExConfigFileGenerated = File.Exists(Path.Combine(gameFolder, bepInExConfigFileRelativePath, bepInExConfigFileName));

            if (!wasBepInExConfigFileGenerated)
            {
                ConsoleUtils.WriteByType($"File {bepInExConfigFileName} wasn't generated yet, creating one...", MessageType.Warning);

                string[] bepInExConfigFile = CreateBepInExConfigFile(bepInExConfigDataToCheck);

                Directory.CreateDirectory(Path.Combine(gameFolder, bepInExConfigFileRelativePath));

                File.WriteAllLines(Path.Combine(gameFolder, bepInExConfigFileRelativePath, bepInExConfigFileName), bepInExConfigFile);
                ConsoleUtils.WriteByType($"Saved {bepInExConfigFileName}", MessageType.Success);
            }

            #endregion

            return true;
        }

        public bool StartGame() 
        {
            ConsoleUtils.WriteByType("Starting Game");

            string gameEXEPath = Path.Combine(gameFolder, OuterWilds_Alpha_1_2FileName);
            if (!File.Exists(gameEXEPath))
            {
                ConsoleUtils.WriteByType($"Game executable ({gameEXEPath}) wasn't found", MessageType.Error);
                return false;
            }
            Process.Start(gameEXEPath);
            ConsoleUtils.WriteByType("Game was started", MessageType.Success);

            return true;
        }

        #region bepinex_config_editing
        private string[] CreateBepInExConfigFile(ConfigurationFileCheckData data)
        {
            List<string> bepInExConfigFile = new List<string>();

            foreach (var commandHeaderData in data.CommandHeaders)
            {
                bepInExConfigFile.Add(commandHeaderData.CommandHeader);
                bepInExConfigFile.Add("");
                foreach (var commands in commandHeaderData.Commands)
                {
                    bepInExConfigFile.Add(string.Join(" ", commands.Command, "=", commands.Value));
                }
            }
            return bepInExConfigFile.ToArray();
        }
        #endregion
    }
}
