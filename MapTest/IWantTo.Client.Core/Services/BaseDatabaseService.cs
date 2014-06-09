using System;
using IWantTo.Client.Core.Utils;

namespace IWantTo.Client.Core.Services
{
    /// <summary>
    /// Base class for all services which working with database.
    /// </summary>
    public class BaseDatabaseService
    {
        /// <summary>Logger of derived class.</summary>
        private static Logger _log;

        /// <summary>
        /// Default constructor is deprecated.
        /// </summary>
        private BaseDatabaseService()
        {
        }

        /// <summary>
        /// Constructs service with provided logger.
        /// </summary>
        /// <param name="log"></param>
        public BaseDatabaseService(Logger log)
        {
            _log = log;
        }

        /// <summary>
        /// Wrapper for method using database. Here are handled all exceptions from database and logged.
        /// </summary>
        /// <param name="action">Preferably anonymous method.</param>
        public void UseDatabase(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Database exception Ex = {0}", ex.ToString());
            }
        }
    }
}
