using deva.libs.prasi;
using deva.libs.router;
using System.Collections.Specialized;
using HttpRequest = Deva.HttpRequest;

namespace deva.app.page
{
    public class _img : RouterFunc
    {
        public override async Task<ApiRespon> OnReceivedRequest(HttpRequest req, NameValueCollection res)
        {
            PreApi preApi = new PreApi();

            Dictionary<string, dynamic> page = new Dictionary<string, dynamic>();
            try
            {
                string _ = res["_"];
                string _filesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "public", "img");
                string filePath = Path.Combine(_filesDirectory, _);
                if (!File.Exists(filePath))
                    return Reply("404");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string imageExtension = Path.GetExtension(filePath);
                string contentType = $"image/{imageExtension.TrimStart('.')}";
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
    }
}
