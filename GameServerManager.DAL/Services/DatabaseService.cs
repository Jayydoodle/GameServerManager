using GameServerManager.DAL.Core;
using SQLite;
using System.Linq.Expressions;
using System.Reflection;

namespace GameServerManager.DAL.Services
{
    public class DatabaseService
    {
        #region Constants

        public const string DatabaseFilename = "app.db";

        #endregion

        #region Properties

        private SQLiteAsyncConnection Connection { get; set; }
        private string DatabaseFilepath { get; set; }

        #endregion

        #region Constructor

        public DatabaseService(string databaseFilePath)
        {
            DatabaseFilepath = databaseFilePath;
        }

        #endregion

        #region Public API

        public async Task Init()
        {
            if (Connection is not null)
                return;

            Connection = new SQLiteAsyncConnection(Path.Combine(DatabaseFilepath, DatabaseFilename));
            _ = await Connection.CreateTableAsync<DALGameServer>();
        }

        public async Task<int> SaveItemAsync<TDal>(TDal item)
        where TDal : DALObjectBase
        {
            await Init();

            if (item.IsNew)
            {
                item.IsNew = false;
                return await Connection.InsertAsync(item);
            }
            else
            {
                return await Connection.UpdateAsync(item);
            }
        }

        public async Task<int> DeleteItemAsync<TDal>(TDal item)
        where TDal : DALObjectBase
        {
            await Init();
            return await Connection.DeleteAsync(item);
        }

        public async Task<TDal> GetInstance<TDal>(object primaryKeyValue)
        where TDal : DALObjectBase, new()
        {
            PropertyInfo primaryKeyProp = 
                typeof(TDal)
                .GetProperties()
                .Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(PrimaryKeyAttribute)))
                .FirstOrDefault();

            if (primaryKeyProp == null)
                throw new Exception(string.Format("The entity '{0}' lacks a primary key", nameof(TDal)));

            var param = Expression.Parameter(typeof(TDal), "x");
            var prop = Expression.Property(param, primaryKeyProp.Name);
            var value = Expression.Constant(Convert.ChangeType(primaryKeyValue, primaryKeyProp.PropertyType));

            BinaryExpression equal = Expression.Equal(prop, value);

            var lambda = Expression.Lambda<Func<TDal, bool>>(equal, param);
            var items = await GetInstancesAsync(new List<Expression<Func<TDal, bool>>> { lambda });

            return items.FirstOrDefault();
        }

        public async Task<List<TDal>> GetInstancesAsync<TDal>(List<Expression<Func<TDal, bool>>> clauses = null)
         where TDal : DALObjectBase, new()
        {
            await Init();
            List<TDal> items = new List<TDal>();

            var query = Connection.Table<TDal>();

            if (clauses != null)
                clauses.ForEach(x => query = query.Where(x));

            items = await query.ToListAsync();
            return items;
        }

        #endregion
    }
}
