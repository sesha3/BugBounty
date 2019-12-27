namespace Bug.Bounty.DataClasses
{
    /// <summary>
    ///     Model class for Joined column
    /// </summary>
    public class JoinColumn
    {
        private Condition operation = Condition.Equals;

        public object ConditionValue
        {
            get;
            set;
        }

        public string JoinedColumn
        {
            get;
            set;
        }

        public LogicalOperator LogicalOperator
        {
            get;
            set;
        }

        public Condition Operation
        {
            get
            {
                return this.operation;
            }

            set
            {
                this.operation = value;
            }
        }

        public string ParentTable
        {
            get;
            set;
        }

        public string ParentTableColumn
        {
            get;
            set;
        }

        public string TableName
        {
            get;
            set;
        }
    }
}