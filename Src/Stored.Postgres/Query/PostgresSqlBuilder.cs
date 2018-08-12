using System.Text;

namespace Stored.Postgres.Query
{
    public class PostgresSqlBuilder
    {
        readonly StringBuilder _builder = new StringBuilder();

        public PostgresSqlBuilder() { }

        public PostgresSqlBuilder(string sql) => _builder.Append(sql);

        public void Add(string text) => _builder.Append(text);

        public void AddLine() => _builder.AppendLine();

        public void AddLine(string text) => _builder.AppendLine(text);

        public void AddWithWrap(string text) => _builder.Append($"({text})");

        public void AddLineWithWrap(string text) => _builder.AppendLine($"({text})");

        public override string ToString() => _builder.ToString();
    }
}
