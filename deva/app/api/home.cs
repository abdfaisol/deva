using deva.libs.page;
using deva.libs.router;
using System.Collections.Specialized;
using HttpRequest = Deva.HttpRequest;

namespace deva.app.page
{
    public class Home : RouterFunc
    {
        public override async Task<ApiRespon> OnReceivedRequest(HttpRequest req, NameValueCollection res)
        {
            string prasiFile = Global.Config["site"]["id"];
            return Reply($"{prasiFile}");
        }
    }
}
