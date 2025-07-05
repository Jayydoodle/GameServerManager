using GameServerManager.DAL.Services;

namespace GameServerManager.BAL.Services
{
    public static class ServiceLocator
    {
        #region Properties

        private static DatabaseService _databaseService;

        #endregion

        #region Public API

        /// <summary>
        /// Gets the current database service instance
        /// </summary>
        public static DatabaseService DatabaseService
        {
            get
            {
                if (_databaseService == null)
                    throw new InvalidOperationException("DatabaseService has not been registered. Call ServiceLocator.RegisterDatabaseService() first.");
                return _databaseService;
            }
        }

        /// <summary>
        /// Registers the database service instance
        /// </summary>
        /// <param name="databaseService">The database service to register</param>
        public static void RegisterDatabaseService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Clears all registered services (useful for testing)
        /// </summary>
        public static void Clear()
        {
            _databaseService = null;
        }

        #endregion
    }
}
