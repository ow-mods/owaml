using System;
using System.IO;
namespace CMOWA
{
    public class Program
    {

        static void Main(string[] args) 
        {
            try
            {
                Initialization(new ArgumentHelper(args));
            }
            catch (Exception ex) 
            {
                ConsoleUtils.WriteByType("CMOWA crashed unexpectedly with the following message: " + ex.Message, MessageType.Fatal);
                ConsoleUtils.WriteByType("Source: " + ex.Source, MessageType.Fatal);
                ConsoleUtils.WriteByType("StackTrace:" + ex.StackTrace, MessageType.Fatal);
            }

            Console.ResetColor();
            Console.ReadLine();
        }

        static void Initialization(ArgumentHelper argumentHelper)
        {
            ConsoleUtils.WriteByType($"Starting CMOWA", MessageType.Info);

            FileVerificationAndInitialization initialization = new FileVerificationAndInitialization();

            if (!initialization.CheckArguments(argumentHelper))
            {
                ConsoleUtils.WriteByType("CMOWA failed on CheckArguments", MessageType.Fatal);
                return;
            }
            if (!initialization.CheckBepInExConfig())
            {
                ConsoleUtils.WriteByType("CMOWA failed on CheckAndBepInExFiles", MessageType.Fatal);
                return;
            }
            if (!initialization.StartGame())
            {
                ConsoleUtils.WriteByType("CMOWA failed on StartGame", MessageType.Fatal);
                return;
            }
        }
    }
}
