using System;
namespace CMOWA
{
    public class Program
    {

        static void Main(string[] args) 
        {
            bool exit = false;
            try
            {
                exit = Initialization(new ArgumentHelper(args));
            }
            catch (Exception ex) 
            {
                exit = false;
                ConsoleUtils.WriteByType("CMOWA crashed unexpectedly with the following message: " + ex.Message, MessageType.Fatal);
                ConsoleUtils.WriteByType("Source: " + ex.Source, MessageType.Fatal);
                ConsoleUtils.WriteByType("StackTrace:" + ex.StackTrace, MessageType.Fatal);
            }

            Console.ResetColor();
            if (!exit)
                Console.ReadLine();
        }

        public static bool Initialization(ArgumentHelper argumentHelper)
        {
            ConsoleUtils.WriteByType($"Starting CMOWA", MessageType.Info);

            FileVerificationAndInitialization initialization = new FileVerificationAndInitialization();

            if (!initialization.CheckArguments(argumentHelper))
            {
                ConsoleUtils.WriteByType("CMOWA failed on CheckArguments", MessageType.Fatal);
                return false;
            }
            if (!initialization.CheckBepInExConfig())
            {
                ConsoleUtils.WriteByType("CMOWA failed on CheckAndBepInExFiles", MessageType.Fatal);
                return false;
            }
            if (!initialization.CheckBepInExDoorstopAndWinhttp())
            {
                ConsoleUtils.WriteByType("CMOWA failed on CheckBepInExDoorstopAndWinhttp", MessageType.Fatal);
                return false;
            }
            if (!initialization.StartGame())
            {
                ConsoleUtils.WriteByType("CMOWA failed on StartGame", MessageType.Fatal);
                return false;
            }
            return true;
        }
    }
}
