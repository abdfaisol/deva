using deva.libs.page;
using deva.libs.prasi;
using deva.libs.router;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using HttpRequest = Deva.HttpRequest;

namespace deva.app.page
{
    public class _prasi : RouterFunc
    {
        public override async Task<ApiRespon> OnReceivedRequest(HttpRequest req, NameValueCollection res)
        {
            PreApi preApi = new PreApi();
            Dictionary<string, dynamic> param = BodyParameter(req.Body);
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            List<Dictionary<string, dynamic>> pages = new List<Dictionary<string, dynamic>>();
            List<Dictionary<string, dynamic>> comps = new List<Dictionary<string, dynamic>>();
            List<Dictionary<string, dynamic>> resultArray = new List<Dictionary<string, dynamic>>();

            Dictionary<string, dynamic> page = new Dictionary<string, dynamic>();
            try
            {
                string _ = res["_"];
                string[] parts = _.Split("/");

                string pathname = parts[0];

                Dictionary<string, dynamic> gz = Global.Config;

                pages = gz["pages"].ToObject<List<Dictionary<string, dynamic>>>();
                string[] slicedPath = new string[parts.Length - 1];
                Array.Copy(parts, 1, slicedPath, 0, parts.Length - 1);
                string path = string.Join("/", slicedPath);

                switch (pathname)
                {
                    case "_":
                        result["prasi"] = "v2";
                        return Reply(JsonConvert.SerializeObject(result));
                        break;

                    case "code":
                        // kirim file ?
                        string code = string.Empty;
                        try
                        {
                            code = gz["code"]["site"][path];
                        }
                        catch (Exception)
                        {

                        }
                        var fileName = "data.json.gz";


                        return Reply(code, "gzip");
                        break;
                    case "route":
                        // kirim file ?
                        //Global.Route = new List<RouterModel>();
                        List<Dictionary<string, dynamic>> urls = new List<Dictionary<string, dynamic>>();
                        result["api_url"] = null;
                        try
                        {
                            result["api_url"] = gz["site"]["config"]["api_url"];
                        }
                        catch (Exception e)
                        {

                        }
                        try
                        {
                            if (pages.Count > 0)
                            {
                                foreach (var item in pages)
                                {
                                    page = new Dictionary<string, dynamic>();
                                    page["id"] = item["id"];
                                    page["url"] = item["url"];
                                    urls.Add(page);
                                    //RouterModel RoutePage = new RouterModel();
                                    //RoutePage.id = item["id"];
                                    //RoutePage.Path = item["url"];
                                    //Global.Route.Add(RoutePage);
                                }
                            }
                        }
                        catch (Exception e)
                        {

                        }
                        Dictionary<string, dynamic> layout = new Dictionary<string, dynamic>();
                        layout["id"] = gz["layouts"][0]["id"];
                        layout["root"] = gz["layouts"][0]["content_tree"];
                        result["site"] = gz["site"];
                        result["site"]["api_url"] = "";
                        try
                        {
                            result["site"]["api_url"] = gz["site"]["config"]["api_url"];
                        }
                        catch (Exception)
                        {

                        }
                        result["layout"] = layout;
                        result["urls"] = urls;

                        return Reply(JsonConvert.SerializeObject(result), "application/octet-stream");
                        break;

                    case "page":
                        // kirim file ?
                        page = new Dictionary<string, dynamic>();
                        page = pages.FirstOrDefault(e => e["id"] == parts[1]);
                        result["id"] = page["id"];
                        result["root"] = page["content_tree"];
                        result["url"] = page["url"];
                        return Reply(JsonConvert.SerializeObject(result));
                        break;
                    case "pages":
                        List<Dictionary<string, dynamic>> pagesResult = new List<Dictionary<string, dynamic>>();
                        try
                        {
                            JArray jArray = param["ids"];
                            string[] ids = jArray.Select(jv => (string)jv).ToArray();
                            if (ids.Length > 0)
                            {
                                foreach (var item in ids)
                                {

                                    Dictionary<string, dynamic> pageItem = pages.FirstOrDefault(e => e["id"] == item);
                                    if (pageItem != null)
                                    {

                                        page = new Dictionary<string, dynamic>();
                                        page["id"] = page["id"];
                                        page["root"] = page["content_tree"];
                                        page["url"] = page["url"];
                                        pagesResult.Add(page);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        return Reply(JsonConvert.SerializeObject(pagesResult));
                        // belum ?
                        break;
                    case "comp":

                        Dictionary<string, dynamic> compResult = new Dictionary<string, dynamic>();
                        comps = gz["comps"].ToObject<List<Dictionary<string, dynamic>>>();
                        try
                        {

                            if (comps.Count > 0)
                            {
                                JArray jArray = param["ids"];
                                string[] ids = jArray.Select(jv => (string)jv).ToArray(); ;
                                if (ids.Length > 0)
                                {

                                    foreach (var item in ids)
                                    {

                                        Dictionary<string, dynamic> compItem = comps.FirstOrDefault(e => e["id"] == item);
                                        if (compItem != null)
                                        {
                                            compResult[item] = compItem;
                                            if (compItem.ContainsKey("content_tree"))
                                            {
                                                compResult[item] = compItem["content_tree"];
                                            }
                                            else
                                            {

                                            }

                                        }

                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            return Reply(JsonConvert.SerializeObject(ex));

                        }

                        return Reply(JsonConvert.SerializeObject(compResult));
                        break;
                    // belum ?
                    case "load.json":
                        string loadjson = string.Empty;
                        loadjson = preApi.GetContent("load.json");

                        return Reply(loadjson);
                        break;
                    case "load.js":
                        string url = "undefined";
                        try
                        {
                            url = param["url"];
                        }
                        catch (Exception ex)
                        {

                        }
                        string dev = string.Empty;
                        try
                        {
                            dev = param["dev"];
                        }
                        catch (Exception)
                        {

                        }
                        string loadjs = string.Empty;
                        if (string.IsNullOrEmpty(dev))
                        {
                            loadjs = preApi.GetContent("load.js.prod", url);
                        }
                        else
                        {
                            loadjs = preApi.GetContent("load.js.prod", url);

                        }

                        return Reply(loadjs);
                        break;

                }

            }
            catch (Exception ex)
            {
                result["Error"] = ex.Message;

            }
            //Dictionary<string, dynamic> web = new Dictionary<string, dynamic>();
            //string prasiFile = Global.Config["site"]["id"];
            string reply = string.Empty;
            return Reply(JsonConvert.SerializeObject(result));
        }
    }
}
