using deva.libs.db;
using deva.libs.prasi;
using deva.libs.router;
using Newtonsoft.Json;

namespace deva.libs.page
{
    public class GeneratePage
    {
        public async void DeploySchema()
        {
            Console.WriteLine($"Generate Table: Ready");
            Console.WriteLine($"Generate Table: Progress");
            Dictionary<string, dynamic> res = new Dictionary<string, dynamic>();
            OracleDeva db = new OracleDeva();
            res = await db.GenerateTable();
            Console.WriteLine($"\rGenerate Table: Progress");
            try
            {
                string filePath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "app"), "schema.json");
                if (File.Exists(filePath))
                    File.Delete(filePath);

                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    // Buat direktori jika tidak ada
                    Directory.CreateDirectory(directoryPath);
                }
                string content = string.Empty;
                content = JsonConvert.SerializeObject(res);
                try
                {
                    // Membuat file dan menuliskan konten ke dalamnya
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine(content);
                    }

                    Console.WriteLine("Schema berhasil disimpan");
                }
                catch (IOException e)
                {
                    Console.WriteLine("Terjadi kesalahan saat membuat atau menulis ke file:");
                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception ex)
            {

            }
            Console.WriteLine($"Generate Table: Done");
        }
        public async void DeployFilePrasi()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string prasiPath = Path.Join(currentDirectory, "deploy", $"{Global.prasi_id}.gz");
            Prasi prasi = new Prasi();
            prasi.GetPrasiFile(prasiPath);
            Dictionary<string, dynamic> gz = Global.Config;
            Global.site_id = gz["site"]["id"];
            string[] files = { "index.css", "index.js", "main.js" };
            string[] pathName = { Path.Combine(Directory.GetCurrentDirectory(), "app") };
            Dictionary<string, dynamic> core = gz["code"]["core"].ToObject<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> site = gz["code"]["site"].ToObject<Dictionary<string, dynamic>>();
            int idx = 1;
            int max = 0;
            foreach (string _filesDirectory in pathName)
            {
                foreach (string fileidx in files)
                {
                    try
                    {
                        string filePath = Path.Combine(_filesDirectory, fileidx);
                        if (File.Exists(filePath))
                            File.Delete(filePath);

                        string directoryPath = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directoryPath))
                        {
                            // Buat direktori jika tidak ada
                            Directory.CreateDirectory(directoryPath);
                        }
                        string content = string.Empty;
                        if (core.ContainsKey(fileidx))
                        {
                            content = core[fileidx];
                        }
                        else if (site.ContainsKey(fileidx))
                        {
                            content = site[fileidx];
                        }
                        try
                        {
                            // Membuat file dan menuliskan konten ke dalamnya
                            using (StreamWriter writer = new StreamWriter(filePath))
                            {
                                writer.WriteLine(content);
                            }

                            string loading = $"\rCreate file" + $": {fileidx}";
                            if (loading.Length > max)
                            {
                                max = loading.Length;
                            }
                            else
                            {
                                loading = loading.PadRight(max);
                            }
                            Console.Write($"\r{loading}");
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine($"Terjadi kesalahan saat membuat atau menulis ke file: {e.Message}");
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    idx++;
                }
            }
            List<Dictionary<string, dynamic>> pages = new List<Dictionary<string, dynamic>>();
            pages = gz["pages"].ToObject<List<Dictionary<string, dynamic>>>();
            Console.WriteLine("");
        }
        public void DeployConfiguration()
        {

            var services = new ServiceCollection();
            AppConfig.ConfigureServices(services);
            //services.BuildServiceProvider();
            Console.WriteLine("Deploy Configuration: DONE");
        }
        public void DeployRoute()
        {
            Dictionary<string, dynamic> gz = Global.Config;
            List<Dictionary<string, dynamic>> pages = new List<Dictionary<string, dynamic>>();
            pages = gz["pages"].ToObject<List<Dictionary<string, dynamic>>>();
            Global.Route = new List<RouterModel>();
            List<Dictionary<string, dynamic>> urls = new List<Dictionary<string, dynamic>>();
            try
            {
                if (pages.Count > 0)
                {
                    foreach (var item in pages)
                    {
                        RouterModel RoutePage = new RouterModel();
                        RoutePage.id = item["id"];
                        RoutePage.Path = item["url"];
                        Global.Route.Add(RoutePage);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
