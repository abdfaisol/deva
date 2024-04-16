namespace deva.libs.prasi
{
    public class PreApi
    {
        public string GetContent(string type, string url = "localhost:2001")
        {
            string result = string.Empty;
            string mainFile = string.Empty;
            switch (type)
            {
                case "load.json":
                    // belum
                    mainFile = @"
{apiEntry: {
            ""_img"": {
                ""url"": ""/_img/**"",
                ""args"": [],
                ""raw"": false,
                ""path"": ""app/srv/api/_img.ts"",
                ""name"": ""_img""
            },
            ""_proxy"": {
                ""url"": ""/_proxy/*"",
                ""args"": [],
                ""raw"": true,
                ""path"": ""app/srv/api/_proxy.ts"",
                ""name"": ""_proxy""
            },
            ""_notif"": {
                ""url"": ""/_notif/:action/:token"",
                ""args"": [""action"", ""data""],
                ""raw"": false,
                ""path"": ""app/srv/api/_notif.ts"",
                ""name"": ""_notif""
            },
            ""_api_frm"": {
                ""url"": ""/_api_frm"",
                ""args"": [],
                ""raw"": false,
                ""path"": ""app/srv/api/_api_frm.ts"",
                ""name"": ""_api_frm""
            },
            ""_dbs"": {
                ""url"": ""/_dbs/:dbName/:action"",
                ""args"": [""dbName"", ""action""],
                ""raw"": false,
                ""path"": ""app/srv/api/_dbs.ts"",
                ""name"": ""_dbs""
            },
            ""_upload"": {
                ""url"": ""/_upload"",
                ""args"": [""body""],
                ""raw"": true,
                ""path"": ""app/srv/api/_upload.ts"",
                ""name"": ""_upload""
            },
            ""_prasi"": {
                ""url"": ""/_prasi/**"",
                ""args"": [],
                ""raw"": false,
                ""path"": ""app/srv/api/_prasi.ts"",
                ""name"": ""_prasi""
            },
            ""_deploy"": {
                ""url"": ""/_deploy"",
                ""args"": [""action""],
                ""raw"": false,
                ""path"": ""app/srv/api/_deploy.ts"",
                ""name"": ""_deploy""
            },
            ""_file"": {
                ""url"": ""/_file/**"",
                ""args"": [],
                ""raw"": false,
                ""path"": ""app/srv/api/_file.ts"",
                ""name"": ""_file""
            }
        }}
";
                    break;

                case "load.js.dev":
                    // belum
                    mainFile = @"
  const w = window;
  if (!w.prasiApi) {
    w.prasiApi = {};
  }
  w.prasiApi[url] = {
        apiEntry: {
            ""_img"": {
                ""url"": ""/_img/**"",
                ""args"": [],
                ""raw"": false,
                ""path"": ""app/srv/api/_img.ts"",
                ""name"": ""_img""
            },
            ""_proxy"": {
                ""url"": ""/_proxy/*"",
                ""args"": [],
                ""raw"": true,
                ""path"": ""app/srv/api/_proxy.ts"",
                ""name"": ""_proxy""
            },
            ""_notif"": {
                ""url"": ""/_notif/:action/:token"",
                ""args"": [""action"", ""data""],
                ""raw"": false,
                ""path"": ""app/srv/api/_notif.ts"",
                ""name"": ""_notif""
            },
            ""_api_frm"": {
                ""url"": ""/_api_frm"",
                ""args"": [],
                ""raw"": false,
                ""path"": ""app/srv/api/_api_frm.ts"",
                ""name"": ""_api_frm""
            },
            ""_dbs"": {
                ""url"": ""/_dbs/:dbName/:action"",
                ""args"": [""dbName"", ""action""],
                ""raw"": false,
                ""path"": ""app/srv/api/_dbs.ts"",
                ""name"": ""_dbs""
            },
            ""_upload"": {
                ""url"": ""/_upload"",
                ""args"": [""body""],
                ""raw"": true,
                ""path"": ""app/srv/api/_upload.ts"",
                ""name"": ""_upload""
            },
            ""_prasi"": {
                ""url"": ""/_prasi/**"",
                ""args"": [],
                ""raw"": false,
                ""path"": ""app/srv/api/_prasi.ts"",
                ""name"": ""_prasi""
            },
            ""_deploy"": {
                ""url"": ""/_deploy"",
                ""args"": [""action""],
                ""raw"": false,
                ""path"": ""app/srv/api/_deploy.ts"",
                ""name"": ""_deploy""
            },
            ""_file"": {
                ""url"": ""/_file/**"",
                ""args"": [],
                ""raw"": false,
                ""path"": ""app/srv/api/_file.ts"",
                ""name"": ""_file""
            }
        },
    }
})();
";
                    break;
                case "load.js.prod":
                    mainFile = @"
  const w = window;
  if (!w.prasiApi) {
    w.prasiApi = {};
  }
  w.prasiApi[url] = {
        apiEntry: {
            ""_img"": {
                ""url"": ""/_img/**"",
                ""args"": [],
                ""raw"": false,
                ""path"": ""app/srv/api/_img.ts"",
                ""name"": ""_img""
            },
            ""_proxy"": {
                ""url"": ""/_proxy/*"",
                ""args"": [],
                ""raw"": true,
                ""path"": ""app/srv/api/_proxy.ts"",
                ""name"": ""_proxy""
            },
            ""_notif"": {
                ""url"": ""/_notif/:action/:token"",
                ""args"": [""action"", ""data""],
                ""raw"": false,
                ""path"": ""app/srv/api/_notif.ts"",
                ""name"": ""_notif""
            },
            ""_api_frm"": {
                ""url"": ""/_api_frm"",
                ""args"": [],
                ""raw"": false,
                ""path"": ""app/srv/api/_api_frm.ts"",
                ""name"": ""_api_frm""
            },
            ""_dbs"": {
                ""url"": ""/_dbs/:dbName/:action"",
                ""args"": [""dbName"", ""action""],
                ""raw"": false,
                ""path"": ""app/srv/api/_dbs.ts"",
                ""name"": ""_dbs""
            },
            ""_upload"": {
                ""url"": ""/_upload"",
                ""args"": [""body""],
                ""raw"": true,
                ""path"": ""app/srv/api/_upload.ts"",
                ""name"": ""_upload""
            },
            ""_prasi"": {
                ""url"": ""/_prasi/**"",
                ""args"": [],
                ""raw"": false,
                ""path"": ""app/srv/api/_prasi.ts"",
                ""name"": ""_prasi""
            },
            ""_deploy"": {
                ""url"": ""/_deploy"",
                ""args"": [""action""],
                ""raw"": false,
                ""path"": ""app/srv/api/_deploy.ts"",
                ""name"": ""_deploy""
            },
            ""_file"": {
                ""url"": ""/_file/**"",
                ""args"": [],
                ""raw"": false,
                ""path"": ""app/srv/api/_file.ts"",
                ""name"": ""_file""
            }
        },
    }
})();
";
                    break;
            }
            result = @"

(() => {
  const baseurl = new URL(location.href);
  baseurl.pathname = '';
  const url = " + url;
            result += " || baseurl.toString();" + mainFile;
            return result;

        }
    }
}
