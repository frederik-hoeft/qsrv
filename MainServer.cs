using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using qsrv.Config;
using qsrv.ApiRequests;
using System.Diagnostics;
using qsrv.Database;
using washared.DatabaseServer.ApiResponses;
using washared.DatabaseServer;

namespace qsrv
{
    /// <summary>
    /// Main server loop accepting connections
    /// </summary>
    public static class MainServer
    {
        public static WamsrvConfig Config;
        private static bool configLoaded = false;
        public static int ClientCount = 0;
        public static X509Certificate2 ServerCertificate;

        public static void Run()
        {
            IPAddress ipAddress = IPAddress.Parse(Config.LocalIpAddress);
            if (ipAddress == null)
            {
                Console.WriteLine("Unable to resolve IP address.");
                return;
            }
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Config.LocalPort);
            using Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Listen(16);
            Console.WriteLine("Started Main Quiz Server on " + Config.LocalIpAddress + ":" + Config.LocalPort.ToString() + "!");
            while (true)
            {
                Socket clientSocket = socket.Accept();
                ClientCount++;
                new Thread(() => ApiServer.Create(clientSocket)).Start();
            }
        }

        public static async void AddQuestion(Question question)
        {
            using DatabaseManager databaseManager = new DatabaseManager(new ApiContext(ApiServer.CreateDummy(),ApiRequestId.Invalid));
            string query = "SELECT EXISTS (SELECT 1 FROM Tbl_questions WHERE text = \'" + InputSanitizer.Sanitize(question.Text) + "\' LIMIT 1);";
            SqlApiRequest request = SqlApiRequest.Create(SqlRequestId.GetSingleOrDefault, query, 1);
            Optional<SqlSingleOrDefaultResponse> optionalSingle = await databaseManager.GetSingleOrDefaultResponseAsync(request);
            if (!optionalSingle.Success || !optionalSingle.Result.Success)
            {
                Console.WriteLine("Failed to add question.");
                return;
            }
            query = InputSanitizer.SanitizeQuery(new string[] { "INSERT INTO Tbl_questions (text, category) VALUES (\'", question.Text, "\', \'", ((int)question.Category).ToString(), "\');" });
            request = SqlApiRequest.Create(SqlRequestId.ModifyData, query, -1);
            Optional<SqlModifyDataResponse> optionalResponse = await databaseManager.GetModifyDataResponseAsync(request);
            if (!optionalResponse.Success)
            {
                Console.WriteLine("Failed to add question.");
                return;
            }
            query = "SELECT id FROM Tbl_questions WHERE text = \'" + InputSanitizer.Sanitize(question.Text) + "\' LIMIT 1;";
            request = SqlApiRequest.Create(SqlRequestId.GetSingleOrDefault, query, 1);
            optionalSingle = await databaseManager.GetSingleOrDefaultResponseAsync(request);
            if (!optionalSingle.Success || !optionalSingle.Result.Success)
            {
                Console.WriteLine("Failed to get added question.");
                return;
            }
            for (int i = 0; i < question.Answers.Length; i++)
            {
                Answer answer = question.Answers[i];
                query = InputSanitizer.SanitizeQuery(new string[] { "INSERT INTO Tbl_answers (qid, text, isCorrect) VALUES (", optionalSingle.Result.Result, ", \'", answer.Text, "\', ", Convert.ToInt32(answer.IsCorrect).ToString(), ");" });
                request = SqlApiRequest.Create(SqlRequestId.ModifyData, query, -1);
                optionalResponse = await databaseManager.GetModifyDataResponseAsync(request);
                if (!optionalResponse.Success)
                {
                    Console.WriteLine("Failed to add answer.");
                    return;
                }
            }
        }

        public static void LoadConfig()
        {
            if (configLoaded)
            {
                return;
            }
            string config = File.ReadAllText("Config" + Path.DirectorySeparatorChar + "wamsrv.config.json");
            Config = JsonConvert.DeserializeObject<WamsrvConfig>(config);
            ServerCertificate = new X509Certificate2(Config.PfxCertificatePath, Config.PfxPassword);
            configLoaded = true;
        }
    }
}