using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameServerManager.BAL
{
    public static class TypeUtil
    {
        /// <summary>
        /// Gets a value that indicates whether a property has a certain attribute
        /// </summary>
        /// <typeparam name="T">The type of the attribute</typeparam>
        /// <param name="prop">The property</param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this PropertyInfo prop)
        where T : Attribute
        {
            return prop.GetCustomAttribute<T>() != null;
        }

        /// <summary>
        /// Copies values from a source object to a destination object of the same type
        /// </summary>
        /// <typeparam name="T">The type of the objects</typeparam>
        /// <param name="source">The source object</param>
        /// <param name="destination">The destination object</param>
        /// <param name="predicate">A predicate used to filter which properties will be copied</param>
        public static void CopyPropertiesTo<T>(this T source, T destination, Func<PropertyInfo, bool> predicate = null)
        {
            if (source == null || destination == null)
                return;

            var properties = typeof(T).GetProperties();

            if (predicate != null)
                properties = properties.Where(predicate).ToArray();

            foreach (var property in properties)
            {
                if (property.CanWrite && property.CanRead)
                {
                    var value = property.GetValue(source);
                    property.SetValue(destination, value);
                }
            }
        }
    }
}
