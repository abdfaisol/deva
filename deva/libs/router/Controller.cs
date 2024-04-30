using deva.app.page;
using HttpRequest = Deva.HttpRequest;
namespace deva.libs.router
{
    public static class Controller
    {

        public static List<RouterModel> GetController(HttpRequest request)
        {
            List<RouterModel> res = new List<RouterModel>();
            res.Add(new RouterModel() { Path = "/_web/:id/*", Model = new _web() });
            res.Add(new RouterModel() { Path = "/_dbs/*", Model = new _dbs() });
            res.Add(new RouterModel() { Path = "/_prasi/*", Model = new _prasi() });
            res.Add(new RouterModel() { Path = "/_img/*", Model = new _img() });
            res.Add(new RouterModel() { Path = "/_file/*", Model = new _file() });
            res.Add(new RouterModel() { Path = "/_upload", Model = new _upload() });
            res.Add(new RouterModel() { Path = "/_proxy/*", Model = new _proxy() });
            return res;
        }
    }
}
