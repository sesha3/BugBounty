namespace BugBounty
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;    

    public class SqlQueryBuilder
    {
        private Regex regexName = new Regex("^[a-zA-Z_]*$");

        public string IsExistingEmailQuery(string email) => "Select * from [User] where Email = '" + email + "'And IsActive= 'true'";

        public static string ApplyHavingClause(string query, List<ConditionColumn> havingClauseColumns)
        {
            var queryString = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(query))
            {
                queryString.Append(query);
                if (havingClauseColumns != null && havingClauseColumns.Count > 0)
                {
                    queryString.Append(" HAVING ");
                    for (var i = 0; i < havingClauseColumns.Count; i++)
                    {
                        if (havingClauseColumns[i].LogicalOperator != LogicalOperator.None && i != 0)
                        {
                            queryString.Append(" " + havingClauseColumns[i].LogicalOperator);
                        }

                        if (havingClauseColumns[i].Aggregation != AggregateMethod.None)
                        {
                            queryString.Append(" " + havingClauseColumns[i].Aggregation);
                            queryString.Append("(" +
                                               ((!string.IsNullOrWhiteSpace(havingClauseColumns[i].TableName))
                                                   ? " [" + havingClauseColumns[i].TableName + "]."
                                                   : string.Empty) + "[" + havingClauseColumns[i].ColumnName + "])");
                        }
                        else
                        {
                            queryString.Append(((!string.IsNullOrWhiteSpace(havingClauseColumns[i].TableName))
                                ? " [" + havingClauseColumns[i].TableName + "]."
                                : " ") + "[" + havingClauseColumns[i].ColumnName + "]");
                        }

                        queryString.Append(GetConditionOperator(havingClauseColumns[i].Condition));

                        queryString.Append(QueryHelper.GetData(havingClauseColumns[i].Value));
                    }
                }
            }

            return queryString.ToString();
        }

        public string AddToTable(string tableName, Dictionary<string, object> values)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName", "Table Name is null");
            }
            else
            {
                tableName = tableName.Trim();

                if (!this.regexName.IsMatch(tableName))
                {
                    throw new ArgumentException("Table name should not contain special characters");
                }
            }

            if (tableName.Contains(" "))
            {
                throw new ArgumentException("Table Name has whitespace");
            }

            var queryString = new StringBuilder();
            queryString.Append("INSERT INTO ");
            queryString.Append("[" + tableName.Replace("'", string.Empty) + "] (");
            var columnValues = new StringBuilder();
            var counter = 0;

            if (values == null || values.Count <= 0)
            {
                throw new ArgumentNullException("values", "The Values should not be null");
            }

            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(value.Key))
                {
                    throw new ArgumentNullException("value", "The key field should not be null");
                }
                else
                {
                    if (value.Key.Trim().Contains(" "))
                    {
                        throw new ArgumentException("Column Name has whitespace");
                    }

                    if (!this.regexName.IsMatch(value.Key))
                    {
                        throw new ArgumentException("Column name should not contain special characters");
                    }
                }

                queryString.Append("[" + value.Key.Trim() + "]");
                columnValues.Append(QueryHelper.GetData(value.Value));

                if (counter != values.Keys.Count - 1)
                {
                    queryString.Append(",");
                    columnValues.Append(",");
                }

                counter++;
            }

            queryString.Append(") VALUES (");
            queryString.Append(columnValues);
            queryString.Append(")");
            return queryString.ToString();
        }

        public string AddToTable(string tableName, Dictionary<string, object> values, List<string> outputColumns)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName", "Table Name is null");
            }
            else
            {
                tableName = tableName.Trim();

                if (!this.regexName.IsMatch(tableName))
                {
                    throw new ArgumentException("Table name should not contain special characters");
                }
            }

            if (tableName.Contains(" "))
            {
                throw new ArgumentException("Table Name has whitespace");
            }

            var queryString = new StringBuilder();
            queryString.Append("INSERT INTO ");
            queryString.Append("[" + tableName.Replace("'", string.Empty) + "] (");
            var columnValues = new StringBuilder();
            int counter = 0;

            if (values == null || values.Count <= 0)
            {
                throw new ArgumentNullException("values", "The Values should not be null");
            }

            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(value.Key))
                {
                    throw new ArgumentNullException("value", "The key field should not be null");
                }
                else
                {
                    if (value.Key.Trim().Contains(" "))
                    {
                        throw new ArgumentException("Column Name has whitespace");
                    }

                    if (!this.regexName.IsMatch(value.Key))
                    {
                        throw new ArgumentException("Column name should not contain special characters");
                    }
                }

                queryString.Append("[" + value.Key + "]");
                columnValues.Append(QueryHelper.GetData(value.Value));
                if (counter != values.Keys.Count - 1)
                {
                    queryString.Append(",");
                    columnValues.Append(",");
                }

                counter++;
            }

            queryString.Append(")");
            if (outputColumns != null)
            {
                queryString.Append(" OUTPUT ");
                for (int i = 0; i < outputColumns.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(outputColumns[i]))
                    {
                        throw new ArgumentNullException("Column Name is null");
                    }
                    else
                    {
                        outputColumns[i] = outputColumns[i].Trim();

                        if (!this.regexName.IsMatch(outputColumns[i]))
                        {
                            throw new ArgumentException("Column name should not contain special characters");
                        }
                    }

                    if (outputColumns[i].Contains(" "))
                    {
                        throw new ArgumentException("Column Name has whitespace");
                    }

                    queryString.Append(" Inserted." + outputColumns[i]);

                    if (i != outputColumns.Count - 1)
                    {
                        queryString.Append(",");
                    }
                }
            }

            queryString.Append(" VALUES (");
            queryString.Append(columnValues);
            queryString.Append(")");
            return queryString.ToString();
        }

        public string AddToTableAsBulkForLogs(
            string tableName,
            Dictionary<string, object> values,
            string bulkUpdateColumName,
            List<int> bulkValues)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName", "Table Name is null");
            }
            else
            {
                tableName = tableName.Trim();

                if (!this.regexName.IsMatch(tableName))
                {
                    throw new ArgumentException("Table name should not contain special characters");
                }
            }

            if (tableName.Contains(" "))
            {
                throw new ArgumentException("Table Name has whitespace");
            }

            var queryString = new StringBuilder();
            var tempDictionary = new Dictionary<string, object>(values);
            tempDictionary.Add(bulkUpdateColumName, DBNull.Value);
            queryString.Append(this.AddToTable(tableName, tempDictionary));
            queryString.Append(";");

            for (var queryCount = 0; queryCount < bulkValues.Count; queryCount++)
            {
                tempDictionary = new Dictionary<string, object>(values);
                tempDictionary.Add(bulkUpdateColumName, bulkValues[queryCount]);
                queryString.Append(this.AddToTable(tableName, tempDictionary));
                queryString.Append(";");
            }

            return queryString.ToString();
        }

        public string AddToTableWithGUID(string tableName, Dictionary<string, object> values, string guidColumn, Guid guid)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName", "Table Name is null");
            }
            else
            {
                tableName = tableName.Trim();

                if (!this.regexName.IsMatch(tableName))
                {
                    throw new ArgumentException("Table name should not contain special characters");
                }
            }

            if (tableName.Contains(" "))
            {
                throw new ArgumentException("Table Name has whitespace");
            }

            var queryString = new StringBuilder();
            queryString.Append("INSERT INTO ");
            queryString.Append("[" + tableName.Replace("'", string.Empty) + "] (");
            var columnValues = new StringBuilder();
            var counter = 0;
            if (values == null || values.Count <= 0)
            {
                throw new ArgumentNullException("values", "The Values should not be null");
            }

            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(value.Key))
                {
                    throw new ArgumentNullException("value", "The key field should not be null");
                }
                else
                {
                    if (value.Key.Trim().Contains(" "))
                    {
                        throw new ArgumentException("Column Name has whitespace");
                    }

                    if (!this.regexName.IsMatch(value.Key))
                    {
                        throw new ArgumentException("Column name should not contain special characters");
                    }
                }

                queryString.Append("[" + value.Key + "]");

                columnValues.Append(QueryHelper.GetData(value.Value));

                if (counter != values.Keys.Count - 1)
                {
                    queryString.Append(",");
                    columnValues.Append(",");
                }

                counter++;
            }

            if (string.IsNullOrWhiteSpace(guidColumn))
            {
                throw new ArgumentNullException("Guid Column Name is null");
            }
            else
            {
                guidColumn = guidColumn.Trim();

                if (!this.regexName.IsMatch(guidColumn))
                {
                    throw new ArgumentException("Guid Column name should not contain special characters");
                }
            }

            if (guidColumn.Contains(" "))
            {
                throw new ArgumentException("Guid Column Name has whitespace");
            }

            if (guid == Guid.Empty)
            {
                throw new ArgumentException("Guid value should not be empty");
            }

            queryString.Append(",[" + guidColumn + "]) VALUES (" + columnValues + ",'" + guid.ToString() + "')");
            return queryString.ToString();
        }

        public string ApplyGroupBy(string query, List<GroupByColumn> columns)
        {
            var queryString = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(query))
            {
                queryString.Append(query);
                if (columns != null && columns.Count > 0)
                {
                    queryString.Append("  GROUP BY  ");
                    for (var i = 0; i < columns.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(columns[i].TableName))
                        {
                            columns[i].TableName = columns[i].TableName.Trim();

                            if (!this.regexName.IsMatch(columns[i].TableName))
                            {
                                throw new ArgumentException("Table name should not contain special characters");
                            }

                            if (columns[i].TableName.Contains(" "))
                            {
                                throw new ArgumentException("Table Name has whitespace");
                            }
                        }

                        if (string.IsNullOrWhiteSpace(columns[i].ColumnName))
                        {
                            throw new ArgumentNullException("columns", "The column name should not be null");
                        }
                        else
                        {
                            columns[i].ColumnName = columns[i].ColumnName.Trim();
                            if (columns[i].ColumnName.Contains(" "))
                            {
                                throw new ArgumentException("Column Name has whitespace");
                            }

                            if (!this.regexName.IsMatch(columns[i].ColumnName))
                            {
                                throw new ArgumentException("Column name should not contain special characters");
                            }
                        }

                        queryString.Append(((!string.IsNullOrWhiteSpace(columns[i].TableName))
                                            ? "[" + columns[i].TableName + "]."
                                            : " ") + "[" + columns[i].ColumnName + "]");
                        if (i != columns.Count - 1)
                        {
                            queryString.Append(",");
                        }
                    }
                }
            }

            return queryString.ToString();
        }

        public string ApplyJoins(string query, JoinSpecification joinSpecification)
        {
            var queryString = new StringBuilder();
            if (joinSpecification.Table == null && joinSpecification.Column == null)
            {
                return query;
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                queryString.Append(query);
                queryString.Append(" " + joinSpecification.JoinType + " JOIN ");

                if (string.IsNullOrWhiteSpace(joinSpecification.Table))
                {
                    throw new ArgumentNullException("joinSpecification", "Table Name is null");
                }
                else
                {
                    joinSpecification.Table = joinSpecification.Table.Trim();

                    if (!this.regexName.IsMatch(joinSpecification.Table))
                    {
                        throw new ArgumentException("Table name should not contain special characters");
                    }
                }

                if (joinSpecification.Table.Contains(" "))
                {
                    throw new ArgumentException("Table Name has whitespace");
                }

                queryString.Append("[" + joinSpecification.Table + "]");
                if (!string.IsNullOrWhiteSpace(joinSpecification.JoinTableAliasName))
                {
                    queryString.Append(" AS " + joinSpecification.JoinTableAliasName);
                }

                queryString.Append(" ON ");
                queryString.Append("(");
                for (var j = 0; j < joinSpecification.Column.Count; j++)
                {
                    var parentTable =
                        query.Substring((query.LastIndexOf("FROM") != -1)
                                        ? query.LastIndexOf("FROM") + 4
                                        : query.LastIndexOf("from") + 4);

                    if (joinSpecification.Column[j].LogicalOperator != LogicalOperator.None && j != 0)
                    {
                        queryString.Append(" " + joinSpecification.Column[j].LogicalOperator);
                    }

                    if (string.IsNullOrWhiteSpace(joinSpecification.Column[j].ParentTableColumn))
                    {
                        throw new ArgumentNullException("joinSpecification", "Table Name is null");
                    }
                    else
                    {
                        joinSpecification.Column[j].ParentTableColumn =
                            joinSpecification.Column[j].ParentTableColumn.Trim();

                        if (!this.regexName.IsMatch(joinSpecification.Column[j].ParentTableColumn))
                        {
                            throw new ArgumentException("Table name should not contain special characters");
                        }
                    }

                    if (joinSpecification.Column[j].ParentTableColumn.Contains(" "))
                    {
                        throw new ArgumentException("Table Name has whitespace");
                    }

                    queryString.Append(parentTable + ".[" + joinSpecification.Column[j].ParentTableColumn + "]" +
                                       GetConditionOperator(joinSpecification.Column[j].Operation));

                    if (!string.IsNullOrWhiteSpace(joinSpecification.JoinTableAliasName))
                    {
                        joinSpecification.JoinTableAliasName = joinSpecification.JoinTableAliasName.Trim();

                        if (!this.regexName.IsMatch(joinSpecification.JoinTableAliasName))
                        {
                            throw new ArgumentException("Alias name should not contain special characters");
                        }

                        if (joinSpecification.JoinTableAliasName.Contains(" "))
                        {
                            throw new ArgumentException("Alias Name has whitespace");
                        }
                    }

                    queryString.Append(joinSpecification.Column[j].ConditionValue != null
                                       ? joinSpecification.Column[j].ConditionValue
                                       : (!string.IsNullOrWhiteSpace(joinSpecification.JoinTableAliasName))
                                       ? joinSpecification.JoinTableAliasName
                                       : "[" + joinSpecification.Column[j].TableName + "]");

                    queryString.Append((joinSpecification.Column[j].ConditionValue == null) ? ".[" + joinSpecification.Column[j].JoinedColumn + "]" : string.Empty);
                }

                queryString.Append(")");
            }

            return queryString.ToString();
        }

        public string ApplyJoins(string tableName, List<SelectedColumn> columnsToSelect, JoinSpecification joinSpecification)
        {
            var query = this.SelectRecordsFromTable(tableName, columnsToSelect);
            return this.ApplyJoins(query, joinSpecification);
        }

        public string ApplyJoins(
            string tableName,
            List<SelectedColumn> columnsToSelect,
            JoinSpecification joinSpecification,
            List<ConditionColumn> whereClauseColumns)
        {
            var query = this.SelectRecordsFromTable(tableName, columnsToSelect);
            return this.ApplyWhereClause(this.ApplyJoins(query, joinSpecification), whereClauseColumns);
        }

        public string ApplyMultipleJoins(string query, List<JoinSpecification> joinSpecification)
        {
            var queryString = new StringBuilder();
            queryString.Append(query);
            for (var t = 0; t < joinSpecification.Count(); t++)
            {
                if (joinSpecification[t].Table == null && joinSpecification[t].Column == null)
                {
                    return query;
                }

                if (!string.IsNullOrWhiteSpace(query))
                {
                    queryString.Append(" " + joinSpecification[t].JoinType + " JOIN ");

                    if (string.IsNullOrWhiteSpace(joinSpecification[t].Table))
                    {
                        throw new ArgumentNullException("joinSpecification", "Table Name is null");
                    }
                    else
                    {
                        joinSpecification[t].Table = joinSpecification[t].Table.Trim();

                        if (!this.regexName.IsMatch(joinSpecification[t].Table))
                        {
                            throw new ArgumentException("Table name should not contain special characters");
                        }
                    }

                    if (joinSpecification[t].Table.Contains(" "))
                    {
                        throw new ArgumentException("Table Name has whitespace");
                    }

                    queryString.Append("[" + joinSpecification[t].Table + "]");

                    if (!string.IsNullOrWhiteSpace(joinSpecification[t].JoinTableAliasName))
                    {
                        queryString.Append(" AS " + joinSpecification[t].JoinTableAliasName);
                    }

                    queryString.Append(" ON ");
                    queryString.Append("(");

                    for (var j = 0; j < joinSpecification[t].Column.Count; j++)
                    {
                        if (joinSpecification[t].Column[j].LogicalOperator != LogicalOperator.None && j != 0)
                        {
                            queryString.Append(" " + joinSpecification[t].Column[j].LogicalOperator);
                        }

                        if (string.IsNullOrWhiteSpace(joinSpecification[t].Column[j].ParentTable))
                        {
                            throw new ArgumentNullException("joinSpecification", "Table Name is null");
                        }
                        else
                        {
                            joinSpecification[t].Column[j].ParentTable = joinSpecification[t].Column[j].ParentTable.Trim();

                            if (!this.regexName.IsMatch(joinSpecification[t].Column[j].ParentTable))
                            {
                                throw new ArgumentException("Table name should not contain special characters");
                            }
                        }

                        if (joinSpecification[t].Column[j].ParentTable.Contains(" "))
                        {
                            throw new ArgumentException("Table Name has whitespace");
                        }

                        if (string.IsNullOrWhiteSpace(joinSpecification[t].Column[j].ParentTableColumn))
                        {
                            throw new ArgumentNullException("joinSpecification", "Table Name is null");
                        }
                        else
                        {
                            joinSpecification[t].Column[j].ParentTableColumn =
                                joinSpecification[t].Column[j].ParentTableColumn.Trim();

                            if (!this.regexName.IsMatch(joinSpecification[t].Column[j].ParentTableColumn))
                            {
                                throw new ArgumentException("Table name should not contain special characters");
                            }
                        }

                        if (joinSpecification[t].Column[j].ParentTableColumn.Contains(" "))
                        {
                            throw new ArgumentException("Table Name has whitespace");
                        }

                        queryString.Append(" [" + joinSpecification[t].Column[j].ParentTable + "] " + ".[" +
                                           joinSpecification[t].Column[j].ParentTableColumn + "]" +
                                           GetConditionOperator(joinSpecification[t].Column[j].Operation));

                        if (!string.IsNullOrWhiteSpace(joinSpecification[t].JoinTableAliasName))
                        {
                            joinSpecification[t].JoinTableAliasName = joinSpecification[t].JoinTableAliasName.Trim();

                            if (!this.regexName.IsMatch(joinSpecification[t].JoinTableAliasName))
                            {
                                throw new ArgumentException("Alias name should not contain special characters");
                            }

                            if (joinSpecification[t].JoinTableAliasName.Contains(" "))
                            {
                                throw new ArgumentException("Alias Name has whitespace");
                            }
                        }

                        queryString.Append((joinSpecification[t].Column[j].ConditionValue != null)
                                           ? IsNumber(joinSpecification[t].Column[j].ConditionValue) ? joinSpecification[t].Column[j].ConditionValue : "'" + joinSpecification[t].Column[j].ConditionValue + "'"
                                           : (!string.IsNullOrWhiteSpace(joinSpecification[t].JoinTableAliasName))
                                           ? joinSpecification[t].JoinTableAliasName
                                           : "[" + joinSpecification[t].Column[j].TableName + "]");

                        queryString.Append((joinSpecification[t].Column[j].ConditionValue == null) ? ".[" + joinSpecification[t].Column[j].JoinedColumn + "]" : string.Empty);
                    }

                    queryString.Append(")");
                }
            }

            return queryString.ToString();
        }

        public string ApplyMultipleJoins(string tableName, List<SelectedColumn> columnsToSelect, List<JoinSpecification> joinSpecification)
        {
            var query = this.SelectRecordsFromTable(tableName, columnsToSelect);
            return this.ApplyMultipleJoins(query, joinSpecification);
        }

        public string ApplyOrderBy(string query, List<OrderByColumns> orderByColumns)
        {
            var queryString = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(query))
            {
                queryString.Append(query);
                if (orderByColumns != null && orderByColumns.Count > 0)
                {
                    queryString.Append(" ORDER BY ");
                    for (var i = 0; i < orderByColumns.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(orderByColumns[i].TableName))
                        {
                            orderByColumns[i].TableName = orderByColumns[i].TableName.Trim();

                            if (!this.regexName.IsMatch(orderByColumns[i].TableName))
                            {
                                throw new ArgumentException("Table name should not contain special characters");
                            }

                            if (orderByColumns[i].TableName.Contains(" "))
                            {
                                throw new ArgumentException("Table Name has whitespace");
                            }
                        }

                        if (string.IsNullOrWhiteSpace(orderByColumns[i].ColumnName))
                        {
                            throw new ArgumentNullException("orderByColumns", "The column name should not be null");
                        }
                        else
                        {
                            orderByColumns[i].ColumnName = orderByColumns[i].ColumnName.Trim();
                            if (orderByColumns[i].ColumnName.Contains(" "))
                            {
                                throw new ArgumentException("Column Name has whitespace");
                            }

                            if (!this.regexName.IsMatch(orderByColumns[i].ColumnName))
                            {
                                throw new ArgumentException("Column name should not contain special characters");
                            }
                        }

                        queryString.Append(((!string.IsNullOrWhiteSpace(orderByColumns[i].TableName))
                                            ? "[" + orderByColumns[i].TableName + "]."
                                            : " ") + "[" + orderByColumns[i].ColumnName + "]");

                        if (orderByColumns[i].OrderBy != OrderByType.None)
                        {
                            queryString.Append(" " + orderByColumns[i].OrderBy);
                        }

                        if (i != orderByColumns.Count - 1)
                        {
                            queryString.Append(",");
                        }
                    }
                }
            }

            return queryString.ToString();
        }

        public string ApplyWhereClause(string query, List<ConditionColumn> whereConditionColumns)
        {
            var queryString = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(query))
            {
                queryString.Append(query);
                if (whereConditionColumns != null && whereConditionColumns.Count > 0)
                {
                    queryString.Append("  WHERE  ");
                    for (var i = 0; i < whereConditionColumns.Count; i++)
                    {
                        if (whereConditionColumns[i].Condition == Condition.IN || whereConditionColumns[i].Condition == Condition.NOTIN)
                        {
                            if (whereConditionColumns[i].LogicalOperator != LogicalOperator.None && i != 0)
                            {
                                queryString.Append(" " + whereConditionColumns[i].LogicalOperator);
                            }

                            queryString.Append(((!string.IsNullOrWhiteSpace(whereConditionColumns[i].TableName))
                                                ? " [" + whereConditionColumns[i].TableName + "]."
                                                : " ") + "[" + whereConditionColumns[i].ColumnName + "]");
                            queryString.Append(" " + GetConditionOperator(whereConditionColumns[i].Condition));
                            queryString.Append("(");
                            if (whereConditionColumns[i].Values != null)
                            {
                                for (var j = 0; j < whereConditionColumns[i].Values.Count; j++)
                                {
                                    if (j != 0)
                                    {
                                        queryString.Append(",");
                                    }

                                    queryString.Append(QueryHelper.GetData(whereConditionColumns[i].Values[j]));
                                }
                            }

                            queryString.Append(")");
                        }
                        else
                        {
                            if (whereConditionColumns[i].LogicalOperator != LogicalOperator.None && i != 0)
                            {
                                queryString.Append(" " + whereConditionColumns[i].LogicalOperator);
                            }

                            if (!string.IsNullOrWhiteSpace(whereConditionColumns[i].TableName))
                            {
                                whereConditionColumns[i].TableName = whereConditionColumns[i].TableName.Trim();
                                if (!this.regexName.IsMatch(whereConditionColumns[i].TableName))
                                {
                                    throw new ArgumentException("Table name should not contain special characters");
                                }

                                if (whereConditionColumns[i].TableName.Contains(" "))
                                {
                                    throw new ArgumentException("Table Name has whitespace");
                                }
                            }

                            queryString.Append(((!string.IsNullOrWhiteSpace(whereConditionColumns[i].TableName))
                                                ? " [" + whereConditionColumns[i].TableName + "]."
                                                : " ") + "[" + whereConditionColumns[i].ColumnName + "]");
                            queryString.Append(GetConditionOperator(whereConditionColumns[i].Condition));
                            if (whereConditionColumns[i].Condition == Condition.LIKE)
                            {
                                queryString.Append(whereConditionColumns[i].Value == DBNull.Value
                                                   ? "Null"
                                                   : (IsNumber(whereConditionColumns[i].Value)
                                                      ? whereConditionColumns[i].Value
                                                      : (whereConditionColumns[i].Value == null) ? "%%" : "'%" + whereConditionColumns[i].Value + "%'"));
                            }
                            else if (whereConditionColumns[i].Condition == Condition.IS)
                            {
                                queryString.Append(whereConditionColumns[i].Value == DBNull.Value ? "Null"
                                    : (IsNumber(whereConditionColumns[i].Value)
                                    ? whereConditionColumns[i].Value
                                    : (whereConditionColumns[i].Value == null) ? "NULL" : "'" + whereConditionColumns[i].Value + "'"));
                            }
                            else
                            {
                                queryString.Append(QueryHelper.GetData(whereConditionColumns[i].Value));
                            }
                        }
                    }
                }
            }

            return queryString.ToString();
        }

        public string DeleteAllRowsFromTable(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName", "Table Name is null");
            }
            else
            {
                tableName = tableName.Trim();
                if (!this.regexName.IsMatch(tableName))
                {
                    throw new ArgumentException("Table name should not contain special characters");
                }
            }

            if (tableName.Contains(" "))
            {
                throw new ArgumentException("Table Name has whitespace");
            }

            return "DELETE FROM [" + tableName + "]";
        }

        public string DeleteRowFromTable(string tableName, List<ConditionColumn> row)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName", "Table Name is null");
            }
            else
            {
                tableName = tableName.Trim();
                if (!this.regexName.IsMatch(tableName))
                {
                    throw new ArgumentException("Table name should not contain special characters");
                }
            }

            if (tableName.Contains(" "))
            {
                throw new ArgumentException("Table Name has whitespace");
            }

            var queryString = new StringBuilder();
            queryString.Append("DELETE FROM ");
            queryString.Append("[" + tableName.Replace("'", string.Empty) + "]");
            return this.ApplyWhereClause(queryString.ToString(), row);
        }

        public string SelectAllRecordsFromTable(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName", "Table Name is null");
            }
            else
            {
                tableName = tableName.Trim();

                if (!this.regexName.IsMatch(tableName))
                {
                    throw new ArgumentException("Table name should not contain special characters");
                }
            }

            if (tableName.Contains(" "))
            {
                throw new ArgumentException("Table Name has whitespace");
            }

            return "SELECT * FROM [" + tableName.Replace("'", string.Empty) + "]";
        }

        public string SelectAllRecordsFromTable(string tableName, List<ConditionColumn> whereConditionColumns)
        {
            return this.ApplyWhereClause(this.SelectAllRecordsFromTable(tableName), whereConditionColumns);
        }

        public string SelectAllRecordsFromTable(
            string tableName,
            List<ConditionColumn> whereConditionColumns,
            List<OrderByColumns> orderByColumns)
        {
            var query = this.SelectAllRecordsFromTable(tableName);

            if (whereConditionColumns != null && whereConditionColumns.Count > 0)
            {
                query = this.ApplyWhereClause(query, whereConditionColumns);
            }

            if (orderByColumns != null && orderByColumns.Count > 0)
            {
                query = this.ApplyOrderBy(query, orderByColumns);
            }

            return query;
        }

        public string SelectRecordsFromTable(string tableName, List<SelectedColumn> columns)
        {
            var queryString = new StringBuilder();

            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName", "Table Name is null");
            }
            else
            {
                tableName = tableName.Trim();
                if (!this.regexName.IsMatch(tableName))
                {
                    throw new ArgumentException("Table name should not contain special characters");
                }
            }

            if (tableName.Contains(" "))
            {
                throw new ArgumentException("Table Name has whitespace");
            }

            queryString.Append("SELECT ");
            for (var i = 0; i < columns.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(columns[i].TableName))
                {
                    columns[i].TableName = columns[i].TableName.Trim();
                    if (!this.regexName.IsMatch(columns[i].TableName))
                    {
                        throw new ArgumentException("Table name should not contain special characters");
                    }

                    if (columns[i].TableName.Contains(" "))
                    {
                        throw new ArgumentException("Table Name has whitespace");
                    }
                }

                if (columns[i].IsDistinct)
                {
                    queryString.Append(" DISTINCT ");
                }

                if (columns[i].Aggregation != AggregateMethod.None)
                {
                    queryString.Append(columns[i].Aggregation + "(" +
                                       ((!string.IsNullOrWhiteSpace(columns[i].TableName))
                                        ? "[" + columns[i].TableName + "]."
                                        : string.Empty) + columns[i].ColumnName + ")");
                }
                else
                {
                    queryString.Append((!string.IsNullOrWhiteSpace(columns[i].JoinAliasName)
                        ? columns[i].JoinAliasName + "."
                        : !string.IsNullOrWhiteSpace(columns[i].TableName)
                            ? "[" + columns[i].TableName + "]."
                            : string.Empty) + columns[i].ColumnName);
                }

                if (!string.IsNullOrWhiteSpace(columns[i].AliasName))
                {
                    queryString.Append(" AS [" + columns[i].AliasName + "]");
                }

                if (i != columns.Count - 1)
                {
                    queryString.Append(",");
                }
            }

            queryString.Append(" FROM ");
            queryString.Append("[" + tableName + "]");
            return queryString.ToString();
        }

        public string SelectRecordsFromTable(
            string tableName,
            List<SelectedColumn> columns,
            List<ConditionColumn> values)
        {
            return this.ApplyWhereClause(this.SelectRecordsFromTable(tableName, columns), values);
        }

        public string SelectTopRecordsFromTable(string tableName, List<SelectedColumn> columns, int numberOfRecords)
        {
            var queryString = new StringBuilder();

            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName", "Table Name is null");
            }
            else
            {
                tableName = tableName.Trim();
                if (!this.regexName.IsMatch(tableName))
                {
                    throw new ArgumentException("Table name should not contain special characters");
                }
            }

            if (tableName.Contains(" "))
            {
                throw new ArgumentException("Table Name has whitespace");
            }

            queryString.Append("SELECT TOP ");
            queryString.Append(numberOfRecords);
            queryString.Append(" ");
            for (var i = 0; i < columns.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(columns[i].TableName))
                {
                    columns[i].TableName = columns[i].TableName.Trim();
                    if (!this.regexName.IsMatch(columns[i].TableName))
                    {
                        throw new ArgumentException("Table name should not contain special characters");
                    }

                    if (columns[i].TableName.Contains(" "))
                    {
                        throw new ArgumentException("Table Name has whitespace");
                    }
                }

                if (columns[i].IsDistinct)
                {
                    queryString.Append(" DISTINCT ");
                }

                if (columns[i].Aggregation != AggregateMethod.None)
                {
                    queryString.Append(columns[i].Aggregation + "(" +
                                       ((!string.IsNullOrWhiteSpace(columns[i].TableName))
                                        ? "[" + columns[i].TableName + "]."
                                        : string.Empty) + columns[i].ColumnName + ")");
                }
                else
                {
                    queryString.Append(
                        (!string.IsNullOrWhiteSpace(columns[i].JoinAliasName)
                         ? columns[i].JoinAliasName + "."
                         : !string.IsNullOrWhiteSpace(columns[i].TableName)
                         ? "[" + columns[i].TableName + "]."
                         : string.Empty) + columns[i].ColumnName);
                }

                if (!string.IsNullOrWhiteSpace(columns[i].AliasName))
                {
                    queryString.Append(" AS [" + columns[i].AliasName + "]");
                }

                if (i != columns.Count - 1)
                {
                    queryString.Append(",");
                }
            }

            queryString.Append(" FROM ");
            queryString.Append("[" + tableName + "]");
            return queryString.ToString();
        }

        public string SelectTopRecordsFromTable(
            string tableName,
            List<SelectedColumn> columnsToSelect,
            int numberOfRecords,
            List<ConditionColumn> whereConditionColumns,
            List<OrderByColumns> orderByColumns)
        {
            var query = this.SelectTopRecordsFromTable(tableName, columnsToSelect, numberOfRecords);
            if (whereConditionColumns != null && whereConditionColumns.Count > 0)
            {
                query = this.ApplyWhereClause(query, whereConditionColumns);
            }

            if (orderByColumns != null && orderByColumns.Count > 0)
            {
                query = this.ApplyOrderBy(query, orderByColumns);
            }

            return query;
        }

        public string UpdateRowInTable(
            string tableName,
            List<UpdateColumn> updateColumns,
            List<ConditionColumn> whereConditionColumns)
        {
            var queryString = new StringBuilder();
            queryString.Append("UPDATE ");
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                tableName = tableName.Trim();

                if (!this.regexName.IsMatch(tableName))
                {
                    throw new ArgumentException("Table name should not contain special characters");
                }

                if (tableName.Contains(" "))
                {
                    throw new ArgumentException("Table Name has whitespace");
                }

                queryString.Append("[" + tableName + "]");
                queryString.Append(" SET ");

                for (var i = 0; i < updateColumns.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(updateColumns[i].ColumnName))
                    {
                        throw new ArgumentNullException("updateColumns", "The column name should not be null");
                    }
                    else
                    {
                        updateColumns[i].ColumnName = updateColumns[i].ColumnName.Trim();
                        if (updateColumns[i].ColumnName.Contains(" "))
                        {
                            throw new ArgumentException("Column Name has whitespace");
                        }

                        if (!this.regexName.IsMatch(updateColumns[i].ColumnName))
                        {
                            throw new ArgumentException("Column name should not contain special characters");
                        }
                    }

                    queryString.Append("[" + updateColumns[i].ColumnName + "]=");
                    queryString.Append(QueryHelper.GetData(updateColumns[i].Value));
                    if (i != updateColumns.Count - 1)
                    {
                        queryString.Append(",");
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("tableName", "The table name should not be null");
            }

            return this.ApplyWhereClause(queryString.ToString(), whereConditionColumns);
        }

        /// <summary>
        ///   Gives the count of records
        /// </summary>
        /// <param name="query">Input query</param>
        /// <returns>Query string</returns>
        public string GetRecordsCount(string query)
        {
            var queryString = new StringBuilder();
            queryString.Append("SELECT COUNT(*) AS TotalRecords FROM (" + query);
            if (query.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) != -1)
            {
                queryString.Append(" OFFSET 0 ROWS");
            }

            queryString.Append(") AS GETRECORD");

            return queryString.ToString();
        }

        public string GetDistinctRecordsCountFromTable(string column, string tableName)
        {
            var queryString = new StringBuilder();
            queryString.Append("SELECT COUNT(DISTINCT [" + column + "]) AS TotalRecords FROM [" + tableName + "]");
            return queryString.ToString();
        }

        /// <summary>
        ///    Appends pagination query
        /// </summary>
        /// <param name="query">Input query</param>
        /// <param name="orderByColumn">Column used to apply order by</param>
        /// <param name="skip">Number of records to be skipped</param>
        /// <param name="take">Number of records to be taken</param>
        /// <returns>Query string</returns>
        public string ApplyPagination(string query, OrderByColumns orderByColumn, int skip = Pagination.DefaultSkip, int take = Pagination.DefaultPageSize)
        {
            string orderByClause;
            var queryString = new StringBuilder();
            queryString.Append("SELECT * FROM (SELECT ROW_NUMBER() OVER(");
            Regex regex = new Regex(@"(order\s+by)\s+[A-Za-z0-9\[\]\.\\_]+\s+(desc|asc)?", RegexOptions.IgnoreCase);
            Match match = regex.Match(query);
            if (match.Success)
            {
                query = query.Replace(match.Value, string.Empty);
                orderByClause = match.Value;
                queryString.Append(" " + orderByClause);
            }
            else
            {
                queryString.Append(" ORDER BY " + orderByColumn.ColumnName + " " + orderByColumn.OrderBy);
            }

            queryString.Append(") AS ROW_NUM, * FROM (" + query + ") AS INPUT_QUERY) AS ROW_FN ");

            queryString.Append("WHERE ROW_NUM BETWEEN " + (skip + 1) + " AND " + (take + skip));

            return queryString.ToString();
        }

        public string GetDistinctRecordsCount(string query, string columName)
        {
            var queryString = new StringBuilder();
            queryString.Append("SELECT COUNT(DISTINCT " + columName + ") AS TotalRecords FROM (" + query);
            if (query.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) != -1)
            {
                queryString.Append(" OFFSET 0 ROWS");
            }

            queryString.Append(") AS GETRECORD");

            return queryString.ToString();
        }

        /// <summary>
        ///     Returns mathematical operator for the given condition
        /// </summary>
        /// <param name="condition">Conditions Enum</param>
        /// <returns>Mathematical operator as string</returns>
        private static string GetConditionOperator(Condition condition)
        {
            switch (condition)
            {
                case Condition.Equals:
                    return "=";

                case Condition.GreaterThan:
                    return ">";

                case Condition.GreaterThanOrEquals:
                    return ">=";

                case Condition.LessThan:
                    return "<";

                case Condition.LessThanOrEquals:
                    return "<=";

                case Condition.NotEquals:
                    return "!=";

                case Condition.IS:
                    return " IS ";

                case Condition.IN:
                    return " IN ";

                case Condition.LIKE:
                    return " LIKE ";

                case Condition.NOTIN:
                    return " NOT IN ";

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        ///     Checks if the value is a number type
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>True if the object is a number type</returns>
        private static bool IsNumber(object value)
        {
            if (value == null)
            {
                return false;
            }

            return value is double ||
                       value is int ||
                       value is short ||
                       value is long ||
                       value is decimal;
        }
    }
}