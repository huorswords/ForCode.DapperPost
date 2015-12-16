namespace ForCode.DapperPost
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using Dapper;

    public static class DapperExtensions
    {
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection)
        {
            string query = SelectSql<TEntity>();
            return connection.Query<TEntity>(query);
        }
        
        private static string SelectSql<TEntity>(string tableName = "")
        {
            Type type = typeof(TEntity);
            if (!type.GetCustomAttributes().All(a => a is TableAttribute))
            {
                throw new NotQueryableTypeException();
            }

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = type.Name;
            }

            Func<PropertyInfo, bool> predicate =
                x => x.GetCustomAttributes(true).Any(a => a is ColumnAttribute)
                     && ((x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() != typeof(ICollection<>)) || !x.PropertyType.IsGenericType);

            var properties = type.GetProperties();
            var onlyMembers = properties.Where(predicate).Select(x => string.Format("[{0}]", x.Name));
            var columns = string.Join(", ", onlyMembers);

            return string.Format("SELECT {0} FROM [{1}] ", columns, tableName);
        }
    }
}