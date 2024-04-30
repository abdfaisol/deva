using Oracle.ManagedDataAccess.Client;

namespace deva.libs.db
{
    public static class Helper
    {
        public static Guid FlipEndian(this Guid guid)
        {
            var newBytes = new byte[16];
            var oldBytes = guid.ToByteArray();

            for (var i = 8; i < 16; i++)
                newBytes[i] = oldBytes[i];

            newBytes[3] = oldBytes[0];
            newBytes[2] = oldBytes[1];
            newBytes[1] = oldBytes[2];
            newBytes[0] = oldBytes[3];
            newBytes[5] = oldBytes[4];
            newBytes[4] = oldBytes[5];
            newBytes[6] = oldBytes[7];
            newBytes[7] = oldBytes[6];

            return new Guid(newBytes);
        }
        public static Dictionary<string, dynamic> GenerateRow(OracleDataReader data)
        {
            Dictionary<string, dynamic> row = new Dictionary<string, dynamic>();

            for (int i = 0; i < data.FieldCount; i++)
            {
                string col = data.GetName(i);
                Type type = data.GetFieldType(i);
                dynamic val;

                if (data[i] != DBNull.Value)
                {
                    if (type.FullName == "System.Byte[]")
                    {
                        Byte[] bit = (Byte[])data[i];
                        Guid guid = new Guid(bit).FlipEndian();
                        val = guid.ToString("N").ToUpper();
                    }
                    else if (type.FullName == "System.DateTime")
                    {
                        val = DateTime.Parse(data[i].ToString());
                    }
                    else if (type.FullName == "System.Int16" || type.FullName == "System.Int32" || type.FullName == "System.Int64")
                    {
                        val = Convert.ToInt64(data[i]);
                    }
                    else if (type.FullName == "System.Float" || type.FullName == "System.Decimal" || type.FullName == "System.Double")
                    {
                        val = Convert.ToDecimal(data[i]);
                    }
                    else
                    {
                        val = data[i].ToString();
                    }
                }
                else
                {
                    val = null;
                }

                if (!row.ContainsKey(col))
                    row.Add(col, val);
                else
                    row.Add(col + "01", val);
            }

            return row;
        }

    }
}
