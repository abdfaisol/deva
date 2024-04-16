using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace deva.libs
{
    public class DevaMatcher
    {

        public Action<string> Logger;

        private string _Header = "[UrlParser] ";
        //
        // Summary:
        //     Match a URL against a pattern. For example, match URL /v1.0/something/else/32
        //     against pattern /{v}/something/else/{id}. If a match exists, vals will contain
        //     keys name 'v' and 'id', and the associated values from the supplied URL.
        //
        // Parameters:
        //   url:
        //     The URL to evaluate.
        //
        //   pattern:
        //     The pattern used to evaluate the URL.
        //
        //   vals:
        //     Dictionary containing keys and values.
        //
        // Returns:
        //     True if matched.
        public MatcherModel NormalizeRoute(string route)
        {
            MatcherModel res = new MatcherModel();
            res.Pattern = route;
            return res;
        }
        public RouteModel createRoute(MatcherModel res)
        {
            List<string> tokens = new List<string>();
            RouteModel result = new RouteModel();
            List<ParamModel> param = new List<ParamModel>();
            string regex = @":([a-zA-Z_$][a-zA-Z0-9_$]*)|\*\*|\*|\(|\)|[^:*()]+/g";
            string regexpSource = "";
            if (!res.Pattern.StartsWith("/")) res.Pattern = "/" + res.Pattern;
            List<MatchesModel> matches = getAllMatches(regex, res.Pattern);
            foreach (MatchesModel item in matches)
            {
                string rule = "";
                if (!string.IsNullOrEmpty(item.ParamName))
                {
                    rule = res.Rules[item.ParamName];
                }
                else if (item.Token == "**")
                {
                    rule = @"([\\s\\S]*)";
                    item.ParamName = "splat";
                }
                else if (item.Token == "*")
                {
                    rule = @"([\\s\\S]*?)";
                    item.ParamName = "splat";
                }
                else if (item.Token == "(")
                {
                    regexpSource += "(?:";
                }
                else if (item.Token == ")")
                {
                    regexpSource += ")?";
                }
                else
                { // anything else
                    regexpSource += escapeSource(item.Token);
                }
                if (!string.IsNullOrEmpty(item.ParamName))
                {
                    regexpSource += rule;

                    ParamModel paramItem = new ParamModel();
                    paramItem.ParamName = item.ParamName;
                    paramItem.Rule = rule;
                    param.Add(paramItem);
                }
                tokens.Add(item.Token);
            }
            bool captureRemaining = tokens[tokens.Count - 1] != "*";
            string ending = captureRemaining ? "" : "$";
            regexpSource = @"^" + regexpSource + @"/*" + ending;
            result.Tokens = tokens;
            result.RegexpSource = regexpSource;
            result.Params = param;
            List<string> ParamsName = new List<string>();
            if (param.Count > 0)
            {
                foreach (var item in param)
                {
                    ParamsName.Add(item.ParamName);
                }
            }
            result.paramNames = ParamsName;
            return result;
        }
        public RouteModel getRoute(string path)
        {
            RouteModel result = new RouteModel();
            MatcherModel route = new MatcherModel();
            route = NormalizeRoute(path);
            result = createRoute(route);
            return result;
        }
        public string escapeRegExp(string res)
        {
            string result = Regex.Replace(res, @"[.*+?^${}()|[\]\\]", @"\\$&");
            return result;

        }
        public string escapeSource(string source)
        {
            string val = escapeRegExp(source);
            string result = Regex.Replace(val, @"\/+", @"/+");
            return result;
        }
        public List<MatchesModel> getAllMatches(string regex, string pattern)
        {
            List<MatchesModel> result = new List<MatchesModel>();
            Regex rx = new Regex(regex);
            MatchCollection matches = rx.Matches(pattern);
            foreach (Match match in matches)
            {
                MatchesModel res = new MatchesModel();

                GroupCollection groups = match.Groups;
                try
                {
                    res.Token = groups[0].ToString();
                }
                catch (Exception ex)
                {

                }
                try
                {
                    res.ParamName = groups[1].ToString();
                }
                catch (Exception ex)
                {

                }
                result.Add(res);
            }
            return result;
        }
        public bool MatchPattern(string url, string pattern, out NameValueCollection vals)
        {
            vals = null;
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentNullException("pattern");
            }
            if (!pattern.StartsWith("/")) pattern = "/" + pattern;
            RouteModel route = new RouteModel();
            route = getRoute(pattern);
            Match match = Regex.Match(url, route.RegexpSource);
            if (!match.Success)
            {
                return false;
            }
            //Console.WriteLine(route.RegexpSource);

            return true;
        }
        public bool FindMatch(string url, string pattern, out NameValueCollection vals)
        {
            vals = null;
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentNullException("pattern");
            }

            vals = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            string[] array = url.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string[] array2 = pattern.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);


            if (array.Length != array2.Length || pattern.EndsWith("*"))
            {
                if (pattern.EndsWith("*") && !pattern.Contains(":"))
                {
                    string[] arr = pattern.Split(new char[1] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Count() > 0)
                    {
                        string path = arr[0];
                        if (url.StartsWith(path))
                        {
                            Logger?.Invoke(_Header + "_" + ": " + url.Replace(path, ""));
                            vals.Add("_", url.Replace(path, ""));
                            return true;
                        }
                    }
                }
                else if (pattern.EndsWith("*") && pattern.Contains(":"))
                {

                    string[] arr = pattern.Split(new char[1] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Count() > 0)
                    {
                        string path = arr[0];
                        bool isValidate = true;
                        if (path.Contains(":"))
                        {
                            string urlWildCard = url;
                            if (urlWildCard.EndsWith("/"))
                            {
                                urlWildCard = urlWildCard.Remove(urlWildCard.Length - 1);
                            }
                            if (path.EndsWith("/"))
                            {
                                path = path.Remove(path.Length - 1);
                            }
                            int num2 = pattern.LastIndexOf('*');
                            array = urlWildCard.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                            array2 = path.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                            string[] arr2 = new string[array2.Length];
                            for (int i = 0; i < array.Length; i++)
                            {
                                if (i <= array2.Length - 1)
                                {
                                    arr2[i] = array[i];
                                }
                            }
                            array = arr2;
                            if (array.Length == array2.Length)
                            {
                                for (int i = 0; i < array.Length; i++)
                                {
                                    string text = null;
                                    try
                                    {
                                        text = ExtractParameter(array2[i]);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    if (string.IsNullOrEmpty(text))
                                    {
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Logger?.Invoke(_Header + text.Replace(":", "") + ": " + array[i]);
                                            vals.Add(text.Replace(":", ""), array[i]);
                                            urlWildCard = urlWildCard.Replace(array[i], text);
                                        }
                                        catch (Exception ex) { }

                                    }
                                }
                                url = urlWildCard;
                            }
                            else
                            {
                                isValidate = false;
                            }

                        }

                        if (isValidate)
                        {
                            if (url.StartsWith(path))
                            {
                                string valueWildCard = url.Replace(path, "");
                                if (!string.IsNullOrEmpty(valueWildCard))
                                {
                                    try
                                    {

                                        string urlWildCard = url.Replace(valueWildCard, "");
                                        if (valueWildCard.StartsWith("/")) valueWildCard = valueWildCard.Substring(1);
                                        Logger?.Invoke(_Header + "_" + ": " + valueWildCard);
                                        vals.Add("_", valueWildCard);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                return true;
                            }

                        }
                        else
                        {
                            return false;
                        }
                    }

                }
                return false;
            }
            else
            {

                for (int i = 0; i < array.Length; i++)
                {
                    string text = ExtractParameter(array2[i]);
                    if (string.IsNullOrEmpty(text))
                    {
                        if (!array[i].Equals(array2[i]))
                        {
                            Logger?.Invoke(_Header + "content mismatch at position " + i);
                            vals = null;
                            return false;
                        }
                    }
                    else
                    {
                        Logger?.Invoke(_Header + text.Replace(":", "") + ": " + array[i]);
                        vals.Add(text.Replace(":", ""), array[i]);
                    }
                }
            }
            Logger?.Invoke(_Header + "match detected, " + vals.Count + " parameters extracted");
            return true;
        }

        private string ExtractParameter(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentNullException("pattern");
            }

            if (pattern.Contains(":"))
            {
                int num = pattern.IndexOf(':');
                // int num2 = pattern.LastIndexOf('}');
                if (num >= 0)
                {
                    return pattern;
                }
            }

            return null;
        }

        private string ExtractParameterValue(string url, string pattern)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentNullException("pattern");
            }

            int num = pattern.IndexOf('{');
            int num2 = pattern.LastIndexOf('}');
            if (num2 - 1 > num)
            {
                return url.Substring(num, num2 - 1);
            }

            return "";
        }
    }
}
