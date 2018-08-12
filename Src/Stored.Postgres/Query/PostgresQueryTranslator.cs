using System;
using System.Collections.Generic;
using System.Text;
using Stored.Query;

namespace Stored.Postgres.Query
{
    internal class PostgresQueryTranslator
    {
        public string Translate(
            LegacyRestrictions restrictions,
            Dictionary<string, object> parameters,
            ITableMetadata tableMetadata,
            bool hasStats)
        {
            var builder = new PostgresSqlBuilder();
            builder.AddLine(hasStats
                ? "select body, count(1) over() as total_rows"
                : "select body");
            builder.AddLine($"from public.{tableMetadata.Name}");

            var first = true;
            foreach (var item in restrictions.Filters)
            {
                builder.Add(first
                    ? $"where {GetFilter(item, parameters)}"
                    : $"and {GetFilter(item, parameters)}");

                if (first)
                {
                    first = false;
                }
            }

            if (restrictions.Filters.Count > 0)
            {
                builder.AddLine();
            }

            if (!string.IsNullOrWhiteSpace(restrictions.SortClause.FieldName))
            {
                var fieldName = restrictions.SortClause.FieldName;
                var sortDirection = restrictions.SortClause.SortOrder == SortOrder.Ascending
                    ? "asc"
                    : "desc";

                switch (restrictions.SortClause.SortType)
                {
                    case SortType.Undefined:
                        //do nothing
                        break;

                    case SortType.Date:
                        builder.Add($"ORDER BY CAST(CAST(body->'{fieldName}' as TEXT) as DATE) {sortDirection}");
                        break;

                    case SortType.Number:
                        // TODO: replace fixed number size with more dynamic.
                        builder.Add($"ORDER BY to_number((body->'{fieldName}')::TEXT, '9999999999999999') {sortDirection}");
                        break;

                    default:
                        // Text will always be the default conversion
                        builder.Add($"ORDER BY (body->'{fieldName}')::TEXT {sortDirection}");
                        break;
                }
            }

            builder.AddLine();

            if (restrictions.Take > 0)
            {
                builder.Add($"limit {restrictions.Take}");
            }

            if (restrictions.Skip > 0)
            {
                builder.AddLine();
                builder.Add($"offset {restrictions.Skip}");
            }

            builder.Add(";");
            return builder.ToString();
        }

        static string GetFilter(FilterBase filter, IDictionary<string, object> parameters)
        {
            if (filter is BinaryFilter binaryFilter)
            {
                return GetBinaryComparison(binaryFilter, parameters);
            }

            throw new Exception($"Filter type {filter.GetType().Name} not supported.");
        }

        static string GetBinaryComparison(BinaryFilter filter, IDictionary<string, object> parameters)
        {
            parameters.Add(":" + filter.FieldName.ToLower(), TypeHelper.GetUnderlyingValue(filter.Value).ToString());

            var type = GetJsonType(typeof(string));

            return $"(body->>'{filter.FieldName}')::{type} {GetOperator(filter.Operator)} :{filter.FieldName.ToLower()}";
        }

        static string GetJsonType(Type type)
        {
            if (type == typeof(string))
            {
                return "TEXT";
            }

            if (type == typeof(Guid))
            {
                return "UUID";
            }

            if (type == typeof(int))
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
                    throw new ArgumentOutOfRangeException(nameof(binaryOperator));
            }
        }
    }
}
