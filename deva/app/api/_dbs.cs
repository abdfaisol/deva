using deva.libs.db;
using deva.libs.prasi;
using deva.libs.router;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using HttpRequest = Deva.HttpRequest;

namespace deva.app.page
{
    public class _dbs : RouterFunc
    {
        public override async Task<ApiRespon> OnReceivedRequest(HttpRequest req, NameValueCollection res)
        {
            PreApi preApi = new PreApi();
            Dictionary<string, dynamic> param = BodyParameter(req.Body);
            try
            {
                Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                List<Dictionary<string, dynamic>> resultList = new List<Dictionary<string, dynamic>>();
                PrismaModel paramPrisma = new PrismaModel();

                // Console.WriteLine(getTypeValue(param["params"]));
                if (getTypeValue(param["params"]) != "System.String")
                {
                    paramPrisma.param = param["params"].ToObject<List<Dictionary<string, dynamic>>>();
                    paramPrisma.table = param["table"];
                }
                paramPrisma.action = param["action"];
                OracleDeva db = new OracleDeva();
                try
                {
                    switch (paramPrisma.action)
                    {
                        case "queryRaw":
                            resultList = await db.QueryRaw($"{param["params"]}");
                            if (resultList.Count() > 0) return Reply(JsonConvert.SerializeObject(resultList));
                            return Reply(null);
                            break;
                        case "findFirst":
                            resultList = await db.QueryPrisma(paramPrisma);
                            result = resultList.FirstOrDefault();
                            // Console.WriteLine(getTypeValue(result));
                            return Reply(JsonConvert.SerializeObject(result));
                            break;
                        case "findMany":
                            resultList = await db.QueryPrisma(paramPrisma);
                            if (resultList.Count() > 0) return Reply(JsonConvert.SerializeObject(resultList));
                            return Reply(null);
                            break;
                        default:
                            return Reply(null);
                    }
                }
                catch (Exception ex)
                {
                    return Reply(JsonConvert.SerializeObject(ex));
                }
            }
            catch (Exception ex)
            {
                return Reply(JsonConvert.SerializeObject(ex));
            }
            return Reply("Ok");

        }
        static string generateWhere(Dictionary<string, dynamic> whereClause)
        {
            string result = string.Empty;
            List<string> sql = new List<string>();
            getWhere(whereClause, sql, "and");
            result = $" where {string.Join(" ", sql)}";
            return result;

        }

