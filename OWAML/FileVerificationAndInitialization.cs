using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace OWAML
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
        private string bepInExFolder;
        private string consolePort;
        private SocketListener socketListener;

        public bool CheckArguments(ArgumentHelper argumentHelper)
        {
            if (!argumentHelper.HasArgument("gamePath"))
            {
                ConsoleUtils.WriteByType($"gamePath argument wasn't found", MessageType.Error);
                return false;
            }


            gameFolder = argumentHelper.GetArgument("gamePath");
            bepInExFolder = argumentHelper.GetArgument("bepInExFolder");
            if (argumentHelper.HasArgument("consolePort"))
                consolePort = argumentHelper.GetArgument("consolePort");
            else
            {
                socketListener = new SocketListener();
                consolePort = socketListener.Init().ToString();
            }

            if (!argumentHelper.HasArgument("bepInExFolder"))
            {
                bepInExFolder = gameFolder;
            }

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

                Directory.CreateDirectory(Path.Combine(bepInExFolder, bepInExConfigFileRelativePath));

                File.WriteAllLines(Path.Combine(bepInExFolder, bepInExConfigFileRelativePath, bepInExConfigFileName), bepInExConfigFile);
                ConsoleUtils.WriteByType($"Saved {bepInExConfigFileName}", MessageType.Success);
            }

            #endregion

            return true;
        }

        public bool CheckBepInExDoorstopAndWinhttp() 
        {
            if (gameFolder.Equals(bepInExFolder))
                return true;
            #region doorstop_ini_file_handling
            bool isDoorstopAlreadyMoved = File.Exists(Path.Combine(gameFolder, doorstop_configFileName));

            string doorstopFolderToCheck = (isDoorstopAlreadyMoved ? gameFolder : bepInExFolder);
            string doorstopFilePath = Path.Combine(doorstopFolderToCheck, doorstop_configFileName);

            if (!File.Exists(doorstopFilePath))
            {
                ConsoleUtils.WriteByType($"{doorstop_configFileName} ({doorstopFilePath}) file wasn't found", MessageType.Error);
                return false;
            }

            string[] doorstopFile = File.ReadAllLines(doorstopFilePath);

            ConsoleUtils.WriteByType("Checking doorstop_config.ini");
            bool isThereAnyChangesToDoorstop = CheckDoorstopConfigFile(ref doorstopFile, bepInExFolder);

            if (isThereAnyChangesToDoorstop)
            {
                ConsoleUtils.WriteByType(isDoorstopAlreadyMoved ? "Saving doorstop_config.ini" : "Copying doorstop_config.ini", MessageType.Warning);
                File.WriteAllLines(Path.Combine(gameFolder, doorstop_configFileName), doorstopFile);

                ConsoleUtils.WriteByType(isDoorstopAlreadyMoved ? "doorstop_config.ini was saved" : "doorstop_config.ini was copied", MessageType.Success);
            }

            #endregion

            #region winhttp_file_handling

            bool iswinhttpAlreadyMoved = File.Exists(Path.Combine(gameFolder, winhttpFileName));
            if (!iswinhttpAlreadyMoved)
            {
                ConsoleUtils.WriteByType("Copying winhttp.dll");

                string winhttpFilePath = Path.Combine(bepInExFolder, winhttpFileName);

                if (!File.Exists(winhttpFilePath))
                {
                    ConsoleUtils.WriteByType($"{winhttpFileName} ({winhttpFilePath}) file wasn't found", MessageType.Error);
                    return false;
                }

                File.Copy(winhttpFilePath, Path.Combine(gameFolder, winhttpFileName));

                ConsoleUtils.WriteByType("winhttp.dll was copied", MessageType.Success);
            }
            return true;
            #endregion
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
            if (string.IsNullOrWhiteSpace(consolePort))
            {
                Process.Start(gameEXEPath);
            }
            else
            {
                Process.Start(gameEXEPath, $"-consolePort {consolePort}");
            }
            ConsoleUtils.WriteByType("Game was started", MessageType.Success);

            return true;
        }

        private bool CheckDoorstopConfigFile(ref string[] doorstopFile, string bepInExFolder)
        {
            bool isThereAnyChanges = false;
            for (int i = 0; i < doorstopFile.Length; i++)
            {
                if (doorstopFile[i].StartsWith(doorstop_configtargetAssemblyCommand))
                {
                    string expectedCommand = Path.Combine(bepInExFolder, doorstop_configtargetAssemblyDefaultCommand);
                    if (!doorstopFile[i].EndsWith(expectedCommand))
                    {
                        int equalSignPosition = doorstopFile[i].IndexOf('=');
                        string removedCommandString = doorstopFile[i].Remove(equalSignPosition + 1);
                        doorstopFile[i] = removedCommandString + expectedCommand;
                        ConsoleUtils.WriteByType("Editing doorstop_config.ini", MessageType.Warning);
                        isThereAnyChanges = true;
                    }
                }
            }
            return isThereAnyChanges;
        }

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
    }
}
