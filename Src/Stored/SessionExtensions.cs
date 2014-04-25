using System.Collections.Generic;

namespace Stored
{
    public static class SessionExtensions
    {
        public static void CreateAll<T>(this ISession session, IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                session.Create(item);
            }
        }

        public static void ModifyAll<T>(this ISession session, IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                session.Modify(item);
            }
        }

        public static void DeleteAll<T>(this ISession session, IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                session.Delete(item);
            }
        }
    }
}
