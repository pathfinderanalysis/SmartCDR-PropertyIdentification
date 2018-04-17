using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Dapper;

namespace SmartCDR_PropertyIdenAutomation
{
    public static class UtilExtensions
    {
        public static DataTable DataTable<TSource>(this IList<TSource> data)
        {
            DataTable dataTable = new DataTable(typeof(TSource).Name);
            PropertyInfo[] props = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //Getting properties readonly attributes
            PropertyInfo[] readonlyProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => Attribute.IsDefined(p, typeof(ReadOnlyAttribute))).ToArray();

            PropertyInfo[] filteredProps = props.Except(readonlyProps).ToArray();
            foreach (PropertyInfo prop in filteredProps)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ??
                                                 prop.PropertyType);
            }

            foreach (TSource item in data)
            {
                var values = new object[filteredProps.Length];
                for (int i = 0; i < filteredProps.Length; i++)
                {
                    values[i] = filteredProps[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        /// <summary>
        /// The ToDataTable method
        /// </summary>
        /// <typeparam name="T">Generic parameter</typeparam>
        /// <param name="items">The items parameter</param>
        /// <returns>The System.Data.DataTable type object</returns>
        public static DataTable ToDataTable<T>(this T[] items)
        {
            var name = typeof(T).Name;
            string schemaName = null;
            var attribute = typeof(T).GetCustomAttribute(typeof(Dapper.TableAttribute));
            if (attribute is Dapper.TableAttribute)
            {
                schemaName = ((Dapper.TableAttribute)attribute).Schema;
            }
            if (schemaName != null)
            {
                name = string.Format("{0}.{1}", schemaName, name);
            }
            DataTable dataTable = new DataTable(name);

            // Get all the properties
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                // Setting column names as Property names
                dataTable.Columns.Add(field.Name);
            }

            // Get all the properties
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                // Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (var item in items)
            {
                var values = new object[fields.Length + props.Length];
                for (int i = 0; i < fields.Length; i++)
                {
                    // inserting field values to datatable rows
                    values[i] = fields[i].GetValue(item);
                }

                for (int i = fields.Length; i < fields.Length + props.Length; i++)
                {
                    // inserting property values to datatable rows
                    values[i] = props[i].GetValue(item);
                }

                dataTable.Rows.Add(values);
            }

            // put a breakpoint here and check datatable
            return dataTable;
        }

        /// <summary>
        /// Bulks the copy.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        public static void BulkCopy(this IDbTransaction transaction, DataTable dataTable, SqlBulkCopyOptions options)
        {
            using (
                                                        SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(
                                                        transaction.Connection as SqlConnection,
                                                        options,
                                                        transaction as SqlTransaction))
            {
                sqlBulkCopy.DestinationTableName = dataTable.TableName;
                sqlBulkCopy.BulkCopyTimeout = transaction.Connection.ConnectionTimeout;
                foreach (DataColumn column in dataTable.Columns)
                {
                    sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                try
                {
                    sqlBulkCopy.WriteToServer(dataTable);
                }
                catch (SqlException sqlex)
                {
                    sqlBulkCopy.WriteToServer(dataTable);
                }
            }
        }

        public static List<T> DataTableToList<T>(DataTable table) where T : class, new()
        {
            List<T> list = new List<T>();
            foreach (var row in table.AsEnumerable())
            {
                var obj = new T();

                foreach (var prop in obj.GetType().GetProperties())
                {
                    try
                    {
                        var propertyInfo = obj.GetType().GetProperty(prop.Name);
                        if (propertyInfo != null)
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                    }
                    catch
                    {
                        continue;
                    }
                }
                list.Add(obj);
            }
            return list;
        }
    }
}