﻿namespace Bug.Bounty.DataClasses
{
    public class GroupByColumn
    {
        /// <summary>
        ///     Gets or sets the Column Name
        /// </summary>
        public string ColumnName
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the table name where the column belongs to ( Need to set only if the column belongs to a different
        ///     table in join query)
        /// </summary>
        public string TableName
        {
            get;
            set;
        }
    }
}