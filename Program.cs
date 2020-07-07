using System;
using System.Diagnostics;
using System.Threading;
using qsrv.ApiRequests;
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
            Question question = new Question(
                "Wie heißt keine der Göttinen aus Ocarina of time?",
                new Answer[]
                {
                    new Answer("Ganon", true),
                    new Answer("Nayru", false),
                    new Answer("Din", false),
                    new Answer("Farore", false)
                },
                Category.Nerdistan);
            MainServer.AddQuestion(question);
            Console.WriteLine("done!");
        }
    }
}