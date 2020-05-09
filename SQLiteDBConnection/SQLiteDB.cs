using System;
using System.Collections.Generic;
using System.Data;
using SQLite;

namespace SQLiteDBConnection
{
    public class SQLiteDB
    {
        string _path = "SKI.db";
        public SQLiteDB() { }

        public void Insert(object e)
        {
            using (var db = new SQLiteConnection(_path))
            {
                db.Insert(e);
                db.Commit();
            }
        }

        public void Update(object e)
        {
            using (var db = new SQLiteConnection(_path))
            {
                db.Update(e);
                db.Commit();
            }
        }

        public void Delete(object e)
        {
            using (var db = new SQLiteConnection(_path))
            {
                db.Delete(e);
                db.Commit();
            }
        }

        public List<string> GetTablesNames()
        {
            List<string> ImportedFiles = new List<string>();
            using (System.Data.SQLite.SQLiteConnection connect = new System.Data.SQLite.SQLiteConnection("Data Source=" + _path + ";Version=3;"))
            {
                connect.Open();
                using (System.Data.SQLite.SQLiteCommand fmd = connect.CreateCommand())
                {
                    fmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' and name not in('sqlite_sequence','Authorization')";
                    fmd.CommandType = CommandType.Text;
                    System.Data.SQLite.SQLiteDataReader r = fmd.ExecuteReader();
                    while (r.Read())
                    {
                        ImportedFiles.Add(r.GetString(0));
                    }
                }
            }
            return ImportedFiles;
        }

        public List<Authorization> GetAuthorization()
        {
            using (var db = new SQLiteConnection(_path))
            {
                return db.Table<Authorization>().ToList();
            }
        }

        public List<ES> GetES()
        {

            using (var db = new SQLiteConnection(_path))
            {
                return db.Table<ES>().ToList();
            }
        }

        public List<CES> GetCES()
        {

            using (var db = new SQLiteConnection(_path))
            {
                return db.Table<CES>().ToList();
            }
        }

        public List<TCES> GetTCES()
        {

            using (var db = new SQLiteConnection(_path))
            {
                return db.Table<TCES>().ToList();
            }
        }

        public List<SES> GetSES()
        {

            using (var db = new SQLiteConnection(_path))
            {
                return db.Table<SES>().ToList();
            }
        }
    }

    public partial class Authorization
    {
        [PrimaryKey, AutoIncrement]
        public Int64 ID { get; set; }
        public String Login { get; set; }
        public String Password { get; set; }
    }

    public partial class ES
    {
        [PrimaryKey, AutoIncrement]
        public Int64 ID_ES { get; set; }
        public String EmergencySituation { get; set; }
    }

    public partial class CES
    {
        [PrimaryKey, AutoIncrement]
        public Int64 ID_CES { get; set; }
        public String CausesOfES { get; set; }
        public Int64 ID_ES { get; set; }
        public Int64 Tip { get; set; }
        public Int64 ParentID { get; set; }
    }

    public partial class TCES
    {
        [PrimaryKey, AutoIncrement]
        public Int64 ID_TCES { get; set; }
        public String TipOfCES { get; set; }
        public Int64 ID_CES { get; set; }
        public Int64 ParentID { get; set; }
    }

    public partial class SES
    {
        [PrimaryKey, AutoIncrement]
        public Int64 ID_SES { get; set; }
        public Int64 ID_CES { get; set; }
        public Int64 ID_TCES { get; set; }
        public String SolutionOfES { get; set; }
    }
}
