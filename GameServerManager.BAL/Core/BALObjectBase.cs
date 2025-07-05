using GameServerManager.BAL.Services;
using GameServerManager.DAL;
using GameServerManager.DAL.Core;
using GameServerManager.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GameServerManager.BAL
{
    public class BALObjectBase<TDal> : IDisposable
    where TDal : DALObjectBase, new()
    {
        #region Properties

        protected TDal _item;

        #endregion

        #region Constructor

        public BALObjectBase()
        {
            _item = new TDal();
            _item.IsNew = true;
        }

        public BALObjectBase(TDal item)
        {
            _item = item;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Saves the current item to the database
        /// </summary>
        /// <returns>The number of rows affected</returns>
        public async Task<int> Save()
        {
            // Update the UpdatedDate before saving
            if (_item is IUpdatedObject updated)
            {
                if (_item.IsNew)
                    updated.CreatedDate = DateTime.Now;

                updated.UpdatedDate = DateTime.Now;
            }

            return await ServiceLocator.DatabaseService.SaveItemAsync(_item);
        }

        /// <summary>
        /// Deletes the current item from the database
        /// </summary>
        /// <returns>The number of rows affected</returns>
        /// <exception cref="InvalidOperationException">Thrown when trying to delete an unsaved item</exception>
        public async Task<int> Delete()
        {
            if (_item.IsNew)
                throw new InvalidOperationException("Cannot delete an item that hasn't been saved to the database yet.");

            return await ServiceLocator.DatabaseService.DeleteItemAsync(_item);
        }

        public virtual void Dispose()
        {
        }

        #endregion

        #region Static API

        internal static async Task<TBal> GetInstance<TBal>(object primaryKeyValue)
        {
            TDal item = await ServiceLocator.DatabaseService.GetInstance<TDal>(primaryKeyValue);
            TBal instance = (TBal)Activator.CreateInstance(typeof(TBal), new object[] { item });

            return instance;
        }

        internal static async Task<List<TBal>> GetAll<TBal>(List<Expression<Func<TDal, bool>>> clauses = null)
        {
            List<TDal> items = await ServiceLocator.DatabaseService.GetInstancesAsync(clauses);
            List<TBal> instances = new List<TBal>();

            foreach (var item in items)
                instances.Add((TBal)Activator.CreateInstance(typeof(TBal), new object[] { item }));

            return instances;
        }

        #endregion
    }
}
