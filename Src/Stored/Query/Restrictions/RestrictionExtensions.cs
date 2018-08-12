using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace Stored.Query.Restrictions
{
    public static class Restriction
    {
        public static void SetSkip(this IImmutableList<IRestriction> restrictions, long value)
        {
            restrictions.RemoveAll(x => x is SkipRestriction);
            restrictions.Add(new SkipRestriction(value));
        }

        public static void SetTake(this IImmutableList<IRestriction> restrictions, long value)
        {
            restrictions.RemoveAll(x => x is TakeRestriction);
            restrictions.Add(new TakeRestriction(value));
        }

        public static long GetSkip(this IEnumerable<IRestriction> restrictions)
        {
            var skipRestriction = restrictions.Where(x => x is SkipRestriction)
                .Cast<SkipRestriction>()
                .FirstOrDefault();

            return skipRestriction?.Count ?? 0;
        }

        public static long GetTake(this IEnumerable<IRestriction> restrictions)
        {
            var takeRestriction = restrictions.Where(x => x is TakeRestriction)
                .Cast<TakeRestriction>()
                .FirstOrDefault();

            return takeRestriction?.Count ?? 0;
        }

        public static IEnumerable<IRestriction> IsQueryRestriction(this IEnumerable<IRestriction> restrictions) =>
            restrictions.Where(x => x is IQueryRestriction);

        public static IEnumerable<IRestriction> IsNotQueryRestriction(this IEnumerable<IRestriction> restrictions) =>
            restrictions.Where(x => !(x is IQueryRestriction));

        public static IQuery<T> WhereTrue<T>(this IQuery<T> query, Expression<Func<T, bool>> expression)
        {
            var fieldDefinition = expression.AsField();
            query.Restrictions.Add(new EqualRestriction(fieldDefinition.Name, typeof(bool), true));

            return query;
        }

        public static IQuery<T> WhereFalse<T>(this IQuery<T> query, Expression<Func<T, bool>> expression)
        {
            var fieldDefinition = expression.AsField();
            query.Restrictions.Add(new EqualRestriction(fieldDefinition.Name, typeof(bool), false));

            return query;
        }

        public static IQuery<T> WhereEqual<T>(this IQuery<T> query, Expression<Func<T, object>> expression, object value)
        {
            if (value == null)
            {
                return WhereIsNull(query, expression);
            }

            var fieldDefinition = expression.AsField();
            query.Restrictions.Add(new EqualRestriction(fieldDefinition.Name, fieldDefinition.Type, value));

            return query;
        }

        public static IQuery<T> WhereNotEqual<T>(this IQuery<T> query, Expression<Func<T, object>> expression, object value)
        {
            if (value == null)
            {
                return WhereIsNotNull(query, expression);
            }

            var fieldDefinition = expression.AsField();
            query.Restrictions.Add(new NotEqualRestriction(fieldDefinition.Name, fieldDefinition.Type, value));

            return query;
        }

        public static IQuery<T> WhereIsNull<T>(this IQuery<T> query, Expression<Func<T, object>> expression)
        {
            var fieldDefinition = expression.AsField();
            query.Restrictions.Add(new NullRestriction(fieldDefinition.Name));

            return query;
        }

        public static IQuery<T> WhereIsNotNull<T>(this IQuery<T> query, Expression<Func<T, object>> expression)
        {
            var fieldDefinition = expression.AsField();
            query.Restrictions.Add(new NotNullRestriction(fieldDefinition.Name));

            return query;
        }
    }
}
