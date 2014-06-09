using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using IWantTo.Client.Core.DataStorage.Contract;
using IWantTo.Client.Core.DataStorage.Model;
using IWantTo.Client.Core.DataStorage.SQLite;
using IWantTo.Client.Core.Utils;

namespace IWantTo.Client.Core.DataStorage
{
    /// <summary>
    /// JobDispatchDatabase builds on SQLite.Net and represents a specific database, in our case,
    /// the Job Dispatch DB. It contains methods for retrieval and persistence as well as db
    /// creation, all based on the underlying ORM.
    /// 
    /// </summary>
    public class IWantToDatabase : SQLiteConnection
    {
        /// <summary>Logger.</summary>
        private static readonly Logger _log = Logger.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly IWantToDatabase _db;
        private static readonly object _locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="IWantToDatabase"/>. 
        /// If the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        private IWantToDatabase(string path) : base(path)
        {
        }

        static IWantToDatabase()
        {
            _db = new IWantToDatabase(DatabaseFilePath);

            // apply alter scripts
            AlterDatabase();
        }

        public static string DatabaseFilePath
        {
            get
            {
#if __ANDROID__
                // just use whatever directory SpecialFolder.Personal returns
                var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); 
#elif __IOS__
                // we need to put in /Library/ on iOS 5.1 to meet Apple's iCloud terms
                // (they don't want non-user-generated data in Documents)
                var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
                var libraryPath = Path.Combine (documentsPath, "../Library/"); // Library folder
#else
                var libraryPath = string.Empty;
#endif
                return Path.Combine(libraryPath, "IWantTo.db3");
            }
        }

        public static bool TableExists<T>()
        {
            lock (_locker)
            {
                var query = string.Format("select count(*) from sqlite_master where type='table' and name='{0}'", typeof(T).Name);
                return _db.ExecuteScalar<int>(query) > 0;
            }
        }

        public static IEnumerable<string> GetTableNames()
        {
            lock (_locker)
            {
                var names = new List<string>();

                var query = string.Format("select name from sqlite_master where type='table' and name not like 'sqlite_%'"); // get all table names except sqlite system tables
                var statement = SQLite3.Prepare2(_db.Handle, query);

                while (SQLite3.Step(statement) == SQLite3.Result.Row)
                {
                    var name = SQLite3.ColumnString(statement, 0);
                    names.Add(name);
                }

                SQLite3.Finalize(statement);
                return names;
            }
        }

        public static long GetLastRowId<T>()
        {
            lock (_locker)
            {
                if (_db.ExecuteScalar<int>("select count(*) from sqlite_master where type='table' and name='sqlite_sequence'") == 0)
                {
                    return 0;
                }

                var query = string.Format("select seq from sqlite_sequence where name='{0}'", typeof(T).Name);
                return _db.ExecuteScalar<long>(query);
            }
        }

        public static IEnumerable<T> GetItems<T>() where T : IBusinessEntity, new()
        {
            lock (_locker)
            {
                return (from i in _db.Table<T>() select i).ToList();
            }
        }

        public static TableQuery<T> GetItems<T>(Expression<Func<T, bool>> predExpr) where T : IBusinessEntity, new()
        {
            lock (_locker)
            {
                return _db.Table<T>().Where(predExpr);
            }
        }

        public static T GetItem<T>(long id) where T : IBusinessEntity, new()
        {
            lock (_locker)
            {
                return _db.Find<T>(id);
            }
        }

        public static long InsertItem<T>(T item) where T : IBusinessEntity, new()
        {
            lock (_locker)
            {
                return _db.Insert(item);
            }
        }

        public static long UpdateItem<T>(T item) where T : IBusinessEntity, new()
        {
            lock (_locker)
            {
                return _db.Update(item);
            }
        }

        public static void InsertItems<T>(IEnumerable<T> items) where T : IBusinessEntity, new()
        {
            lock (_locker)
            {
                _db.BeginTransaction();

                foreach (var item in items)
                {
                    InsertItem(item);
                }

                _db.Commit();
            }
        }

        public static long DeleteItem<T>(long id) where T : IBusinessEntity, new()
        {
            lock (_locker)
            {
                return _db.Delete<T>(id);
            }
        }

        public static void DeleteTable<T>() where T : IBusinessEntity, new()
        {
            lock (_locker)
            {
                _db.DeleteAll<T>();
            }
        }

        public static void TruncateTable<T>() where T : IBusinessEntity, new()
        {
            lock (_locker)
            {
                _db.DropTable<T>();
                _db.CreateTable<T>();
            }
        }

        public static int CountTable<T>() where T : IBusinessEntity, new()
        {
            lock (_locker)
            {
                return _db.Table<T>().Count();
            }
        }

        public static int CountTable<T>(Expression<Func<T, bool>> predExpr) where T : IBusinessEntity, new()
        {
            lock (_locker)
            {
                return _db.Table<T>().Count(predExpr);
            }
        }

        private static void CreateTables()
        {
            _db.CreateTable<Configuration>();
        }

        private static void DropTables()
        {
            var names = GetTableNames();
            foreach (var name in names)
            {
                DropTable(name);
            }
        }

        private static void DropTable(string name)
        {
            lock (_locker)
            {
                var query = string.Format("drop table if exists '{0}'", name);
                _db.Execute(query);
            }
        }

        #region Configuration

        public static Configuration GetConfiguration()
        {
            lock (_locker)
            {
                return _db.Table<Configuration>().FirstOrDefault();
            }
        }

        public static long InsertConfiguration(Configuration configuration)
        {
            return InsertItem(configuration);
        }

        public static T GetConfiguration<T>(string columnName)
        {
            lock (_locker)
            {
                var ret = _db.ExecuteScalar<T>("select " + columnName + " from Configuration");
                return ret;
            }
        }

        public static void UpdateConfiguration<T>(string columnName, T value)
        {
            lock (_locker)
            {
                _db.Execute("update Configuration set " + columnName + " = ?1", value);
            }
        }

        #endregion Configuration

        #region Log

        /// <inheritdoc />
        protected override void Log(string text, params object[] args)
        {
            _log.InfoFormat(text, args);
        }

        #endregion

        #region Alter Scripts

        private static void AlterDatabase()
        {
            if (!TableExists<Configuration>())
            {
                _log.InfoFormat("Table \"Configuration\" does not exist. Database will be recreated from scratch.");

                RecreateDatabase();
            }

            try
            {
                while (true)
                {
                    var configuration = GetConfiguration();

                    _log.InfoFormat("Database version: {0}", configuration.DatabaseVersion);

                    if (configuration.DatabaseVersion == CoreConstants.DATABASE_VERSION)
                    {
                        break;
                    }

                    switch (configuration.DatabaseVersion)
                    {
                        //case 2:
                        //{
                        //    Alter0003();
                        //    break;
                        //}
                        default:
                        {
                            RecreateDatabase();
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                _log.ErrorFormat("Unable to alter database! Database will be recreated from scratch.");

                RecreateDatabase();
            }
        }

        private static void RecreateDatabase()
        {
            _log.InfoFormat("Recreating database...");
            DropTables();
            CreateTables();
            _log.InfoFormat("Recreating database OK.");
            InsertConfiguration(new Configuration(CoreConstants.DATABASE_VERSION));
        }

        #endregion Alter Scripts
    }
}
