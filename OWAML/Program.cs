using System;
namespace OWAML
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
                ConsoleUtils.WriteByType("OWAML crashed unexpectedly with the following message: " + ex.Message, MessageType.Fatal);
                ConsoleUtils.WriteByType("Source: " + ex.Source, MessageType.Fatal);
                ConsoleUtils.WriteByType("StackTrace:" + ex.StackTrace, MessageType.Fatal);
            }

            Console.ResetColor();
            Console.ReadLine();
        }

        public static bool Initialization(ArgumentHelper argumentHelper)
        {
            ConsoleUtils.WriteByType($"Starting OWAML", MessageType.Info);

            FileVerificationAndInitialization initialization = new FileVerificationAndInitialization();

            if (!initialization.CheckArguments(argumentHelper))
            {
                ConsoleUtils.WriteByType("OWAML failed on CheckArguments", MessageType.Fatal);
                return false;
            }
            if (!initialization.CheckBepInExConfig())
            {
                ConsoleUtils.WriteByType("OWAML failed on CheckAndBepInExFiles", MessageType.Fatal);
                return false;
            }
            if (!initialization.CheckBepInExDoorstopAndWinhttp())
            {
                ConsoleUtils.WriteByType("OWAML failed on CheckBepInExDoorstopAndWinhttp", MessageType.Fatal);
                return false;
            }
            if (!initialization.StartGame())
            {
                ConsoleUtils.WriteByType("OWAML failed on StartGame", MessageType.Fatal);
                return false;
            }
            return true;
        }
    }
}
