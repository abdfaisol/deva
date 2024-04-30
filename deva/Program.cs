using deva.libs.page;
using deva.libs.router;
using System.Net;
using System.Net.Sockets;

namespace Deva
{

    class HttpCacheSession : HttpSession
    {
        public HttpCacheSession(HttpServer server) : base(server) { }
        protected override async Task OnReceivedRequestAsync(HttpRequest request)
        {
            Dictionary<string, dynamic> header = new Dictionary<string, dynamic>();



            string key = request.Url;
            key = Uri.UnescapeDataString(key);

            Router route = new Router();
            if (key.Contains("?"))
            {
                string[] splitKey = key.Split("?");
                key = splitKey[0];
            }
            ModelLookUp result = await route.LookUp(key, request);
            if (result.status)
            {
                if (result.typeout == "page")
                {
                    string site_id = "";
                    string indexJs = @"<!DOCTYPE html>
                    <html lang=""en"">

                    <head>
                      <meta charset=""UTF-8"">
                      <meta name=""viewport""
                        content=""width=device-width, initial-scale=1.0, user-scalable=1.0, minimum-scale=1.0, maximum-scale=1.0"">
                      <link rel=""stylesheet"" href=""/index.css"">
                    </head>

                    <body class=""flex-col flex-1 w-full min-h-screen flex opacity-0"">
                      <div id=""root""></div>
                      <script>";
                    indexJs += @"
                        window._prasi = { basepath: ""/"", site_id: """ + Global.site_id;

                    indexJs += @"""}
                      </script>
                      <script src=""/main.js"" type=""module""></script>
                    </body>
                    </html>";
                    SendResponseAsync(Response.MakeResponHtml(indexJs));
                }
                else if (result.typeout == "stream")
                {

                    //SendResponseBodyAsync(result.Stream);
                    SendResponseAsync(Response.MakeGetResponse(result.Stream, result.type));
                }
                else if (result.typeout == "error")
                {

                    SendResponseAsync(Response.MakeErrorResponse(result.ErrorCode, result.respon));
                }
                else
                {
                    SendResponseAsync(Response.MakeGetResponse(result.respon, result.type));

                }
            }
            else
            {


                string currentDirectory = Directory.GetCurrentDirectory();

                string[] pathUrl = key.Split('/');
                string dirFile = "";
                string file = pathUrl[pathUrl.Length - 1];
                string[] newDir = pathUrl.Take(pathUrl.Length - 1).ToArray();
                string combinedPath = Path.Combine(newDir);
                string path = Path.Join(currentDirectory, "app", combinedPath, file);
                string pathParent = pathUrl[1];
                int id = 0;
                foreach (var item in pathUrl)
                {
                    Console.WriteLine($"{id}: {item}");
                    id++;
                }

                string existsFile = Path.Join(currentDirectory, combinedPath, file);
                Console.WriteLine(existsFile);
                if (File.Exists(path))
                {
                    byte[] content = File.ReadAllBytes(path);
                    SendResponseAsync(Response.MakeGetResponse(content, GetContentType(path)));
                }
                else if (pathParent == "upload" && File.Exists(existsFile))
                {

                    byte[] fileBytes = File.ReadAllBytes(existsFile);
                    string imageExtension = GetContentType(existsFile);
                    SendResponseAsync(Response.MakeGetResponse(fileBytes, imageExtension));

                }
                else
                {
                    string indexJs = @"<!DOCTYPE html>
                    <html lang=""en"">

                    <head>
                      <meta charset=""UTF-8"">
                      <meta name=""viewport""
                        content=""width=device-width, initial-scale=1.0, user-scalable=1.0, minimum-scale=1.0, maximum-scale=1.0"">
                      <link rel=""stylesheet"" href=""/index.css"">
                    </head>

                    <body class=""flex-col flex-1 w-full min-h-screen flex opacity-0"">
                      <div id=""root""></div>
                      <script>";
                    indexJs += @"
                        window._prasi = { basepath: ""/"", site_id: """ + Global.site_id;

                    indexJs += @"""}
                      </script>
                      <script src=""/main.js"" type=""module""></script>
                    </body>
                    </html>";
                    SendResponseAsync(Response.MakeResponHtml(indexJs));

                }
            }


        }
        static string GetContentType(string filePath)
        {
            try
            {
                string imageExtension = Path.GetExtension(filePath);
                Dictionary<string, string> mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                mapping.Add(".txt", "text/plain");
                mapping.Add(".html", "text/html");
                mapping.Add(".css", "text/css");
                mapping.Add(".jpg", "image/jpeg");
                mapping.Add(".jpeg", "image/jpeg");
                mapping.Add(".png", "image/png");
                mapping.Add(".gif", "image/gif");
                mapping.Add(".js", "application/javascript");
                if (mapping.ContainsKey(imageExtension))
                {
                    return mapping[imageExtension];
                }
                else
                {
                    return "application/octet-stream";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gagal mendapatkan ContentType: " + ex.Message);
                return null;
            }
        }
    }
    class HttpCacheServer : HttpServer
    {
        public HttpCacheServer(IPAddress address, int port) : base(address, port) { }


        protected override TcpSession CreateSession() { return new HttpCacheSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"HTTP session caught an error: {error}");
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // HTTP server port


            int port = 4001;
            if (args.Length > 0)
                port = int.Parse(args[0]);

            Console.WriteLine($"HTTP server port: {port}");
            Console.WriteLine($"HTTP server website: http://localhost:{port}");
            // Create a new HTTP server
            var server = new HttpCacheServer(IPAddress.Any, port);

            Prasi prasi = new Prasi();
            // Configuration
            prasi.DeployConfiguration();

            // Prasi File Deploy
            prasi.DeployFilePrasi();

            // Router Page
            prasi.DeployRoute();
            // Generate Schema
            prasi.DeploySchema();
            prasi.GenSchemaFile();
            // Start Server
            server.Start();

            Console.WriteLine($"Server is already to used");
            // Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

            //// Perform text input
            for (; ; )
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Restart the server
                if (line == "!")
                {
                    Console.Write("Server restarting...");
                    server.Restart();
                    Console.WriteLine("Done!");
                }
            }

        }
    }
}
