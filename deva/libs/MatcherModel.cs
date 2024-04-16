namespace deva.libs
{
    public class MatcherModel
    {
        public string Pattern { get; set; }
        public Dictionary<string, dynamic> Rules { get; set; }
    }
    public class RouteModel
    {
        public string RegexpSource { get; set; }
        public List<string> Tokens { get; set; }
        public List<ParamModel> Params { get; set; }
        public List<string> paramNames { get; set; }
    }
    public class RegexParamsModel
    {
        public string ParamName { get; set; }
        public string Rule { get; set; }
    }
    public class ParamModel
    {
        public string ParamName { get; set; }
        public string Rule { get; set; }
    }
    public class MatchesModel
    {
        public string ParamName { get; set; }
        public string Token { get; set; }
    }
}