        static void getWhere(Dictionary<string, dynamic> whereClause, List<string> sql, string condition = "and")
        {
            foreach (KeyValuePair<string, dynamic> item in whereClause)
            {
                string typeValue = string.Empty;
                try
                {

                    Type type = item.Value.GetType();
                    typeValue = type.ToString();
                    // Console.WriteLine(item.Value);
                    // Console.WriteLine(item.Value.GetType());
                }
                catch (Exception ex)
                {
                }
                switch (item.Key.ToLower())
                {
                    case "and":
                        if (typeValue == "Newtonsoft.Json.Linq.JArray")
                        {
                            // Console.WriteLine("MASU");
                            // jika and adalah array
                            List<Dictionary<string, dynamic>> val = new List<Dictionary<string, dynamic>>();
                            val = item.Value.ToObject<List<Dictionary<string, dynamic>>>();

                            // Console.WriteLine("masuk");
                            // Console.WriteLine(val.Count);
                            AddSeperator(condition, sql);
                            sql.Add($"(");
                            for (int i = 0; i < val.Count; i++)
                            {
                                var sub = val[i];
                                string separatorWhere = "and";
                                if (i == 0)
                                {
                                    separatorWhere = string.Empty;
                                }
                                getWhere(sub, sql, separatorWhere);
                            }
                            sql.Add($")");
                        }
                        else
                        {
                            throw new Exception("Property 'AND' only accepts array values");
                        }
                        break;
                    case "or":

                        if (typeValue == "Newtonsoft.Json.Linq.JArray")
                        {
                            // jika and adalah array
                            List<Dictionary<string, dynamic>> val = new List<Dictionary<string, dynamic>>();
                            val = item.Value.ToObject<List<Dictionary<string, dynamic>>>();

                            AddSeperator(condition, sql);
                            sql.Add($"(");
                            for (int i = 0; i < val.Count; i++)
                            {
                                var sub = val[i];
                                string separatorWhere = "or";
                                if (i == 0)
                                {
                                    separatorWhere = string.Empty;
                                }
                                getWhere(sub, sql, separatorWhere);
                            }
                            sql.Add($")");
                        }
                        else
                        {
                            throw new Exception("Property 'OR' only accepts array values");
                        }
                        break;
                    case "not":

                        if (typeValue == "Newtonsoft.Json.Linq.JArray")
                        {
                            throw new Exception("Property 'NOT' only accepts object values");
                        }
                        else
                        {
                            List<string> sqlOr = new List<string>();
                            Dictionary<string, dynamic> val = new Dictionary<string, dynamic>();
                            val = item.Value.ToObject<Dictionary<string, dynamic>>();
                            AddSeperator(condition, sql);
                            //sql.Add($"NOT (");
                            getWhere(val, sqlOr, "and");
                            // Console.WriteLine(sqlOr);
                            if (sqlOr.Count() > 0)
                            {
                                // Console.WriteLine("MASUK SINI?");
                                sql.Add($"NOT (");
                                foreach (var or in sqlOr)
                                {
                                    sql.Add(or);
                                }
                                sql.Add($")");
                            }
                        }
                        break;
                    default:
                        if (item.Value is string)
                        {
                            if (sql.Count > 0)
                            {
                                sql.Add($"{condition}");
                            }
                            sql.Add($"\"{item.Key}\" = '{item.Value}'");
                        }
                        else if (item.Value == null)
                        {
                            // jika value null atau empty
                            Dictionary<string, dynamic> valueEmpty = new Dictionary<string, dynamic>();
                            valueEmpty["equals"] = null;
                            string val = getValueWhere(item.Key, valueEmpty);
                            AddSeperator(condition, sql);
                            sql.Add($"{val}");
                        }
                        else
                        {
                            // jika value object
                            string val = getValueWhere(item.Key, item.Value.ToObject<Dictionary<string, dynamic>>());
                            AddSeperator(condition, sql);
                            // Console.WriteLine(val);
                            sql.Add($"{val}");
                        }
                        break;
                }

            }
        }
        static string getTypeValue(object val)
        {
            string typeValue = string.Empty;
            try
            {
                if (val != null)
                {
                    Type type = val.GetType();
                    typeValue = type.ToString();
                }
                else
                {
                    typeValue = "NULL";
                }
            }
            catch (Exception ex)
            {
            }
            return typeValue;
        }
        static void AddSeperator(string condition, List<string> sql)
        {
            IEnumerable<string> sqlFilter = sql.Where(e => e != "(" || e != ")" || e != $"NOT (");
            //// Console.WriteLine(sqlFilter.Count());
            //foreach (var item in sqlFilter)
            //{
            //    // Console.WriteLine($"{item}");
            //}
            if (sqlFilter.Count() > 0)
            {
                sql.Add($"{condition}");
            }
        }
        static string getValueWhere(string keys, Dictionary<string, dynamic> whereClause)
        {
            string result = string.Empty;
            string value = string.Empty;
            string condition = string.Empty;
            bool sensitive = getSensitive(whereClause);
            foreach (KeyValuePair<string, dynamic> item in whereClause)
            {
                string typeValue = string.Empty;
                try
                {

                    Type type = item.Value.GetType();
                    typeValue = type.ToString();
                }
                catch (Exception ex)
                {
                }
                switch (item.Key)
                {
                    case "endsWith":
                        result = valueSensitive(keys, "LIKE", $"'%{item.Value}'", sensitive);
                        break;
                    case "startsWith":
                        result = valueSensitive(keys, "LIKE", $"'{item.Value}%'", sensitive);
                        break;
                    case "not":
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            result = valueSensitive(keys, "!=", $"'{item.Value}'", sensitive);
                        }
                        else
                        {
                            result = valueSensitive(keys, "is not null", $"", false);
                        }
                        break;
                    case "gt":
                        result = valueSensitive(keys, ">", $"'{item.Value}%'", false);
                        break;
                    case "in":
                        result = valueSensitive(keys, "IN", $"{getValueIn(item.Value)}", false);
                        break;
                    case "notin":
                        result = valueSensitive(keys, "NOT IN", $"{getValueIn(item.Value)}", false);
                        break;
                    case "gte":
                        result = valueSensitive(keys, ">=", $"'{item.Value}%'", false);
                        break;
                    case "lt":
                        result = valueSensitive(keys, "<", $"'{item.Value}%'", false);
                        break;
                    case "lte":
                        result = valueSensitive(keys, "<=", $"'{item.Value}%'", false);
                        break;
                    case "contains":
                        result = valueSensitive(keys, "LIKE", $"'%{item.Value}%'", sensitive);
                        break;
                    case "equals":
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            result = valueSensitive(keys, "=", $"'{item.Value}'", sensitive);
                        }
                        else
                        {
                            result = valueSensitive(keys, "is null", $"", false);
                        }
                        break;
                    default:
                        if (whereClause.Count == 1)
                        {

                            if (!string.IsNullOrEmpty(item.Value))
                            {
                                result = valueSensitive(keys, "=", $"'{item.Value}'", false);
                            }
                            else
                            {
                                result = valueSensitive(keys, "is null", $"", false);
                            }
                        }
                        break;
                }

            }
            return result;
        }
        static string getValueIn(JArray val)
        {
            string result = string.Empty;
            string typeValue = string.Empty;
            try
            {

                Type type = val.GetType();
                typeValue = type.ToString();
            }
            catch (Exception ex)
            {
            }
            List<string> valueIn = new List<string>();
            if (typeValue == "Newtonsoft.Json.Linq.JArray")
            {
                try
                {

                    List<object> valTemp = new List<object>();
                    valTemp = val.ToObject<List<object>>();
                    for (int i = 0; i < valTemp.Count; i++)
                    {
                        if (valTemp[i].GetType() == typeof(string))
                        {
                            valueIn.Add($"\'{valTemp[i]}\'");
                        }
                        else
                        {
                            valueIn.Add($"{valTemp[i]}");
                        }

                    }
                }
                catch (Exception ex)
                {
                }
            }
            result = $"()";
            if (valueIn.Count > 0)
            {
                result = $"({string.Join(",", valueIn)})";
            }
            return result;
        }
        static string valueSensitive(string keys, string condition, string value, bool sensitive = false)
        {
            if (sensitive)
            {
                return $"LOWER(\"{keys}\") {condition} LOWER({value})";
            }
            return $"\"{keys}\" {condition} {value}";
        }
        static bool getSensitive(Dictionary<string, dynamic> res)
        {
            try
            {
                if (res["mode"] == "insensitive") return true;
            }
            catch (Exception ex) { }
            return false;
        }
    }
}
