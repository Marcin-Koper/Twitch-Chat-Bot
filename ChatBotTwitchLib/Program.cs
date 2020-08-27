using System;

namespace ChatBotTwitchLib
{
    /// <summary>
    /// Main class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main method
        /// </summary>
        static void Main(string[] args)
        {
            ChatBot bot = new ChatBot();

            //true if bot should log
            bot.Connect(false); 

            Console.ReadLine();

            bot.Disconnect();
        }
    }
}
