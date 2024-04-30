using deva.libs.page;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace deva.libs.db
{
    public class OracleDeva
    {
        private string connectionOracle = Global.ConnectionDb;

        public OracleDeva()
        {

        }
        public async Task<List<Dictionary<string, dynamic>>> QueryPrisma(PrismaModel param, string con = "")
        {
            List<Dictionary<string, dynamic>> res = new List<Dictionary<string, dynamic>>();
            string select = generateSelect(ref param);
            string where = generateWhere(ref param);
            string order = generateOrder(ref param);
            res = await QueryRaw($"{select} {where} {order}");
            try
            {
                Dictionary<string, dynamic> includes = new Dictionary<string, dynamic>();
                includes = param.getInclude();
                if (includes != null)
                    if (includes.Count > 0)
                    {
                        res = await generateIncludes(res, includes, param.table);
                    }

            }
            catch (Exception ex) { }
            return res;
        }
        public async Task<List<Dictionary<string, dynamic>>> GetRelationTable(string con = "")
        {
            List<Dictionary<string, dynamic>> res = new List<Dictionary<string, dynamic>>();
            string sql = $"""
                SELECT 
                  c.table_name, 
                  cc.table_name AS r_table_name, 
                cols.column_name, 
                cc2.column_name as r_column_name, 
                  c.constraint_name, 
                  c.r_constraint_name 
                FROM 
                  user_constraints c 
                  JOIN user_constraints cc ON c.r_constraint_name = cc.constraint_name 
                  JOIN user_cons_columns cols ON c.constraint_name = cols.constraint_name 
                  JOIN user_cons_columns cc2 ON cc.constraint_name = cc2.constraint_name 
                WHERE 
                  c.constraint_type = 'R'
                ORDER BY 
                  c.table_name asc
                
                """;
            res = await QueryRaw($"{sql}");
            return res;
        }
        public async Task<List<Dictionary<string, dynamic>>> GetAllTableView(string con = "")
        {
            List<Dictionary<string, dynamic>> res = new List<Dictionary<string, dynamic>>();
            string sql = $"""
                SELECT view_name as TABLE_NAME FROM user_views
                UNION ALL
                SELECT TABLE_NAME FROM USER_TABLES
                """;
            res = await QueryRaw($"{sql}");
            return res;
        }
        public async Task<List<Dictionary<string, dynamic>>> GetAllTable(string con = "")
        {
            List<Dictionary<string, dynamic>> res = new List<Dictionary<string, dynamic>>();
            string sql = $"""
                SELECT TABLE_NAME FROM USER_TABLES
                """;
            res = await QueryRaw($"{sql}");
            return res;
        }
        public async Task<List<Dictionary<string, dynamic>>> GetAllView(string con = "")
        {
            List<Dictionary<string, dynamic>> res = new List<Dictionary<string, dynamic>>();
            string sql = $"""
                SELECT view_name as TABLE_NAME FROM user_views
                """;
            res = await QueryRaw($"{sql}");
            return res;
        }
        public async Task<List<Dictionary<string, dynamic>>> GetPrimaryKeyTables(string con = "")
        {
            List<Dictionary<string, dynamic>> res = new List<Dictionary<string, dynamic>>();
            string sql = $"""
                SELECT cols.table_name,
                       cols.column_name
                FROM   all_constraints cons,
                       all_cons_columns cols
                WHERE  cons.constraint_type = 'P'
                       AND cols.table_name IN (SELECT table_name AS
                                               FROM   user_tables)
                       AND cons.constraint_name = cols.constraint_name
                       AND cons.owner = cols.owner
                ORDER  BY cols.table_name,
                          cols.position;
                """;
            res = await QueryRaw($"{sql}");
            return res;
        }
        public async Task<List<Dictionary<string, dynamic>>> GetAllFieldsTables(string con = "")
        {
            List<Dictionary<string, dynamic>> res = new List<Dictionary<string, dynamic>>();
            string sql = $"""
                SELECT ut.table_name,
                       ut.column_name,
                       ut.data_type,
                       ut.data_length,
                       pk.constraint_type
                FROM   user_tab_columns ut
                       LEFT JOIN (SELECT cols.table_name,
                                         cols.column_name,
                                         'PK' AS constraint_type
                                  FROM   all_constraints cons,
                                         all_cons_columns cols
                                  WHERE  cons.constraint_type = 'P'
                                         AND cols.table_name IN (SELECT table_name AS
                                                                 FROM   user_tables)
                                         AND cons.constraint_name = cols.constraint_name
                                         AND cons.owner = cols.owner
                                  ORDER  BY cols.table_name,
                                            cols.position) pk
                              ON pk.table_name = ut.table_name
                                 AND pk.column_name = ut.column_name
                """;
            res = await QueryRaw($"{sql}");
            return res;
        }
        public async Task<List<Dictionary<string, dynamic>>> GetRelationTables(List<string> table, string con = "")
        {
            string tables = string.Join(",", table);
            List<Dictionary<string, dynamic>> res = new List<Dictionary<string, dynamic>>();
            string sql = $"""
                SELECT 
                  c.table_name, 
                  cc.table_name AS r_table_name, 
                  cc2.column_name, 
                  cols.column_name as r_column_name, 
                  c.constraint_name, 
                  c.r_constraint_name 
                FROM 
                  user_constraints c 
                  JOIN user_constraints cc ON c.r_constraint_name = cc.constraint_name 
                  JOIN user_cons_columns cols ON c.constraint_name = cols.constraint_name 
                  JOIN user_cons_columns cc2 ON cc.constraint_name = cc2.constraint_name 
                WHERE 
                  c.constraint_type = 'R' 
                  AND c.table_name IN ({table}) 
                ORDER BY 
                  c.table_name asc
                
                """;
            res = await QueryRaw($"{sql}");
            return res;
        }
        public async Task<List<Dictionary<string, dynamic>>> GetRelationHasMany(string con = "")
        {
            List<Dictionary<string, dynamic>> res = new List<Dictionary<string, dynamic>>();
            string sql = $"""
                SELECT 
                  cc.table_name, 
                  c.table_name as r_table_name, 
                  cc2.column_name, 
                  cols.column_name as r_column_name, 
                  c.constraint_name AS constraint_name, 
                  c.r_constraint_name 
                FROM 
                  user_constraints c 
                  JOIN user_constraints cc ON c.r_constraint_name = cc.constraint_name 
                  JOIN user_cons_columns cols ON c.constraint_name = cols.constraint_name 
                  JOIN user_cons_columns cc2 ON cc.constraint_name = cc2.constraint_name 
                WHERE 
                  c.constraint_type = 'R'
                ORDER BY 
                  c.table_name asc
                """;
            res = await QueryRaw($"{sql}");
            return res;
        }
        public async Task<List<Dictionary<string, dynamic>>> GetConstraintsType(string table, string con = "")
        {
            List<Dictionary<string, dynamic>> res = new List<Dictionary<string, dynamic>>();
            string query = $"SELECT column_name, data_type FROM all_tab_columns WHERE table_name = '{table}';";
            res = await QueryRaw($"{query}");
            return res;
        }
        public async Task<List<Dictionary<string, dynamic>>> GetStructureTable(string table, string con = "")
        {
            List<Dictionary<string, dynamic>> res = new List<Dictionary<string, dynamic>>();
            string query = $"SELECT column_name, data_type FROM all_tab_columns WHERE table_name = '{table}';";
            res = await QueryRaw($"{query}");
            return res;
        }
        public async Task<List<Dictionary<string, dynamic>>> QueryRaw(string sql, string con = "")
        {
            List<Dictionary<string, dynamic>> res = new List<Dictionary<string, dynamic>>();
            string connecton = string.IsNullOrEmpty(con) ? connectionOracle : con;
            using (var conn = new OracleConnection(connecton))
            {
                conn.Open();
                OracleCommand command = new OracleCommand();
                command.Connection = conn;
                command.CommandText = sql;
                try
                {
                    command.InitialLOBFetchSize = 8192;
                    OracleDataReader result = command.ExecuteReader();

                    while (result.Read())
                    {
                        Dictionary<string, dynamic> row = Helper.GenerateRow(result);
                        res.Add(row);
                    }
                    result.Close();
                }
                catch (OracleException ex)
                {
                    //throw new Exception(ex.Message);
                }
            }

            return res;
        }
        public async Task<Dictionary<string, dynamic>> GenerateTable(string con = "")
        {
            Dictionary<string, dynamic> res = new Dictionary<string, dynamic>();
            List<Dictionary<string, dynamic>> table = new List<Dictionary<string, dynamic>>();
            List<Dictionary<string, dynamic>> view = new List<Dictionary<string, dynamic>>();
            List<Dictionary<string, dynamic>> fields = new List<Dictionary<string, dynamic>>();

            List<Dictionary<string, dynamic>> hasOnes = new List<Dictionary<string, dynamic>>();
            List<Dictionary<string, dynamic>> hasManys = new List<Dictionary<string, dynamic>>();
            hasOnes = await GetRelationTable();
            hasManys = await GetRelationHasMany();
            Console.WriteLine("Build Structure Table...");
            // MENDAPATKAN VIEW DAN TABEL MASTER
            table = await GetAllTableView();
            fields = await GetAllFieldsTables();
            int idx = 1;
            int l = table.Count();
            int max = 0;
            foreach (var item in table)
            {
                string tbl = string.Empty;
                try
                {
                    tbl = item["TABLE_NAME"];
                }
                catch (Exception ex) { }
                string loading = $"\r{GetLoadingChar(idx)} Structure Table" + $" ({idx}/{l}): {tbl}";
                if (loading.Length > max)
                {
                    max = loading.Length;
                }
                else
                {
                    loading = loading.PadRight(max);
                }
                Console.Write(loading);
                // Thread.Sleep(5);
                if (!string.IsNullOrEmpty(tbl))
                {
                    // FIELDS
                    if (fields.Count > 0)
                    {

                        List<Dictionary<string, dynamic>> field = new List<Dictionary<string, dynamic>>();
                        field = fields.Where(e => e["TABLE_NAME"] == tbl).ToList();
                        generateFields(field, res, tbl);
                    }
                    // RELATIONS
                    List<Dictionary<string, dynamic>> hasOne = new List<Dictionary<string, dynamic>>();
                    List<Dictionary<string, dynamic>> hasMany = new List<Dictionary<string, dynamic>>();
                    hasOne = hasOnes.Where(e => e["TABLE_NAME"] == tbl).ToList();
                    hasMany = hasManys.Where(e => e["TABLE_NAME"] == tbl).ToList();
                    if (hasOne.Count > 0)
                    {
                        generateStructure(hasOne, res, "has-one", tbl);
                    }
                    if (hasMany.Count > 0)
                    {
                        generateStructure(hasMany, res, "has-many", tbl);
                    }

                }
                idx++;
            }
            Console.Write($"\r{"Structure Table: Done".PadRight(max)}");
            Console.WriteLine("");
            return res;
        }
        // Fungsi untuk mendapatkan karakter putar
        static void generateFields(List<Dictionary<string, dynamic>> tbl, Dictionary<string, dynamic> rel, string tableName)
        {
            if (tbl.Count() > 0)
            {
                if (!rel.ContainsKey(tableName))
                {
                    rel[tableName] = new Dictionary<string, dynamic>();
                }
            }
            if (tbl.Count() > 0)
            {
                foreach (var one in tbl)
                {
                    try
                    {
                        if (!rel[tableName].ContainsKey("fields"))
                        {
                            rel[tableName]["fields"] = new Dictionary<string, dynamic>();
                        }
                        rel[tableName]["fields"][one["COLUMN_NAME"]] = new Dictionary<string, dynamic>
                                {
                                    {"type", one["DATA_TYPE"]},
                                    {"length", one["DATA_LENGTH"] },
                                };
                        try
                        {
                            if (!string.IsNullOrEmpty(one["CONSTRAINT_TYPE"]))
                            {
                                if (one["CONSTRAINT_TYPE"] == "PK")
                                {
                                    rel[tableName]["fields"][one["COLUMN_NAME"]]["primary_key"] = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    catch (Exception ex)
                    {
                        // Console.WriteLine(ex.Message);
                    }
                }
            }

        }
        static void generateStructure(List<Dictionary<string, dynamic>> tbl, Dictionary<string, dynamic> rel, string type, string tableName)
        {
            if (tbl.Count() > 0)
            {
                if (!rel.ContainsKey(tableName))
                {
                    rel[tableName] = new Dictionary<string, dynamic>();
                }
            }
            if (tbl.Count() > 0)
            {
                foreach (var one in tbl)
                {
                    try
                    {
                        if (!rel[tableName].ContainsKey("relations"))
                        {
                            rel[tableName]["relations"] = new Dictionary<string, dynamic>();
                        }
                        string keysFk = one["R_TABLE_NAME"];
                        keysFk = validateTable(rel[tableName]["relations"], one["R_TABLE_NAME"]);
                        rel[tableName]["relations"][keysFk] = new Dictionary<string, dynamic>
                                {
                                    {"from", new Dictionary<string, dynamic>
                                        {
                                            {"table", one["TABLE_NAME"]},
                                            {"field", one["COLUMN_NAME"]}
                                        }
                                    },
                                    {"to", new Dictionary<string, dynamic>
                                        {
                                            {"table", one["R_TABLE_NAME"]},
                                            {"field", one["R_COLUMN_NAME"]}
                                        }
                                    },
                                    {"type", type}
                                };

                    }
                    catch (Exception ex)
                    {
                        // Console.WriteLine(ex.Message);
                    }
                }
            }

        }
        static string validateTable(Dictionary<string, dynamic> table, string keys)
        {
            string result = keys;
            if (table.ContainsKey(result))
            {
                result = $"{result}_other";
                result = validateTable(table, result);
            }
            else
            {
                result = $"{result}";

            }
            return result;
        }
        static char GetLoadingChar(int index)
        {
            char[] chars = { '|', '/', '-', '\\' };
            return chars[index % chars.Length];
        }
        private async Task<List<Dictionary<string, dynamic>>> generateIncludes(List<Dictionary<string, dynamic>> res, Dictionary<string, dynamic> include, string table)
        {
            List<Dictionary<string, dynamic>> res1 = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> relations = new Dictionary<string, dynamic>();
            Dictionary<string, dynamic> schema = new Dictionary<string, dynamic>();
            schema = Global.SchemaDb[table].ToObject<Dictionary<string, dynamic>>();
            try
            {
                if (schema.ContainsKey("relations"))
                {
                    relations = schema["relations"].ToObject<Dictionary<string, dynamic>>();
                    foreach (KeyValuePair<string, dynamic> ic in include)
                    {
                        Console.WriteLine(ic.Key);
                        ResponTypeTable typeRelation = new ResponTypeTable();
                        if (relations.ContainsKey(ic.Key))
                        {
                            Console.WriteLine("ADA RELASI?");
                            try
                            {
                                // // Console.WriteLine(ic.Key);
                                // // Console.WriteLine("\r2. Mulai Masuk getTypeTable");

                                Dictionary<string, dynamic> relasi = new Dictionary<string, dynamic>();
                                relasi = relations[ic.Key].ToObject<Dictionary<string, dynamic>>();
                                if (ic.Value.GetType() == typeof(bool))
                                {
                                    typeRelation = getTypeTable(res, relasi, schema);
                                }
                                else
                                {
                                    // // Console.WriteLine("SELAIN BOOLEAN MASUK SINI");
                                    typeRelation = getTypeTable(res, relasi, schema, ic.Value.ToObject<Dictionary<string, dynamic>>());
                                }
                                try
                                {

                                    // // Console.WriteLine("HASIL ResponTypeTable");
                                    Console.WriteLine(JsonConvert.SerializeObject(typeRelation));
                                    typeRelation.type = relasi["type"];
                                    // Schema Params
                                    PrismaModel param = new PrismaModel();
                                    param.table = typeRelation.table;
                                    param.param = typeRelation.param;
                                    param.action = "findMany";
                                    // // Console.WriteLine("HASIL PRISMA MODEL");
                                    // // Console.WriteLine(JsonConvert.SerializeObject(param));
                                    List<Dictionary<string, dynamic>> result = await QueryPrisma(param);

                                    // // Console.WriteLine("==========><==========");
                                    Console.WriteLine(JsonConvert.SerializeObject(result));
                                    // // Console.WriteLine(JsonConvert.SerializeObject(result));
                                    // // Console.WriteLine("==========><==========");
                                    genRelation(ref res, result, typeRelation, ic.Key);
                                    // // Console.WriteLine("==========>SERI<==========");
                                    // // Console.WriteLine(JsonConvert.SerializeObject(res));
                                    // // Console.WriteLine("==========>SERI<==========");

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"{ex.Message}");
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.Message);
            }
            return res;
        }
        static ResponTypeTable getTypeTable(List<Dictionary<string, dynamic>> data, Dictionary<string, dynamic> rel, Dictionary<string, dynamic> schema, Dictionary<string, dynamic> parameter = null)
        {
            ResponTypeTable result = new ResponTypeTable();
            Console.WriteLine("===> 1.2");
            List<Dictionary<string, dynamic>> paramsQuery = new List<Dictionary<string, dynamic>>();
            try
            {
                // // Console.WriteLine(JsonConvert.SerializeObject(data));
                // // Console.WriteLine(JsonConvert.SerializeObject(schema));
                // Baris untuk mendapatkan include
                Dictionary<string, dynamic> param = new Dictionary<string, dynamic>();
                // Console.WriteLine("--schema--");
                // Console.WriteLine(JsonConvert.SerializeObject(rel));
                string tblFrom = rel["from"]["table"];
                string tblTo = rel["to"]["table"];
                string fieldsFrom = rel["from"]["field"];
                string fieldsTo = rel["to"]["field"];
                string pk = findPrimaryKey(tblFrom);
                Console.WriteLine("===> 1.2");
                Console.WriteLine($"===> {pk}");
                try
                {
                    if (parameter != null)
                    {
                        foreach (KeyValuePair<string, dynamic> ic in parameter)
                        {
                            param[ic.Key] = ic.Value;
                        }
                    }

                }
                catch (Exception ex)
                {
                    // Console.WriteLine(ex.Message);
                }
                Console.WriteLine("===> 1.3");
                // Console.WriteLine("--1--");
                // Console.WriteLine(JsonConvert.SerializeObject(parameter));
                // Console.WriteLine($"--{fieldsFrom}--");
                // Mendapatkan list id
                List<dynamic> id = new List<dynamic>();
                foreach (var item in data)
                {
                    id.Add(item[fieldsFrom]);
                }
                // Console.WriteLine("--2--");
                // Baris untuk generate params query tabel atau relasi
                Console.WriteLine("===> 1.4");
                if (param != null)
                {
                    // Console.WriteLine("--3--");
                    if (!param.ContainsKey("where"))
                    {
                        // Console.WriteLine("--4--");
                        param["where"] = new Dictionary<string, dynamic>();
                    }
                    // Console.WriteLine("--5--");
                    param["where"][fieldsTo] = new Dictionary<string, dynamic>
                                                {
                                                    { "in", id }
                                                };
                }


                // Penghindaran error karena tidak adanya select foreign key atau id karena sifatnya wajib ada.
                try
                {
                    Console.WriteLine("----------------------");
                    Console.WriteLine(JsonConvert.SerializeObject(param));
                    Console.WriteLine("----------------------");
                    if (param != null)
                    {
                        if (param.ContainsKey("select"))
                        {
                            Console.WriteLine("ADA select");
                            if (!param["select"].ContainsKey(fieldsFrom) && param.ContainsKey("include"))
                            {
                                param["select"][fieldsFrom] = true;
                            }
                            else if (param.ContainsKey("include"))
                            {
                                param["select"][fieldsFrom] = true;
                            }
                            param["select"][pk] = true;

                        }
                    }
                }
                catch (Exception ex)
                {

                }

                //// // Console.WriteLine(JsonConvert.SerializeObject(param));
                paramsQuery.Add(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JsonConvert.SerializeObject(param)));
                result.param = paramsQuery;
                result.table = tblTo;
                result.r_column = fieldsTo;
                result.column = fieldsFrom;
            }
            catch (Exception ex)
            {
                // Console.WriteLine("--ERROR--");
                // Console.WriteLine(ex.Message);
            }


            return result;
        }
        static string findPrimaryKey(string table)
        {

            string pk = string.Empty;
            try
            {
                Dictionary<string, dynamic> schema = new Dictionary<string, dynamic>();
                schema = Global.SchemaDb[table].ToObject<Dictionary<string, dynamic>>();
                if (schema != null)
                {
                    Dictionary<string, dynamic> fields = new Dictionary<string, dynamic>();
                    fields = schema["fields"].ToObject<Dictionary<string, dynamic>>();
                    // Console.WriteLine(JsonConvert.SerializeObject(fields));
                    List<string> keysPK = fields.Where(e => e.Value["primary_key"] == true)
                                                  .Select(pair => pair.Key)
                                                  .ToList();
                    string keyPK = keysPK[0];
                    if (!string.IsNullOrEmpty(keyPK))
                    {
                        pk = keyPK;
                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"pk: {ex.Message}");
            }

            return pk;
        }
        static void genRelation(ref List<Dictionary<string, dynamic>> data, List<Dictionary<string, dynamic>> relData, ResponTypeTable rel, string table)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            foreach (var item in data)
            {
                switch (rel.type)
                {
                    case "has-one":
                        item[table] = null;
                        Dictionary<string, dynamic> findOne = new Dictionary<string, dynamic>();
                        try
                        {
                            if (relData.Count > 0)
                            {
                                findOne = relData.Where(e => e[rel.r_column] == item[rel.column]).FirstOrDefault();
                                item[table] = findOne;
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        break;
                    case "has-many":

                        item[table] = new List<Dictionary<string, dynamic>>();
                        List<Dictionary<string, dynamic>> findMany = new List<Dictionary<string, dynamic>>();
                        try
                        {
                            if (relData.Count > 0)
                            {
                                findMany = relData.Where(e => e[rel.r_column] == item[rel.column]).ToList();
                                // // Console.WriteLine("MANY");
                                // // Console.WriteLine(JsonConvert.SerializeObject(findMany));
                                item[table] = findMany;
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        break;
                }
                result.Add(item);
            }
            // return result;
        }

        static void genInclude(Dictionary<string, dynamic> data, List<Dictionary<string, dynamic>> main, ResponTypeTable rel)
        {
            switch (rel.type)
            {
                case "one2one":
                    Dictionary<string, dynamic> robj = new Dictionary<string, dynamic>();
                    try
                    {
                        robj = main.FindAll(e => e[rel.r_column] == data[rel.column]).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                    }
                    data[rel.r_table] = robj;
                    break;
                case "hasmany":
                    List<Dictionary<string, dynamic>> rdata = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        rdata = main.FindAll(e => e[rel.r_column] == data[rel.column]).ToList();
                    }
                    catch (Exception ex)
                    {
                    }
                    data[rel.r_table] = rdata;
                    break;
            }

        }
        static string generateOrder(ref PrismaModel parameter)
        {
            Dictionary<string, dynamic> param = parameter.getOrder();
            string result = string.Empty;
            List<string> orderBy = new List<string>();
            try
            {
                if (param["type"] == "Newtonsoft.Json.Linq.JArray")
                {
                    JArray orders = new JArray();
                    orders = param["data"];
                    foreach (JObject ord in orders)
                    {

                        Dictionary<string, dynamic> order = ord.ToObject<Dictionary<string, dynamic>>();
                        foreach (KeyValuePair<string, dynamic> item in order)
                        {
                            orderBy.Add($"{item.Key} {item.Value}");
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, dynamic> item in param["data"])
                    {
                        orderBy.Add($"{item.Key} {item.Value}");
                    }
                }
                if (orderBy.Count > 0)
                {
                    result = $" ORDER BY {string.Join(", ", orderBy)}";
                }

            }
            catch (Exception ex)
            {

            }
            return result;
        }
        static string generateSelect(ref PrismaModel param)
        {
            string result = string.Empty;
            List<string> sql = new List<string>();

            Dictionary<string, dynamic> schema = new Dictionary<string, dynamic>();
            schema = Global.SchemaDb[param.table].ToObject<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> fields = findObject("fields", schema);
            Dictionary<string, dynamic> relations = findObject("relations", schema);
            string pk = findPrimaryKey(param.table);
            sql.Add($"select");
            Dictionary<string, dynamic> select = new Dictionary<string, dynamic>();
            Dictionary<string, dynamic> parameter = new Dictionary<string, dynamic>();
            List<Dictionary<string, dynamic>> paramQuery = new List<Dictionary<string, dynamic>>();
            try
            {
                parameter = param.param[0];
            }
            catch (Exception ex) { }
            try
            {
                select = param.getSelect();
                if (select == null)
                {
                    sql.Add($"*");
                }
                else
                {
                    List<string> selectColumn = new List<string>();
                    select[pk] = true;
                    foreach (KeyValuePair<string, dynamic> item in select)
                    {
                        try
                        {
                            Console.WriteLine($"Key => {item.Key}");
                            if (relations.ContainsKey(item.Key))
                            {

                                try
                                {

                                    if (!parameter.ContainsKey("include"))
                                    {
                                        parameter["include"] = new Dictionary<string, dynamic>();
                                        parameter["include"][item.Key] = true;
                                    }
                                    else
                                    {
                                        string typeInclude = getTypeValue(parameter["include"]);
                                        Dictionary<string, dynamic> inc = new Dictionary<string, dynamic>();
                                        inc = typeInclude == "Newtonsoft.Json.Linq.JObject" ? parameter["include"].ToObject<Dictionary<string, dynamic>>() : parameter["include"];
                                        if (!inc.ContainsKey(item.Key))
                                        {
                                            parameter["include"][item.Key] = true;
                                        }
                                    }

                                    Dictionary<string, dynamic> incl = new Dictionary<string, dynamic>();
                                    incl = getTypeValue(parameter["include"]) == "Newtonsoft.Json.Linq.JObject" ? parameter["include"].ToObject<Dictionary<string, dynamic>>() : parameter["include"];

                                    foreach (KeyValuePair<string, dynamic> ic in incl)
                                    {
                                        Dictionary<string, dynamic> rel = findObject(ic.Key, relations);
                                        Console.WriteLine($"rel => {JsonConvert.SerializeObject(rel)}");
                                        if (rel != null)
                                        {
                                            string fk = $"\"{rel["from"]["field"]}\"";
                                            bool search = selectColumn.Any(e => e == fk);
                                            Console.WriteLine($"ketemu => {search}");
                                            if (!search)
                                            {
                                                selectColumn.Add(fk);
                                            }
                                        }
                                    }
                                    Console.WriteLine($"parameter => {JsonConvert.SerializeObject(selectColumn)}");

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else
                            {
                                if (fields.ContainsKey(item.Key))
                                {
                                    if (item.Value)
                                    {
                                        selectColumn.Add($"\"{item.Key}\"");
                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    if (selectColumn.Count > 0)
                    {
                        sql.Add($"{string.Join(",", selectColumn)}");
                    }
                    else
                    {
                        sql.Add("*");
                    }

                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.Message);
            }
            paramQuery.Add(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JsonConvert.SerializeObject(parameter)));
            param.param = paramQuery;
            sql.Add($"from \"{param.table}\"");
            result = $"{string.Join(" ", sql)}";
            return result;
        }
        private static Dictionary<string, dynamic> findObject(string keys, Dictionary<string, dynamic> data)
        {
            Dictionary<string, dynamic> res = null;
            if (data.ContainsKey(keys))
            {
                res = data[keys].ToObject<Dictionary<string, dynamic>>();

            }
            return res;
        }
        static string generateWhere(ref PrismaModel param)
        {
            Dictionary<string, dynamic> whereClause = param.getWhere();
            string result = string.Empty;
            if (whereClause != null)
            {
                if (whereClause.Count > 0)
                {
                    List<string> sql = new List<string>();
                    getWhere(whereClause, sql, "and");
                    result = $" where {string.Join(" ", sql)}";

                }

            }
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
                }
                catch (Exception ex)
                {
                }
                switch (item.Key.ToLower())
                {
                    case "and":
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
                            if (sqlOr.Count() > 0)
                            {
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

                Type type = val.GetType();
                typeValue = type.ToString();
            }
            catch (Exception ex)
            {
            }
            return typeValue;
        }
        static void AddSeperator(string condition, List<string> sql)
        {
            IEnumerable<string> sqlFilter = sql.Where(e => e != "(" || e != ")" || e != $"NOT (");
            //// // Console.WriteLine(sqlFilter.Count());
            //foreach (var item in sqlFilter)
            //{
            //    // // Console.WriteLine($"{item}");
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
