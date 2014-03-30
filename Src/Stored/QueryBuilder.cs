using System;
using System.Collections.Generic;
using System.Text;

namespace Stored
{
    public class QueryBuilder
    {
        readonly ITableMetadata _tableMetadata;
        readonly IQuery _query;
        readonly IDictionary<string, object> _values;

        public QueryBuilder(ITableMetadata tableMetadata, IQuery query, IDictionary<string, object> values)
        {
            _tableMetadata = tableMetadata;
            _query = query;
            _values = values;
        }

        public string Build()
        {
            var builder = new StringBuilder();

            builder.AppendFormat("SELECT body");
            builder.AppendLine();
            builder.AppendFormat("FROM public.{0}", _tableMetadata.Name);
            builder.AppendLine();

            var first = true;
            foreach (var item in _query.Filters)
            {
                builder.AppendFormat("{0} {1}", first ? "WHERE" : "AND", GetFilterQuery(item));
                first = false;
            }

            builder.AppendLine();
            builder.AppendFormat("LIMIT {0} OFFSET {1};", _query.Take, _query.Skip);

            return builder.ToString();
        }

        string GetFilterQuery(IQueryFilter filter)
        {
            switch (filter.Operator)
            {
                case QueryOperator.Equal:
                    return CreateBasicComparison(filter, "=");

                case QueryOperator.NotEqual:
                    return CreateBasicComparison(filter, "!=");

                default:
                    throw new Exception("Operator not supported!");
            }
        }

        string CreateBasicComparison(IQueryFilter filter, string op)
        {
            _values.Add(":" + filter.Name, filter.Value);
            
            var type = GetJsonType(typeof(String));

            return String.Format("(body->>'{0}')::{1} {2} :{3}",
                filter.Name,
                type,
                op,
                filter.Name.ToLower());
        }

        static string GetJsonType(Type type)
        {
            // TODO: Add support for mapping types
            return "TEXT";
        }
    }
}