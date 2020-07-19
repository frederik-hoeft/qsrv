using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using qsrv.ApiRequests;
using qsrv.ApiResponses;
using qsrv.Database;
using washared.DatabaseServer;
using washared.DatabaseServer.ApiResponses;

namespace qsrv
{
    internal static class Program
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        private static void Main()
        {
            MainServer.LoadConfig();
            if (MainServer.Config.WamsrvDevelopmentConfig.BlockResponses)
            {
                Development();
                return;
            }
            MainServer.Run();
        }

        private static void Development()
        {
            AddQuestion();
        }

        private static void AddQuestion()
        {
            Question question = new Question(
                "",
                new Answer[]
                {
                    new Answer("", true),
                    new Answer("", false),
                    new Answer("", false),
                    new Answer("", false)
                },
                Category.Undefined);
            MainServer.AddQuestion(question);
            Console.WriteLine("Question added!");
        }
    }
}