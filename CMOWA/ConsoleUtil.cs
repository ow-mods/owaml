﻿using System;

namespace CMOWA
{
    public static class ConsoleUtils
    {
        public static void WriteByType(string message, MessageType type = MessageType.Message)
        {
            ConsoleColor GetColorFromMessageType(MessageType messageType)
            {
                switch (messageType)
                {
                    case MessageType.Error:
                        return ConsoleColor.Red;
                    case MessageType.Warning:
                        return ConsoleColor.Yellow;
                    case MessageType.Info:
                        return ConsoleColor.Cyan;
                    case MessageType.Success:
                        return ConsoleColor.Green;
                    case MessageType.Fatal:
                        return ConsoleColor.Magenta;
                    case MessageType.Debug:
                        return ConsoleColor.DarkGray;

                    default:
                    case MessageType.Message:
                        return ConsoleColor.White;
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                Console.ForegroundColor = GetColorFromMessageType(type);
                Console.WriteLine(message);
            }
        }
    }
    public enum MessageType
    {
        Message,
        Error,
        Warning,
        Info,
        Success,
        Fatal,
        Debug
    }
}
