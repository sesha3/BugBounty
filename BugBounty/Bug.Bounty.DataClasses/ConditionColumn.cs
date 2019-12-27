namespace Bug.Bounty.DataClasses
{ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class ConditionColumn
    {
        /// <summary>
        ///     Gets or sets the Aggregation Method
        /// </summary>
        public AggregateMethod Aggregation
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets Column Name
        /// </summary>
        public string ColumnName
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the Operator condition
        /// </summary>
        public Condition Condition
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the Logical Operator
        /// </summary>
        public LogicalOperator LogicalOperator
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the name of the table the column belongs to ( Need to set only if the column belongs to a different
        ///     table in join query)
        /// </summary>
        public string TableName
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the Value
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the Value
        /// </summary>
        public List<string> Values
        {
            get;
            set;
        }
    }
}