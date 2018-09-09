namespace Stored.Query
{
    public class SortClause
    {
        public SortClause()
        {
            SortOrder = SortOrder.Ascending;
            SortType = SortType.Text;
        }

        public string FieldName { get; set; }
        public SortType SortType { get; set; }
        public SortOrder SortOrder { get; set; }

        public SortClause Clone() =>
            new SortClause
                {
                    SortType =  SortType,
                    SortOrder = SortOrder,
                    FieldName = FieldName?.Clone() as string
                };
    }
}
