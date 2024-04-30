using deva.libs.prasi;
using deva.libs.router;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Collections.Specialized;
using HttpRequest = Deva.HttpRequest;

namespace deva.app.page
{
    public class _upload : RouterFunc
    {
        public override async Task<ApiRespon> OnReceivedRequest(HttpRequest req, NameValueCollection res)
        {
            //IFormFile file
            PreApi preApi = new PreApi();
            string boundary = GetBoundary(req);
            List<string> result = new List<string>();
            using (MemoryStream stream = new MemoryStream(req.BodyBytes))
            {
                var pembacaMultipart = new MultipartReader(boundary, stream);
                MultipartSection bagian;
                while ((bagian = await pembacaMultipart.ReadNextSectionAsync()) != null)
                {
                    if (!String.IsNullOrEmpty(bagian.ContentType))
                    {
                        // Bagian berisi data formulir, bisa berisi file atau data lainnya
                        var contentDisposition = bagian.GetContentDispositionHeader();

                        // Misalnya, periksa apakah bagian ini adalah file
                        if (contentDisposition != null && contentDisposition.DispositionType.Equals("form-data") && !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                        {
                            // Ini adalah bagian yang berisi file, lakukan sesuatu dengan file tersebut
                            var fileName = contentDisposition.FileName.Value;
                            // Membaca konten file dari bagian dan menyimpannya
                            string urlPath = getPathUpload(fileName);
                            string url = urlPath.Replace('\\', '/');
                            Console.WriteLine($"{url}");
                            result.Add(url);
                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), urlPath);
                            Console.WriteLine($"{filePath}");
                            Console.WriteLine("CEK POINT 0");
                            try
                            {
                                Console.WriteLine("CEK POINT");
                                string directoryPath = Path.GetDirectoryName(filePath);
                                Console.WriteLine("SUDAH?");
                                if (!Directory.Exists(directoryPath))
                                {
                                    Console.WriteLine("SUDAH?");
                                    // Buat direktori jika tidak ada
                                    Directory.CreateDirectory(directoryPath);
                                }
                                Console.WriteLine("SUDAH?");
                                using (var fileStream = File.Create(filePath))
                                {
                                    Console.WriteLine("SUDAH?");
                                    await bagian.Body.CopyToAsync(fileStream);
                                }
                                Console.WriteLine("SUDAH?");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        else
                        {
                        }
                    }
                }
            }
            return Reply(JsonConvert.SerializeObject(result));

        }
        static async Task SaveFile(HttpRequest req)
        {

        }
        static string getPathUpload(string filename)
        {
            DateTime now = DateTime.Now;
            int year = now.Year;
            int month = now.Month;
            int day = now.Day;
            long millisecond = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            string fileName = $"{millisecond}-{filename}";
            string filePath = Path.Combine("upload", $"{year}-{month}", $"{day}", fileName);
            return filePath;
        }
        static string GetBoundary(HttpRequest req)
        {

            List<(string, string)> header = req.HeaderList;
            List<(string, string)> hct = header.Where(e => e.Item1 == "Content-Type").ToList();
            string ct = hct[0].Item2;
            string contentTypeHeader = ct;
            string boundary = null;
            if (!string.IsNullOrEmpty(contentTypeHeader) && contentTypeHeader.Contains("boundary="))
            {
                boundary = contentTypeHeader
                    .Split(';')
                    .Select(p => p.Trim())
                    .FirstOrDefault(p => p.StartsWith("boundary="))
                    ?.Substring("boundary=".Length);
            }
            return boundary;
        }


    }
}
