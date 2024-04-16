using deva.libs.page;
using deva.libs.router;
using Newtonsoft.Json;
using System.Collections.Specialized;
using HttpRequest = Deva.HttpRequest;

namespace deva.app.page
{
    public class _web : RouterFunc
    {
        public override async Task<ApiRespon> OnReceivedRequest(HttpRequest req, NameValueCollection res)
        {

            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            List<Dictionary<string, dynamic>> pages = new List<Dictionary<string, dynamic>>();
            List<Dictionary<string, dynamic>> resultArray = new List<Dictionary<string, dynamic>>();
            try
            {

                //string QueryParameter = 
                string IdWeb = res["id"];
                string _ = res["_"];
                WebConfig g = Global.web[IdWeb];
                Dictionary<string, dynamic> cache = g.cache;
                string[] parts = _.Split("/");
                string idPage = parts[1];
                result["idPage"] = _;
                result["parts_0"] = parts[0];
                result["parts_1"] = idPage;
                result["params"] = SearchParameterQuery("halo");
                switch (parts[0])
                {
                    case "site":
                        // Querry parameter PROD Belumz
                        //"_proxy":{ "url":"/_proxy/*","args":[],"raw":true,"path":"app/srv/api/_proxy.ts","name":"_proxy"},"_notif":{ "url":"/_notif/:action/:token","args":["action", "data"],"raw":false,"path":"app/srv/api/_notif.ts","name":"_notif"},"_api_frm":{ "url":"/_api_frm","args":[],"raw":false,"path":"app/srv/api/_api_frm.ts","name":"_api_frm"},"_dbs":{ "url":"/_dbs/:dbName/:action","args":["dbName", "action"],"raw":false,"path":"app/srv/api/_dbs.ts","name":"_dbs"},"_upload":{ "url":"/_upload","args":["body"],"raw":true,"path":"app/srv/api/_upload.ts","name":"_upload"},"_web":{ "url":"/_web/:id/**","args":["id", "_"],"raw":false,"path":"app/srv/api/_web.ts","name":"_web"},"_prasi":{ "url":"/_prasi/**","args":[],"raw":false,"path":"app/srv/api/_prasi.ts","name":"_prasi"},"_deploy":{
                        //"url":"/_deploy","args":["action"],"raw":false,"path":"app/srv/api/_deploy.ts","name":"_deploy"
                        result = cache["site"].ToObject<Dictionary<string, dynamic>>();
                        return Reply(JsonConvert.SerializeObject(result));
                        break;

                    case "pages":
                        try
                        {
                            pages = cache["pages"].ToObject<List<Dictionary<string, dynamic>>>();
                            if (pages.Count > 0)
                            {
                                foreach (var item in pages)
                                {
                                    Dictionary<string, dynamic> page = new Dictionary<string, dynamic>();
                                    page["id"] = item["id"];
                                    page["url"] = item["url"];
                                    resultArray.Add(page);
                                }
                            }
                            return Reply(JsonConvert.SerializeObject(resultArray));

                        }
                        catch (Exception ex)
                        {

                        }
                        break;
                    case "page":
                        //result["Masuk"] = "PAGE";
                        try
                        {
                            //result["Masuk 1"] = cache["pages"];

                            //result["1.2"] = "AMAN";
                            pages = cache["pages"].ToObject<List<Dictionary<string, dynamic>>>();

                            //result["1.2"] = "AMAN";
                            //result["page"] = pages;
                            Dictionary<string, dynamic> hal = pages.FirstOrDefault(e => e["id"] == idPage);
                            //result["1.3"] = "AMAN";
                            /// result["error"] = result;
                            return Reply(JsonConvert.SerializeObject(hal));

                        }
                        catch (Exception ex)
                        {
                            result["error"] = ex.Message;

                        }
                        break;
                    case "npm-site":
                        string[] slicedPath = new string[parts.Length - 1];
                        Array.Copy(parts, 1, slicedPath, 0, parts.Length - 1);

                        string path = string.Join("/", slicedPath);
                        if (path == "site.js")
                        {
                            path = "index.js";
                        }
                        try
                        {
                            result = cache["npm"]["site"][path].ToObject<Dictionary<string, dynamic>>();
                            return Reply(JsonConvert.SerializeObject(result));

                        }
                        catch (Exception ex)
                        {
                            result["error"] = ex.Message;

                        }
                        break;
                    case "npm-page":
                        string PageId = parts[1];

                        try
                        {
                            if (cache["npm"]["site"][PageId] != null)
                            {
                                string[] slicePath = new string[parts.Length - 1];
                                Array.Copy(parts, 2, slicePath, 0, parts.Length - 1);
                                string pathNpm = string.Join("/", slicePath);
                                if (pathNpm == "page.js")
                                {
                                    pathNpm = "index.js";
                                }
                                result = cache["npm"]["pages"][PageId][pathNpm].ToObject<Dictionary<string, dynamic>>();
                                return Reply(JsonConvert.SerializeObject(result));

                            }
                            else
                            {

                            }

                        }
                        catch (Exception ex)
                        {
                            result["error"] = ex.Message;

                        }
                        break;
                    case "comp":

                        try
                        {
                            List<Dictionary<string, dynamic>> comps = new List<Dictionary<string, dynamic>>();
                            comps = cache["comps"].ToObject<List<Dictionary<string, dynamic>>>();
                            result = (Dictionary<string, dynamic>)comps.Where(e => e["id"] == parts[1]).FirstOrDefault();
                            return Reply(JsonConvert.SerializeObject(result));


                        }
                        catch (Exception ex)
                        {

                        }
                        break;
                    default:

                        return Reply("");
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
