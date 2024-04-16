using deva.libs.prasi;
using deva.libs.router;
using System.Collections.Specialized;
using System.Net.Mime;
using HttpRequest = Deva.HttpRequest;

namespace deva.app.page
{
    public class _file : RouterFunc
    {
        public override async Task<ApiRespon> OnReceivedRequest(HttpRequest req, NameValueCollection res)
        {
            PreApi preApi = new PreApi();

            Dictionary<string, dynamic> page = new Dictionary<string, dynamic>();
            try
            {
                string _ = res["_"];
                string _filesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "public", "doc");
                string filePath = Path.Combine(_filesDirectory, _);
                if (!File.Exists(filePath))
                    return Reply("404");
                // Baca file sebagai byte array
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string imageExtension = Path.GetExtension(filePath);
                string contentType = GetContentType(filePath);
                return ReplyStream(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return Reply(ex.Message);

            }
            //Dictionary<string, dynamic> web = new Dictionary<string, dynamic>();
            //string prasiFile = Global.Config["site"]["id"];
            string reply = string.Empty;
            return Reply("404");
        }
        static string GetContentType(string filePath)
        {
            try
            {
                ContentType contentType = new ContentType(filePath);
                return contentType.MediaType;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gagal mendapatkan ContentType: " + ex.Message);
                return null;
            }
        }

    }
}
