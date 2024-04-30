namespace deva.libs.lodash
{
    public class Lodash
    {
        public Dictionary<string, dynamic> findObject(string keys, Dictionary<string, dynamic> data)
        {
            Dictionary<string, dynamic> res = null;
            if (data.ContainsKey(keys))
            {
                res = data[keys].ToObject<Dictionary<string, dynamic>>();
            }
            return res;
        }
    }
}
