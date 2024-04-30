using deva.libs.lodash;
using deva.libs.page;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace deva.libs.db
{
    public class SchemaRel
    {
        public List<string> fields { get; set; }
        public List<RelationAs> tables { get; set; }
    }
    public class RelationAs
    {
        public string table { get; set; }
        public string like { get; set; }
    }

    public class ResponTypeTable
    {
        public string type { get; set; }
        public string column { get; set; }
        public string r_column { get; set; }
        public string table { get; set; }
        public string r_table { get; set; }
        public List<Dictionary<string, dynamic>> param { get; set; }
    }
    public class PrismaModel
    {
        public string action { get; set; }
        public string table { get; set; }
        public List<Dictionary<string, dynamic>> param { get; set; }
        public Dictionary<string, dynamic> getSelect()
        {
            Dictionary<string, dynamic> result = null;
            Dictionary<string, dynamic> ttmp = new Dictionary<string, dynamic>();
            try
            {
                ttmp = param.Where(e => e.ContainsKey("select")).FirstOrDefault();
                if (ttmp != null) result = ttmp["select"].ToObject<Dictionary<string, dynamic>>();
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
        public Dictionary<string, dynamic> getWhere()
        {
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            Dictionary<string, dynamic> ttmp = new Dictionary<string, dynamic>();
            try
            {
                ttmp = param.Where(e => e.ContainsKey("where")).FirstOrDefault();
                if (ttmp != null)
                {
                    ttmp = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JsonConvert.SerializeObject(param[0]));
                    Console.WriteLine("======");
                    Console.WriteLine(JsonConvert.SerializeObject(ttmp));
                    if (ttmp != null)
                        result = ttmp["where"].ToObject<Dictionary<string, dynamic>>();
                    Console.WriteLine("======");
                    Console.WriteLine(JsonConvert.SerializeObject(result));
                    Console.WriteLine("======");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        public Dictionary<string, dynamic> getInclude()
        {
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            Dictionary<string, dynamic> ttmp = new Dictionary<string, dynamic>();

            Console.WriteLine(JsonConvert.SerializeObject(param));
            try
            {
                ttmp = param.Where(e => e.ContainsKey("include")).FirstOrDefault();
                if (ttmp != null)
                    result = ttmp["include"].ToObject<Dictionary<string, dynamic>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = null;
            }
            return result;
        }
        public Dictionary<string, dynamic> getOrder()
        {
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            Dictionary<string, dynamic> ttmp = new Dictionary<string, dynamic>();
            try
            {
                ttmp = param.Where(e => e.ContainsKey("orderBy")).FirstOrDefault();
                if (ttmp != null)
                {

                    List<string> orderByColumn = new List<string>();
                    string tipeValueOrder = getTypeValue(ttmp["orderBy"]);
                    result["type"] = tipeValueOrder;
                    if (tipeValueOrder == "Newtonsoft.Json.Linq.JArray")
                    {
                        // Array
                        JArray orders = new JArray();
                        orders = ttmp["orderBy"];

                        result["data"] = orders;

                    }
                    else
                    {
                        result["data"] = ttmp["orderBy"].ToObject<Dictionary<string, dynamic>>();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public SchemaRel getFieldsRelation()
        {
            SchemaRel result = new SchemaRel();

            List<string> sql = new List<string>();
            Lodash lodash = new Lodash();

            Dictionary<string, dynamic> schema = new Dictionary<string, dynamic>();
            schema = Global.SchemaDb[table].ToObject<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> fields = lodash.findObject("fields", schema);
            Dictionary<string, dynamic> relations = lodash.findObject("relations", schema);
            return result;
        }
        private static string getTypeValue(object val)
        {
            string typeValue = string.Empty;
            try
            {

                Type type = val.GetType();
                typeValue = type.ToString();
            }
            catch (Exception ex)
            {
            }
            return typeValue;
        }

    }
}
