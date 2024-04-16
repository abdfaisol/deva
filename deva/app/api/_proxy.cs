using deva.libs.router;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Specialized;
using HttpRequest = Deva.HttpRequest;

namespace deva.app.page
{
    public class _proxy : RouterFunc
    {
        public override async Task<ApiRespon> OnReceivedRequest(HttpRequest req, NameValueCollection res)
        {
            Dictionary<string, dynamic> param = BodyParameter(req.Body);
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            string _ = res["_"];
            string[] parts = _.Split("/");
            string pathname = parts[0];
            try
            {
                var client = new RestClient();
                var request = new RestRequest(_, Method.Post);
                request.AddStringBody(req.Body, "application/json");
                RestResponse response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return Reply(response.Content);
                }
                else
                {
                    result["error"] = response.Content;
                    return ReplyError(403, JsonConvert.SerializeObject(result));
                }
            }
            catch (Exception ex)
            {
                result["Error"] = ex;
            }
            string reply = string.Empty;
            return Reply(JsonConvert.SerializeObject(result));
        }
    }
}
