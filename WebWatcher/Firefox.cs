using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Data.SQLite;
using System.Data;
using System.Linq;

namespace WebWatcher
{
    public class Firefox
    {
        private string _firefoxPath;

        public string BrowserPath
        {
            get
            {
                return _firefoxPath;
            }
        }


        public Firefox()
        {
            // Get Current Users App Data
            _firefoxPath = Environment.GetFolderPath
                             (Environment.SpecialFolder.ApplicationData);

            // Move to Firefox Data
            _firefoxPath += "\\Mozilla\\Firefox\\Profiles\\";
        }

        public IEnumerable<URL> GetHistory()
        {
            List<URL> urls = new List<URL>();
            // Check if directory exists
            if (Directory.Exists(_firefoxPath))
            {
                // Loop each Firefox Profile
                foreach (string folder in Directory.GetDirectories
                                                   (_firefoxPath))
                {
                    // Fetch Profile History
                    urls.AddRange(ExtractUserHistory(folder));
                }
            }
            return urls;
        }


        IEnumerable<URL> ExtractUserHistory(string folder)
        {
            List<URL> urls = new List<URL>();

            // Get User history info
            DataTable historyDT = ExtractFromTable(folder);
            

            // Loop each history entry
            foreach (DataRow row in historyDT.Rows)
            {
               // Select entry Date from visits
               var entryDate = (from visit_dates in historyDT.AsEnumerable()
                                select visit_dates).LastOrDefault();

                // If history entry has date
                if (entryDate != null)
                {
                    // Obtain URL and Title strings
                    string url = row["Url"].ToString();
                    string title = row["title"].ToString();
                    string host = Utils.Reverse(row["rev_host"].ToString());
                    long d = Convert.ToInt64(row["visit_date"]);
 
                    // Create new Entry
                    URL u = new URL(url.Replace('\'', ' '),
                                    title.Replace('\'', ' '),
                                    d,
                                    "Mozilla Firefox",
                                    host);

                    // Add entry to list
                    urls.Add(u);
                }
            }
            // Clear URL History
            //DeleteFromTable("moz_places", folder);
            //DeleteFromTable("moz_historyvisits", folder);

            return urls;
        }

        void DeleteFromTable(string table, string folder)
        {
            SQLiteConnection sql_con;
            SQLiteCommand sql_cmd;


            // FireFox database file
            string dbPath = folder + "\\places.sqlite";


            // If file exists
            if (File.Exists(dbPath))
            {
                // Data connection
                sql_con = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;New=False;Compress=True;");

                // Open the Conn
                sql_con.Open();


                // Delete Query
                string CommandText = "delete from " + table;


                // Create command
                sql_cmd = new SQLiteCommand(CommandText, sql_con);


                sql_cmd.ExecuteNonQuery();


                // Clean up
                sql_con.Close();
            }
        }


        DataTable ExtractFromTable(string folder)
        {
            SQLiteConnection sql_con;
            SQLiteCommand sql_cmd;
            SQLiteDataAdapter DB;
            DataTable DT = new DataTable();


            // FireFox database file
            string dbPath = folder + "\\places.sqlite";


            // If file exists
            if (File.Exists(dbPath))
            {
                // Data connection
                sql_con = new SQLiteConnection("Data Source=" + dbPath +

                                    ";Version=3;New=False;Compress=True;");


                // Open the Connection
                sql_con.Open();
                sql_cmd = sql_con.CreateCommand();

                /*
                         DeleteFromTable("moz_places", folder);
            DeleteFromTable("moz_historyvisits", folder);
                 */


                // Select Query
                string CommandText =
                    "SELECT * FROM moz_places JOIN moz_historyvisits " +
                    "ON moz_historyvisits.place_id = moz_places.id " +
                    "ORDER BY moz_historyvisits.visit_date DESC " +
                    "LIMIT 100";

                //"WHERE moz_historyvisits.visit_date BETWEEN DATE('now') AND DATE('now', '+1 day')";



                // Populate Data Table
                DB = new SQLiteDataAdapter(CommandText, sql_con);
                DB.Fill(DT);


                // Clean up
                sql_con.Close();
            }
            return DT;
        }
    }

}