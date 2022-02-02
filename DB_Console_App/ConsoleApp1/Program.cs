using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

namespace MyApp // Note: actual namespace depends on the project name.
{
    public class Program
    {
        //Lists of the entries to insert
        static HttpClient client = new HttpClient();
        static List<string> histRecList = new List<string>();
        static List<string> searchedLocalities = new List<string>();

        //Read properties from app.config
        static string datasource = ConfigurationManager.AppSettings["Datasource"];
        static string port = ConfigurationManager.AppSettings["Port"];
        static string username = ConfigurationManager.AppSettings["Username"];
        static string password = ConfigurationManager.AppSettings["Password"];
        static string database = ConfigurationManager.AppSettings["Database"];
        static string connectionString = $"datasource={datasource};port={port};username={username};password={password};database={database}";
        static string version = ConfigurationManager.AppSettings["Version"];


        public static void Main(string[] args)
        {
            var opt1 = false;
            var opt2 = false;
            var waitingForInput = true;

            Console.WriteLine("PTT-Archive DB Updater");
            Console.WriteLine($"Current Version: v{version} (change in PTT-Archive DB-Updater.dll.config)");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("1. Load from Post PLZ file");
            Console.WriteLine("2. Load from Ortsnamen.ch");
            Console.WriteLine("3. Load from both");

            while (waitingForInput)
            {
                Console.WriteLine();
                Console.WriteLine("Enter the number of the option:");
                var option = Console.ReadLine();
                int.TryParse(option, out var number);
                switch (number)
                {
                    case 1:
                        opt1 = true;
                        waitingForInput = false;
                        break;
                    case 2:
                        opt2 = true;
                        waitingForInput = false;
                        break;
                    case 3:
                        opt1 = true;
                        opt2 = true;
                        waitingForInput = false;
                        break;
                    default:
                        Console.WriteLine("Option not recognized");
                        break;
                }
            }

            InsertVersion(version);

            if (opt1)
            {
                Drop_entries("localities");
                LoadJson();
                Console.WriteLine("End of JSON import.");
            }

            if (opt2)
            {
                Drop_entries("historical");

                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["OrtsnamenApiUri"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                LoadApiAsync().GetAwaiter().GetResult();
                Console.WriteLine("End of API import.");
            }
            
        }

        //Prepares a list of localities to be called with the API
        public static async Task LoadApiAsync()
        {
            var list = GetLocalities();
            foreach(var l in list)
            {
                List<string> names = new List<string>();
                names.Add(l.de);
                names.Add(l.fr);
                names.Add(l.it);

                await RunAsync(names);
                
                if (histRecList.Count < 1) continue;

                var entry = new Historical();
                entry.fk_locality = l.id;

                foreach (var h in histRecList)
                {
                    if (String.IsNullOrWhiteSpace(h)) continue;
                    entry.names += h + ";";
                }

                InsertHistoricals(entry);
                histRecList = new List<string>();
            }    
        }

        //Actual API call
        static async Task RunAsync(List<string> gemeinde)
        {

            foreach (var name in gemeinde)
            {
                if (string.IsNullOrWhiteSpace(name) || name.Any(char.IsDigit) || searchedLocalities.Contains(name)) continue;
                searchedLocalities.Add(name);
                try
                {
                    Console.WriteLine($"Search for {name}.....");
                    HttpResponseMessage response = await client.GetAsync($"de/api/search?q={name}");
                    if (response.IsSuccessStatusCode)
                    {
                        Ortsnamen items = new Ortsnamen();
                        var json = response.Content.ReadAsStringAsync().Result;

                        items = JsonConvert.DeserializeObject<Ortsnamen>(json);

                        var recs = items.results?.Find(x => x.types.Contains("Gemeinde"))?.historical_records;

                        if (recs != null)
                        {
                            foreach (var rec in recs)
                            {
                                if (histRecList.Contains(rec.form)) continue;
                                histRecList.Add(rec.form);
                            }
                        }

                    }
                    Console.WriteLine($"Search ended.....");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                Thread.Sleep(500);
            }
            
        }


        //Reads and inserts localities from the JSON files
        public static void LoadJson()
        {
            List<ItemPlz> items2;

            var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            using (StreamReader r2 = new StreamReader(ConfigurationManager.AppSettings["AltPlzFilePath"]))
            {
                string json2 = r2.ReadToEnd();
                items2 = JsonConvert.DeserializeObject<List<ItemPlz>>(json2);
            }

            using (StreamReader r = new StreamReader(ConfigurationManager.AppSettings["PlzFilePath"]))
            {
                string json = r.ReadToEnd();
                List<ItemPlz> items = JsonConvert.DeserializeObject<List<ItemPlz>>(json);
                foreach (var item in items)
                {
                    if (string.IsNullOrWhiteSpace(item.fields.ortbez18)) continue;
                    var de = "";
                    var fr = "";
                    var it = "";

                    if (item.fields.sprachcode == 1)
                    {
                        de = item.fields.ortbez18.Replace("'", "''");
                    }
                    if (item.fields.sprachcode == 2)
                    {
                        fr = item.fields.ortbez18.Replace("'", "''");
                    }
                    if (item.fields.sprachcode == 3)
                    {
                        it = item.fields.ortbez18.Replace("'", "''");
                    }

                    Console.WriteLine("{0} {1}", item.fields.ortbez18, item.fields.onrp);
                    var itemAlt = items2.FindAll(x => x.fields.onrp == item.fields.onrp);
                    foreach(var t in itemAlt)
                    {
                        if (string.IsNullOrWhiteSpace(t.fields.ortbez18)) continue;
                        if (t.fields.sprachcode == 1)
                        {
                            de = t.fields.ortbez18.Replace("'", "''");
                        }
                        if (t.fields.sprachcode == 2)
                        {
                            fr = t.fields.ortbez18.Replace("'", "''");
                        }
                        if (t.fields.sprachcode == 3)
                        {
                            it = t.fields.ortbez18.Replace("'", "''");
                        }
                        Console.WriteLine("{0} {1}", t.fields.ortbez18, t.fields.onrp);
                    }

                    InsertLanguage(de.TrimEnd(digits), fr.TrimEnd(digits), it.TrimEnd(digits));

                }
            }
            Delete_duplicates();
        }

        #region Object classes (needed to parse the JSON files and API response)
        public class HistoricalRecord
        {
            public string? biblio;
            public string? text;
            public int? year;
            public List<string>? provider;
            public string? form;
        }

        public class Result
        {
            public int? id;
            public string? url;
            public string? permalink;
            public string? name;
            public List<string>? description;
            public string? localisation;
            public List<string>? cantons;
            public List<string>? municipalities;
            public List<string>? types;
            public List<HistoricalRecord>? historical_records;
            public string? terms;
        }

        public class Ortsnamen
        {
            public int? count;
            public object? next;
            public object? previous;
            public List<Result>? results;
        }

        public class ItemPlz
        {
            public string? datasetid;
            public string? recordid;
            public Fields? fields;
            public string? record_timestamp;
        }

        public class Fields
        {
            public string? plz_coff;
            public int? briefz_durch;
            public int? onrp;
            public int? gplz;
            public string? rec_art;
            public string? plz_zz;
            public string? postleitzahl;
            public int? bfsnr;
            public string? ortbez18;
            public int? sprachcode;
            public string? kanton;
            public string? ortbez27;
            public string? gilt_ab_dat;
            public int? plz_briefzust;
            public int? plz_typ;
        }

        public class Locality
        {
            public int id;
            public string de;
            public string fr;
            public string it;
        }

        public class Historical
        {
            public int id;
            public int fk_locality;
            public string names;
        }
        #endregion

        #region SQL Methods
        public static List<Locality> GetLocalities()
        {
            var list = new List<Locality>();

            string query = "";
            try
            {
                //This is my connection string i have assigned the database file address path  
                string MyConnection2 = connectionString;
                //This is my insert query in which i am taking input from the user through windows forms  
                query = $"SELECT * FROM Localities";
                //This is  MySqlConnection here i have created the object and pass my connection string.  
                MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
                //This is command class which will handle the query and connection object.  
                MySqlCommand MyCommand2 = new MySqlCommand(query, MyConn2);
                MySqlDataReader MyReader2;
                MyConn2.Open();
                MyReader2 = MyCommand2.ExecuteReader();     // Here our query will be executed and data saved into the database.  
                while (MyReader2.Read())
                {
                    list.Add(new Locality { id = MyReader2.GetInt32("id"), de = MyReader2.GetString("de"), fr = MyReader2.GetString("fr"), it = MyReader2.GetString("it") });
                }
                MyConn2.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {query}");
            }

            return list;
        }

        public static void InsertHistoricals(Historical historical)
        {
            Console.WriteLine($"Inserting: {historical.names}");
            string query = "";
            try
            {
                //This is my connection string i have assigned the database file address path  
                string MyConnection2 = connectionString;
                //This is my insert query in which i am taking input from the user through windows forms  
                query = $"insert into query_expansion.historical(fk_localitiesId, names) values('{historical.fk_locality}','{historical.names}');";
                //This is  MySqlConnection here i have created the object and pass my connection string.  
                MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
                //This is command class which will handle the query and connection object.  
                MySqlCommand MyCommand2 = new MySqlCommand(query, MyConn2);
                MySqlDataReader MyReader2;
                MyConn2.Open();
                MyReader2 = MyCommand2.ExecuteReader();     // Here our query will be executed and data saved into the database.  
                MyConn2.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {query}");
            }
        }

        public static void InsertLanguage(string de, string fr, string it)
        {
            string query = "";
            try
            {
                //This is my connection string i have assigned the database file address path  
                string MyConnection2 = connectionString;
                //This is my insert query in which i am taking input from the user through windows forms  
                query = $"insert into query_expansion.localities(de,fr,it) values('{de}','{fr}','{it}');";
                //This is  MySqlConnection here i have created the object and pass my connection string.  
                MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
                //This is command class which will handle the query and connection object.  
                MySqlCommand MyCommand2 = new MySqlCommand(query, MyConn2);
                MySqlDataReader MyReader2;
                MyConn2.Open();
                MyReader2 = MyCommand2.ExecuteReader();     // Here our query will be executed and data saved into the database.  
                MyConn2.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {query}");
            }
        }

        public static void InsertVersion(string value)
        {
            string query = "";
            try
            {
                //This is my connection string i have assigned the database file address path  
                string MyConnection2 = connectionString;
                //This is my insert query in which i am taking input from the user through windows forms  
                query = $"insert into query_expansion.versionhistory(version) values('{value}');";
                //This is  MySqlConnection here i have created the object and pass my connection string.  
                MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
                //This is command class which will handle the query and connection object.  
                MySqlCommand MyCommand2 = new MySqlCommand(query, MyConn2);
                MySqlDataReader MyReader2;
                MyConn2.Open();
                MyReader2 = MyCommand2.ExecuteReader();     // Here our query will be executed and data saved into the database.  
                MyConn2.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {query}");
            }
        }

        public static void Delete_duplicates()
        {
            string query = "";
            try
            {
                //This is my connection string i have assigned the database file address path  
                string MyConnection2 = connectionString;
                //This is my insert query in which i am taking input from the user through windows forms  
                query = $"DELETE t1 FROM localities t1 INNER JOIN localities t2 WHERE t1.id < t2.id AND t1.de like t2.de AND t1.fr like t2.fr AND t1.it like t2.it; ";
                //This is  MySqlConnection here i have created the object and pass my connection string.  
                MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
                //This is command class which will handle the query and connection object.  
                MySqlCommand MyCommand2 = new MySqlCommand(query, MyConn2);
                MySqlDataReader MyReader2;
                MyConn2.Open();
                MyReader2 = MyCommand2.ExecuteReader();     // Here our query will be executed and data saved into the database.  
                while (MyReader2.Read())
                {
                }
                MyConn2.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {query}");
            }
        }

        public static void Drop_entries(string table)
        {
            string query = "";
            try
            {
                //This is my connection string i have assigned the database file address path  
                string MyConnection2 = connectionString;
                //This is my insert query in which i am taking input from the user through windows forms  
                query = $"DELETE FROM {table};";
                //This is  MySqlConnection here i have created the object and pass my connection string.  
                MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
                //This is command class which will handle the query and connection object.  
                MySqlCommand MyCommand2 = new MySqlCommand(query, MyConn2);
                MySqlDataReader MyReader2;
                MyConn2.Open();
                MyReader2 = MyCommand2.ExecuteReader();     // Here our query will be executed and data saved into the database.  
                while (MyReader2.Read())
                {
                }
                MyConn2.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {query}");
            }
        }
        #endregion
    }

}