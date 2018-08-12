using System;
using System.Collections.Generic;
using System.Linq;
using Stored.Query;
using Stored.Query.Restrictions;

namespace Stored.Postgres.Query
{
    public class PostgresQueryVisitor
    {
        public static void Visit(PostgresSqlBuilder sql, IList<IRestriction> restrictions, bool isFirst = true)
        {
            foreach (var restriction in restrictions.IsNotQueryRestriction())
            {
                var line = new PostgresSqlBuilder();
                line.Add(isFirst ? "where " : "and ");

                Visit(line, restriction);

                sql.AddLine(line.ToString());
            }

            foreach (var restriction in restrictions.IsQueryRestriction())
            {
                VisitQuery(sql, restriction as IQueryRestriction);
            }
        }

        public static void Visit(PostgresSqlBuilder sql, IRestriction restriction)
        {
            switch (restriction)
            {
                case AndRestriction andRestriction:
                    VisitAnd(sql, andRestriction);
                    break;

                case OrRestriction orRestriction:
                    VisitOr(sql, orRestriction);
                    break;

                case EqualRestriction equalRestriction:
                    VisitEqual(sql, equalRestriction);
                    break;

                case NotEqualRestriction notEqualRestriction:
                    VisitNotEqual(sql, notEqualRestriction);
                    break;

                case NullRestriction nullRestriction:
                    VisitNull(sql, nullRestriction);
                    break;

                case NotNullRestriction notNullRestriction:
                    VisitNotNull(sql, notNullRestriction);
                    break;

                case LikeRestriction likeRestriction:
                    VisitLike(sql, likeRestriction);
                    break;

                case NotLikeRestriction notLikeRestriction:
                    VisitNotLike(sql, notLikeRestriction);
                    break;

                case InsensitiveLikeRestriction insensitiveLikeRestriction:
                    VisitInsensitiveLike(sql, insensitiveLikeRestriction);
                    break;

                case null:
                    throw new ArgumentNullException(nameof(restriction), "Restrictions should not be null.");

                default:
                    throw new ArgumentException($"Unknown restriction type '{restriction.GetType()}'.");
            }
        }

        static void VisitAnd(PostgresSqlBuilder sql, AndRestriction andRestriction)
        {
            throw new NotImplementedException();
        }

        static void VisitInsensitiveLike(PostgresSqlBuilder sql, InsensitiveLikeRestriction restriction)
        {
            sql.Add($"{JsonCast(restriction.FieldName, restriction.FieldType)} ilike {ParameterName(restriction.FieldName)}");
        }

        static void VisitNotLike(PostgresSqlBuilder sql, NotLikeRestriction restriction)
        {
            sql.Add($"{JsonCast(restriction.FieldName, restriction.FieldType)} not like {ParameterName(restriction.FieldName)}");
        }

        static void VisitLike(PostgresSqlBuilder sql, LikeRestriction restriction)
        {
            sql.Add($"{JsonCast(restriction.FieldName, restriction.FieldType)} like {ParameterName(restriction.FieldName)}");
        }

        static void VisitNotEqual(PostgresSqlBuilder sql, NotEqualRestriction restriction)
        {
            sql.Add($"{JsonCast(restriction.FieldName, restriction.FieldType)} != {ParameterName(restriction.FieldName)}");
        }

        static void VisitEqual(PostgresSqlBuilder sql, EqualRestriction restriction)
        {
            sql.Add($"{JsonCast(restriction.FieldName, restriction.FieldType)} = {ParameterName(restriction.FieldName)}");
        }

        static void VisitNotNull(PostgresSqlBuilder sql, NotNullRestriction restriction)
        {
            sql.Add($"{restriction.FieldName.ToLower()} is not null");
        }

        static void VisitNull(PostgresSqlBuilder sql, NullRestriction restriction)
        {
            sql.Add($"{restriction.FieldName.ToLower()} is null");
        }

        static void VisitQuery(PostgresSqlBuilder sql, IQueryRestriction restriction)
        {
            switch (restriction)
            {
                case SkipRestriction skipRestriction:
                    VisitSkip(sql, skipRestriction);
                    break;

                case TakeRestriction takeRestriction:
                    VisitTake(sql, takeRestriction);
                    break;

                case null:
                    throw new ArgumentNullException(nameof(restriction), "Restrictions should not be null.");

                default:
                    throw new ArgumentException($"Unknown query restriction type '{restriction.GetType()}'.");
            }
        }

        static void VisitTake(PostgresSqlBuilder sql, TakeRestriction restriction)
        {
            sql.AddLine($"limit {restriction.Count}");
        }

        static void VisitSkip(PostgresSqlBuilder sql, SkipRestriction restriction)
        {
            sql.AddLine($"offset {restriction.Count}");
        }

        static string JsonCast(string fieldName, Type type)
        {
            return fieldName;
        }

        static string ParameterName(string fieldName)
        {
            return $":{fieldName.ToLower()}";
        }
    }
}
