using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UWEServer.Model;

namespace UWEServer.Model
{
    public class PaginationHelperResponse
    {
        private const string partern = @"\[[a-zA-Z0-9\s<>,.?/:;""'\|\\+=_\-\(\)*&^%$#@!]*[a-zA-Z0-9\s<>,.?/:;""'\|\\+= _\-\(\)*&^%$#@!]\]";
        public static PagedResponse<T> CreatePagedReponse<T>(IQueryable<T> query, PaginationFilter validFilter, int totalRecords, string route)
        {
            if (validFilter.Skip != null)
                query = query
                    .Skip((int)validFilter.Skip);
            totalRecords = query.Count();
            if (totalRecords <= (validFilter.PageNumber - 1) * validFilter.Take)
                validFilter.PageNumber = 1;
            var respose = new PagedResponse<T>(query, validFilter);

            if (validFilter.Take > 0)
            {
                var totalPages = ((double)totalRecords / (double)validFilter.Take);
                int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
                respose.TotalPages = roundedTotalPages;
            }
            else
            {
                respose.TotalPages = 0;
            }
            respose.TotalRecords = totalRecords;
            return respose;
        }

        /// <summary>
        /// Get data filter
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static List<DataFilter> GetDataFilter(string content)
        {
            List<DataFilter> listFilter = new List<DataFilter>();
            if (string.IsNullOrEmpty(content))
                return listFilter;
            var myRegex = new Regex(partern);
            MatchCollection match = myRegex.Matches(content);
            for (int i = 0; i < match.Count; i++)
            {
                var data = Regex.Replace(match[i].Value, @"[{}]", string.Empty);
                try
                {
                    var dataConver = JsonConvert.DeserializeObject<List<string>>(data);
                    if (dataConver.Count == 3)
                    {
                        listFilter.Add(new DataFilter()
                        {
                            file_name = dataConver[0],
                            type = dataConver[1],
                            value = dataConver[2],
                        });
                    }
                }
                catch (Exception e)
                {

                }
            }
            return listFilter;
        }

