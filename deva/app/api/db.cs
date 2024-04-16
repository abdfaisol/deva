using deva.libs.router;
using System.Collections.Specialized;
using HttpRequest = Deva.HttpRequest;

namespace deva.app.page
{
    public class Db : RouterFunc
    {
        public override async Task<ApiRespon> OnReceivedRequest(HttpRequest req, NameValueCollection res)
        {
            return Reply("HALO CONTROLLER");
        }
    }
}
