﻿using System;
using System.Collections.Generic;
using System.Text;
using Stored.Query;

namespace Stored.Postgres.Query
{
    internal class PostgresQueryTranslator
    {
        public string Translate(
            Restrictions restrictions, 
            Dictionary<string, object> parameters,
            ITableMetadata tableMetadata)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("SELECT body FROM public.{0}", tableMetadata.Name);

            var first = true;
            foreach (var item in restrictions.Filters)
            {
                builder.AppendLine();
                builder.AppendFormat("{0} {1}", 
                    first ? "WHERE " : "AND ",
                    GetFilter(item, parameters));

                if (first)
                {
                    first = false;
                }
            }

            if (!String.IsNullOrWhiteSpace(restrictions.SortClause.FieldName))
            {
                builder.AppendLine();
                var sortClause = String.Empty;
                switch (restrictions.SortClause.SortType)
                {
                    case SortType.Undefined:
                        //do nothing
                        break;
                    
                    case SortType.Date:
                        sortClause = "ORDER BY CAST(CAST(body->'{0}' as TEXT) as DATE) {1}";
                        break;
                    case SortType.Number:
                        //TODO: replace fixed number size with more dynamic.
                        sortClause = "ORDER BY to_number((body->'{0}')::TEXT, '9999999999999999') {1}";
                        break;
                    
                    default:
                        //Text will always be the default conversion
                        sortClause = "ORDER BY (body->'{0}')::TEXT {1}";
                        break;
                }

                builder.AppendFormat(sortClause, restrictions.SortClause.FieldName,
                    restrictions.SortClause.SortOrder == SortOrder.Ascending ? "" : "DESC");
            }

            if (restrictions.Take > 0)
            {
                builder.AppendLine();
                builder.AppendFormat("LIMIT {0}", restrictions.Take);
            }

            if (restrictions.Skip > 0)
            {
                builder.AppendLine();
                builder.AppendFormat("OFFSET {0}", restrictions.Skip);
            }

            builder.Append(";");

            return builder.ToString();
        }

        static string GetFilter(FilterBase filter, Dictionary<string, object> parameters)
        {
            var binaryFilter = filter as BinaryFilter;
            if (binaryFilter != null)
            {
                return GetBinaryComparison(binaryFilter, parameters);
            }

            throw new Exception(String.Format("Filter type {0} not supported.", filter.GetType().Name));
        }

        static string GetBinaryComparison(BinaryFilter filter, Dictionary<string, object> parameters)
        {
            parameters.Add(":" + filter.FieldName, filter.Value);

            var type = GetJsonType(typeof(String));

            return String.Format("(body->>'{0}')::{1} {2} :{3}",
                filter.FieldName,
                type,
                GetOperator(filter.Operator),
                filter.FieldName.ToLower());
        }

        static string GetJsonType(Type type)
        {
            if (type == typeof(String))
            {
                return "TEXT";
            }

            if (type == typeof (Guid))
            {
                return "UUID";
            }

            if (type == typeof (int))
            {
                return "INTEGER";
            }

            return "TEXT";
        }

        static string GetOperator(BinaryOperator binaryOperator)
        {
            switch (binaryOperator)
            {
                case BinaryOperator.Equal:
                    return "=";
                case BinaryOperator.NotEqual:
                    return "!=";
                case BinaryOperator.LessThan:
                    return "<";
                case BinaryOperator.LessThanOrEqual:
                    return "<=";
                case BinaryOperator.GreaterThan:
                    return ">";
                case BinaryOperator.GreaterThanOrEqual:
                    return ">=";
                default:
                    throw new ArgumentOutOfRangeException("binaryOperator");
            }
        }
    }
}