        /// <summary>
        /// Query and sort
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <returns></returns>
        public static IQueryable<T> QueryInTable<T>(IQueryable<T> query, List<DataFilter> filters, List<DataSort> sorts)
        {
            // Query
            if (filters?.Count > 0)
            {
                foreach (var filter in filters)
                {
                    try
                    {
                        var column = typeof(T).GetProperties().FirstOrDefault(m => m.Name.ToLower() == filter.file_name.ToLower());
                        if (column != null)
                        {
                            if (column.PropertyType == typeof(string))
                            {
                                query = QueryString(query, column.Name, filter.type, filter.value);
                            }
                            if (column.PropertyType == typeof(short) || column.PropertyType == typeof(short?))
                            {
                                var value = Convert.ToInt16(filter.value);
                                query = QueryShort(query, column.Name, filter.type, value);
                            }
                            if (column.PropertyType == typeof(int) || column.PropertyType == typeof(int?))
                            {
                                var value = Convert.ToInt32(filter.value);
                                query = QueryInt32(query, column.Name, filter.type, value);
                            }
                            if (column.PropertyType == typeof(float) || column.PropertyType == typeof(float?))
                            {
                                var value = float.Parse(filter.value);
                                query = QueryIntFloat(query, column.Name, filter.type, value);
                            }
                            if (column.PropertyType == typeof(long) || column.PropertyType == typeof(long?))
                            {
                                var value = Convert.ToInt64(filter.value);
                                query = QueryInt64(query, column.Name, filter.type, value);
                            }
                            if (column.PropertyType == typeof(bool) || column.PropertyType == typeof(bool?))
                            {
                                var value = Convert.ToBoolean(filter.value);
                                query = QueryBool(query, column.Name, filter.type, value);
                            }
                            if (column.PropertyType == typeof(DateTime?) || column.PropertyType == typeof(DateTime) || column.PropertyType == typeof(DateTimeOffset?) || column.PropertyType == typeof(DateTimeOffset))
                            {
                                var value = Convert.ToDateTime(filter.value);
                                query = QueryDateTime(query, column.Name, filter.type, value);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.ToString());
                    }
                }
            }
            // Sort
            if (sorts?.Count > 0)
            {
                foreach (var sort in sorts)
                {
                    var column = typeof(T).GetProperties().FirstOrDefault(m => m.Name.ToLower() == sort.selector.ToLower());
                    if (column != null)
                    {
                        if (sort.desc)
                        {
                            query = query.OrderByDescending(m => EF.Property<T>(m, column.Name));
                        }
                        else
                        {
                            query = query.OrderBy(m => EF.Property<T>(m, column.Name));
                        }
                    }
                }
            }
            return query;
        }

        /// <summary>
        /// Query type string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="clumnName"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        private static IQueryable<T> QueryString<T>(IQueryable<T> query, string clumnName, string type, string value)
        {
            switch (type)
            {
                case "contains":
                    query = query.Where(m => EF.Property<string>(m, clumnName).Contains(value));
                    break;
                case "notcontains":
                    query = query.Where(m => !EF.Property<string>(m, clumnName).Contains(value));
                    break;
                case "=":
                    query = query.Where(m => EF.Property<string>(m, clumnName) == value);
                    break;
                case "<>":
                    query = query.Where(m => EF.Property<string>(m, clumnName) != value);
                    break;
                case "startswith":
                    query = query.Where(m => EF.Property<string>(m, clumnName).StartsWith(value));
                    break;
                case "endswith":
                    query = query.Where(m => EF.Property<string>(m, clumnName).EndsWith(value));
                    break;
                default:
                    break;
            }
            return query;
        }

        /// <summary>
        /// Query type short
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="clumnName"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        private static IQueryable<T> QueryShort<T>(IQueryable<T> query, string clumnName, string type, short value)
        {
            switch (type)
            {
                case "=":
                    query = query.Where(m => EF.Property<short>(m, clumnName) == value);
                    break;
                case "<>":
                    query = query.Where(m => EF.Property<short>(m, clumnName) != value);
                    break;
                case "<":
                    query = query.Where(m => EF.Property<short>(m, clumnName) < value);
                    break;
                case "<=":
                    query = query.Where(m => EF.Property<short>(m, clumnName) <= value);
                    break;
                case ">":
                    query = query.Where(m => EF.Property<short>(m, clumnName) > value);
                    break;
                case ">=":
                    query = query.Where(m => EF.Property<short>(m, clumnName) >= value);
                    break;
                default:
                    break;
            }
            return query;
        }

        /// <summary>
        /// Query type int32
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="clumnName"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        private static IQueryable<T> QueryInt32<T>(IQueryable<T> query, string clumnName, string type, int value)
        {
            switch (type)
            {
                case "=":
                    query = query.Where(m => EF.Property<int>(m, clumnName) == value);
                    break;
                case "<>":
                    query = query.Where(m => EF.Property<int>(m, clumnName) != value);
                    break;
                case "<":
                    query = query.Where(m => EF.Property<int>(m, clumnName) < value);
                    break;
                case "<=":
                    query = query.Where(m => EF.Property<int>(m, clumnName) <= value);
                    break;
                case ">":
                    query = query.Where(m => EF.Property<int>(m, clumnName) > value);
                    break;
                case ">=":
                    query = query.Where(m => EF.Property<int>(m, clumnName) >= value);
                    break;
                default:
                    break;
            }
            return query;
        }

        /// <summary>
        /// Query type float
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="clumnName"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        private static IQueryable<T> QueryIntFloat<T>(IQueryable<T> query, string clumnName, string type, float value)
        {
            switch (type)
            {
                case "=":
                    query = query.Where(m => EF.Property<float>(m, clumnName) == value);
                    break;
                case "<>":
                    query = query.Where(m => EF.Property<float>(m, clumnName) != value);
                    break;
                case "<":
                    query = query.Where(m => EF.Property<float>(m, clumnName) < value);
                    break;
                case "<=":
                    query = query.Where(m => EF.Property<float>(m, clumnName) <= value);
                    break;
                case ">":
                    query = query.Where(m => EF.Property<float>(m, clumnName) > value);
                    break;
                case ">=":
                    query = query.Where(m => EF.Property<float>(m, clumnName) >= value);
                    break;
                default:
                    break;
            }
            return query;
        }

        /// <summary>
        /// Query type int64
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="clumnName"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        private static IQueryable<T> QueryInt64<T>(IQueryable<T> query, string clumnName, string type, long value)
        {
            switch (type)
            {
                case "=":
                    query = query.Where(m => EF.Property<long>(m, clumnName) == value);
                    break;
                case "<>":
                    query = query.Where(m => EF.Property<long>(m, clumnName) != value);
                    break;
                case "<":
                    query = query.Where(m => EF.Property<long>(m, clumnName) < value);
                    break;
                case "<=":
                    query = query.Where(m => EF.Property<long>(m, clumnName) <= value);
                    break;
                case ">":
                    query = query.Where(m => EF.Property<long>(m, clumnName) > value);
                    break;
                case ">=":
                    query = query.Where(m => EF.Property<long>(m, clumnName) >= value);
                    break;
                default:
                    break;
            }
            return query;
        }

        /// <summary>
        /// Query type boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="clumnName"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        private static IQueryable<T> QueryBool<T>(IQueryable<T> query, string clumnName, string type, bool value)
        {
            switch (type)
            {
                case "=":
                    query = query.Where(m => EF.Property<bool>(m, clumnName) == value);
                    break;
                case "<>":
                    query = query.Where(m => EF.Property<bool>(m, clumnName) != value);
                    break;
                default:
                    break;
            }
            return query;
        }

        /// <summary>
        /// Query type datetime
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="clumnName"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        private static IQueryable<T> QueryDateTime<T>(IQueryable<T> query, string clumnName, string type, DateTime value)
        {
            switch (type)
            {
                case "=":
                    query = query.Where(m => EF.Property<DateTime>(m, clumnName) == value);
                    break;
                case "<>":
                    query = query.Where(m => EF.Property<DateTime>(m, clumnName) != value);
                    break;
                case "<":
                    query = query.Where(m => EF.Property<DateTime>(m, clumnName) < value);
                    break;
                case "<=":
                    query = query.Where(m => EF.Property<DateTime>(m, clumnName) <= value);
                    break;
                case ">":
                    query = query.Where(m => EF.Property<DateTime>(m, clumnName) > value);
                    break;
                case ">=":
                    query = query.Where(m => EF.Property<DateTime>(m, clumnName) >= value);
                    break;
                default:
                    break;
            }
            return query;
        }
    }
}
