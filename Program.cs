using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
            Highscore[] highscores = new Highscore[3];
            highscores[0] = new Highscore("Player A", 1337);
            highscores[1] = new Highscore("Player B", 13);
            highscores[2] = new Highscore("Player C", 2);
            GetHighscoresResponse response = new GetHighscoresResponse(highscores);
            SerializedApiResponse serializedApiResponse = SerializedApiResponse.Create(response);
            File.WriteAllText("GetHighscoresResponse.json", serializedApiResponse.Serialize());
        }

        private static void AddQuestion()
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