using deva.libs.page;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.IO.Compression;
using System.Text;
using HttpRequest = Deva.HttpRequest;

namespace deva.libs.router
{
    public static class DevaGlobal
    {
        public static string url { get; set; }

    }
    public class RouterModel
    {
        public string id { get; set; }
        public string Path { get; set; }
        public RouterFunc Model { get; set; }
    }
    public static class GzipHelper
    {
        public static async Task<byte[]> WriteGzipResponse(HttpRequest context, string content, string fileName)
        {
            context.SetHeader("Content-Type", "gzip");
            context.SetHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            MemoryStream memoryFile = new MemoryStream();
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
                {
                    using (var streamWriter = new StreamWriter(gzipStream, new UTF8Encoding(false)))
                    {
                        await streamWriter.WriteAsync(content);
                    }
                }
                byte[] byteArray = memoryStream.ToArray();
                return byteArray;
            }
        }
    }
    public class RouterFunc
    {
        public virtual async Task<ApiRespon> OnReceivedRequest(HttpRequest request, NameValueCollection result)
        {

            ApiRespon res = new ApiRespon();
            return res;
            ;
        }
        public ApiRespon ReplyError(int res, string content = "", string type = "application/json")
        {

            ApiRespon result = new ApiRespon();
            result.ErrorCode = res;
            result.Content = content;
            result.Type = type;
            result.TypeOut = "error";
            return result;
        }
        public ApiRespon Reply(string res, string type = "application/json")
        {

            ApiRespon result = new ApiRespon();
            result.Content = res;
            result.Type = type;
            return result;
        }
        public ApiRespon ReplyStream(byte[] res, string type = "application/json", string tipeOut = "stream")
        {

            ApiRespon result = new ApiRespon();
            result.Stream = res;
            result.Type = type;
            result.TypeOut = tipeOut;
            return result;
        }
        public Dictionary<string, dynamic> BodyParameter(string res)
        {
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            try
            {
                result = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(res);
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public string SearchParameterQuery(string key)
        {
            string result = string.Empty;


            // Membuat objek Uri dari URL
            Uri uri = new Uri("http://localhost:4001" + DevaGlobal.url);

            // Mendapatkan nilai parameter query dengan menggunakan properti Query
            string queryString = uri.Query;

            result = GetQueryParameterValue(queryString, key);
            return result;
        }
        static string GetQueryParameterValue(string queryString, string parameterName)
        {
            // Memisahkan parameter query menjadi pasangan kunci-nilai
            string[] parameters = queryString.TrimStart('?').Split('&');

            foreach (string parameter in parameters)
            {
                string[] pair = parameter.Split('=');
                string key = pair[0];
                string value = pair.Length > 1 ? pair[1] : null;

                // Jika nama parameter cocok dengan parameter yang dicari, kembalikan nilainya
                if (key.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                {
                    return value;
                }
            }

            // Jika parameter tidak ditemukan, kembalikan null
            return null;
        }
    }
    public class ModelLookUp
    {
        public bool status { get; set; }
        public string respon { get; set; }
        public string type { get; set; }
        public byte[] Stream { get; set; }
        public string typeout { get; set; }
        public int ErrorCode { get; set; }
    }

    public class ApiRespon
    {
        public string Content { get; set; }
        public string Type { get; set; }
        public byte[] Stream { get; set; }
        public string TypeOut { get; set; }
        public int ErrorCode { get; set; }
    }
    public class Router
    {
        public string prefix { get; set; }
        public async Task<ModelLookUp> LookUp(string path, HttpRequest request)
        {

            string url = request.Url;
            DevaGlobal.url = url;

            ModelLookUp result = new ModelLookUp();
            List<RouterModel> libPath = Controller.GetController(request);

            DevaMatcher matcher = new DevaMatcher();
            NameValueCollection vals = null;
            RouterModel res = libPath.FirstOrDefault(e => matcher.FindMatch(path, e.Path, out vals));
            if (res == null)
            {
                result.status = false;
                try
                {
                    ModelLookUp lookPage = await LookUpRoutePage(path, request);
                    if (lookPage.status)
                    {
                        result = lookPage;
                        result.typeout = "page";
                    }
                }
                catch (Exception ex)
                {

                }

            }
            else
            {

                RouterFunc route = res.Model;
                ApiRespon respon = await route.OnReceivedRequest(request, vals);
                result.status = true;
                if (respon.TypeOut == "stream")
                {
                    result.typeout = "stream";
                    result.Stream = respon.Stream;
                    result.respon = respon.Content;
                    result.type = respon.Type;
                }
                else if (respon.TypeOut == "error")
                {

                    result.respon = respon.Content;
                    result.type = respon.Type;
                    result.ErrorCode = respon.ErrorCode;
                }
                else
                {

                    result.respon = respon.Content;
                    result.type = respon.Type;
                }
            }
            return result;

        }
        public async Task<ModelLookUp> LookUpRoutePage(string path, HttpRequest request)
        {

            string url = request.Url;
            DevaGlobal.url = url;

            ModelLookUp result = new ModelLookUp();
            List<RouterModel> libPath = Global.Route;

            DevaMatcher matcher = new DevaMatcher();
            NameValueCollection vals = null;
            RouterModel res = libPath.FirstOrDefault(e => matcher.FindMatch(path, e.Path, out vals));
            if (res == null)
            {
                result.status = false;
            }
            else
            {

                RouterFunc route = res.Model;
                result.status = true;
                result.respon = "INI PAGE";
            }
            return result;

        }
    }
}
