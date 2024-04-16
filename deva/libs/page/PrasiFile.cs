using deva.libs.router;
using Newtonsoft.Json;
using System.IO.Compression;

namespace deva.libs.page
{
    public static class Global
    {
        public static string site_id { get; set; }
        public static string ConfigRaw { get; set; }
        public static Dictionary<string, dynamic> Config { get; set; }
        public static Dictionary<string, WebConfig> web { get; set; }
        public static List<RouterModel> Route { get; set; }

    }
    public class WebConfig
    {
        public string site_id { get; set; }
        public Dictionary<string, dynamic> cache { get; set; }

    }
    public class PrasiFile
    {
        public string GetPrasiFile(string path)
        {
            // Ubah path file.gz sesuai dengan lokasi dan nama file Anda
            //string filePath = "file.gz";
            string result = "";
            // Membuka file .gz
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                // Membuat objek GZipStream untuk membaca file .gz
                using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                {
                    // Membuat objek MemoryStream untuk menampung data dari file .gz
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        // Menyalin data dari GZipStream ke MemoryStream
                        gzipStream.CopyTo(memoryStream);

                        // Mendapatkan nilai dari MemoryStream
                        byte[] data = memoryStream.ToArray();

                        // Mengonversi byte array menjadi string (atau sesuai dengan format data yang Anda miliki)
                        string text = System.Text.Encoding.UTF8.GetString(data);

                        // Menggunakan nilai-nilai dari file .gz
                        //Console.WriteLine(text);
                        result = text;
                    }
                }
            }
            Global.ConfigRaw = result;
            Dictionary<string, dynamic> jsonObject = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Global.ConfigRaw);

            Global.Config = jsonObject;
            string id_site = Global.Config["site"]["id"];
            WebConfig cache = new WebConfig();
            cache.site_id = id_site;
            cache.cache = jsonObject;

            Dictionary<string, WebConfig> web = new Dictionary<string, WebConfig>();
            web[id_site] = cache;
            Global.web = web;
            return result;
        }
    }
}
