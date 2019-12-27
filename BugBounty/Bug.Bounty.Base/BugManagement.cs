namespace Bug.Bounty.Base
{
    using Bug.Bounty.Data;
    using Bug.Bounty.DataClasses;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Web.Configuration;

    public class BugManagement
    {
        SqlQueryBuilder queryBuilder = new SqlQueryBuilder();

        public bool AddBug(Bug bug)
        {
            var bugResult = new Result();
            bug.Id = Guid.NewGuid();
            var values = new Dictionary<string, object>
                    {
                        { "Id", bug.Id },
                        { "Description", bug.Description },
                        { "Title", bug.Title },
                        { "PlatformId", (int) bug.Platform },
                        { "CreatedUserId", bug.CreatedUserID },
                        { "IsActive", true }
                    };

            bugResult = DataProvider.ExecuteNonQuery(queryBuilder.AddToTable("Bug", values), Connection.ConnectionString);
            return bugResult.Status;
        }

        public User AddUser(User user)
        {
            try
            {
                if (!IsExistingEmail(user.Email))
                {
                    // Create new user Id
                    user.Id = Guid.NewGuid();
                    user.DisplayName = new MailAddress(user.Email).User;
                    var values = new Dictionary<string, object>
                    {
                        { "Id", user.Id },
                        { "DisplayName", user.DisplayName },
                        { "Email", user.Email },
                        { "PlatformId", (int) user.Platform },
                        { "UserRole", (int) user.Role },
                        { "IsActive", true },
                        { "IsDeleted", false }
                    };

                    var userResult = new Result();

                    userResult = DataProvider.ExecuteNonQuery(queryBuilder.AddToTable("User", values), Connection.ConnectionString);

                    if (!userResult.Status)
                    {
                        //exception
                    }

                    return user;
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return user;
        }

        public bool UpdateBug(Bug bug)
        {
            var updateColumn = new List<UpdateColumn>
            {
                new UpdateColumn
                {
                    TableName = "Bug",
                    ColumnName = "Description",
                    Value = bug.Description
                },
                new UpdateColumn
                {
                    TableName = "Bug",
                    ColumnName = "Title",
                    Value = bug.Title
                },
                new UpdateColumn
                {
                    TableName = "Bug",
                    ColumnName = "PlatformId",
                    Value = bug.Platform
                }
            };

            var whereColumns = new List<ConditionColumn>
                {
                    new ConditionColumn
                    {
                        ColumnName = "Id",
                        Condition = Condition.Equals,
                        Value = bug.Id
                    }
                };

            var bugResult = DataProvider.ExecuteNonQuery(queryBuilder.UpdateRowInTable("Bug", updateColumn, whereColumns), Connection.ConnectionString);
            return bugResult.Status;
        }

        public Bug GetBug(Guid bugId)
        {
            var bug = new Bug();
            try
            {
                var whereColumns = new List<ConditionColumn>
                {
                    new ConditionColumn
                    {
                        TableName = "Bug",
                        ColumnName = "Id",
                        Condition = Condition.Equals,
                        Value = bugId
                    }
                };

                var selected = new List<SelectedColumn>
                {
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "Id"
                    },
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "Title"
                    },
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "Description"
                    },
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "CreatedUserId"
                    },
                    new SelectedColumn
                    {
                        TableName = "CreatedUserTable",
                        ColumnName = "DisplayName",
                        AliasName = "CreatedUser"
                    },
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "ValidateUserId"
                    },
                    new SelectedColumn
                    {
                        TableName = "ValidateUserTable",
                        ColumnName = "DisplayName",
                        AliasName = "ValidatedUser"
                    },
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "Severity"
                    },
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "PlatformId"
                    }
                };

                var joinSpecification = new List<JoinSpecification>
                {
                    new JoinSpecification
                    {
                        Table = "User",
                        Column = new List<JoinColumn>
                        {
                            new JoinColumn
                            {
                                TableName = "User",
                                JoinedColumn = "Id",
                                Operation = Condition.Equals,
                                ParentTableColumn = "CreatedUserId",
                                ParentTable = "Bug"
                            }
                        },
                        JoinType = JoinType.Left,
                        JoinTableAliasName = "CreatedUserTable"
                    },
                    new JoinSpecification
                    {
                        Table = "User",
                        Column = new List<JoinColumn>
                        {
                            new JoinColumn
                            {
                                TableName = "User",
                                JoinedColumn = "Id",
                                Operation = Condition.Equals,
                                ParentTableColumn = "ValidateUserId",
                                ParentTable = "Bug"
                            }
                        },
                        JoinType = JoinType.Left,
                        JoinTableAliasName = "ValidateUserTable"
                    }
                };

                var result = DataProvider.ExecuteReaderQuery(queryBuilder.ApplyWhereClause(queryBuilder.ApplyMultipleJoins("Bug", selected, joinSpecification), whereColumns));
                bug = result.DataTable.AsEnumerable()
                  .Select(row => new Bug
                  {
                      Id = Guid.Parse(row.Field<object>("Id").ToString()),
                      Title = row.Field<string>("Title"),
                      Description = row.Field<string>("Description"),
                      CreatedUser = row.Field<string>("CreatedUser"),
                      ValidatedUser = row.Field<string>("ValidatedUser"),
                      CreatedUserID = Guid.Parse(row.Field<object>("CreatedUserID").ToString()),
                      ValidatedUserID = row.Field<object>("ValidateUserId") != null ? Guid.Parse(row.Field<object>("ValidateUserId").ToString()) : Guid.Empty,
                      Severity = row.Field<object>("Severity") != null ? int.Parse(row.Field<object>("Severity").ToString()) : 0,
                      Platform = (Platform)Enum.Parse(typeof(Platform), row.Field<object>("PlatformId").ToString())
                  }).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }

            return bug;
        }

        public List<Bug> GetBugs(Platform platform, Guid? userId = null)
        {
            var bugsList = new List<Bug>();
            try
            {
                var whereColumns = new List<ConditionColumn>
                {
                    new ConditionColumn
                    {
                        TableName = "Bug",
                        ColumnName = "PlatformId",
                        Condition = Condition.Equals,
                        Value = (int) platform
                    }
                };

                if(userId != null)
                {
                    whereColumns.Add(new ConditionColumn
                    {
                        TableName = "Bug",
                        ColumnName = "CreatedUserId",
                        Condition = Condition.Equals,
                        Value = userId,
                        LogicalOperator = LogicalOperator.OR
                    });
                }

                var selected = new List<SelectedColumn>
                {
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "Id"
                    },
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "Title"
                    },
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "Description"
                    },
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "CreatedUserId"
                    },
                    new SelectedColumn
                    {
                        TableName = "CreatedUserTable",
                        ColumnName = "DisplayName",
                        AliasName = "CreatedUser"
                    },
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "ValidateUserId"
                    },
                    new SelectedColumn
                    {
                        TableName = "ValidateUserTable",
                        ColumnName = "DisplayName",
                        AliasName = "ValidatedUser"
                    },
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "Severity"
                    },
                    new SelectedColumn
                    {
                        TableName = "Bug",
                        ColumnName = "PlatformId"
                    }
                };

                var joinSpecification = new List<JoinSpecification>
                {
                    new JoinSpecification
                    {
                        Table = "User",
                        Column = new List<JoinColumn>
                        {
                            new JoinColumn
                            {
                                TableName = "User",
                                JoinedColumn = "Id",
                                Operation = Condition.Equals,
                                ParentTableColumn = "CreatedUserId",
                                ParentTable = "Bug"
                            }
                        },
                        JoinType = JoinType.Left,
                        JoinTableAliasName = "CreatedUserTable"
                    },
                    new JoinSpecification
                    {
                        Table = "User",
                        Column = new List<JoinColumn>
                        {
                            new JoinColumn
                            {
                                TableName = "User",
                                JoinedColumn = "Id",
                                Operation = Condition.Equals,
                                ParentTableColumn = "ValidateUserId",
                                ParentTable = "Bug"
                            }
                        },
                        JoinType = JoinType.Left,
                        JoinTableAliasName = "ValidateUserTable"
                    }
                };

                var result = DataProvider.ExecuteReaderQuery(queryBuilder.ApplyWhereClause(queryBuilder.ApplyMultipleJoins("Bug", selected, joinSpecification), whereColumns));
                bugsList = result.DataTable.AsEnumerable()
                  .Select(row => new Bug
                  {
                      Id = Guid.Parse(row.Field<object>("Id").ToString()),
                      Title = row.Field<string>("Title"),
                      Description = row.Field<string>("Description"),
                      CreatedUser = row.Field<string>("CreatedUser"),
                      ValidatedUser = row.Field<string>("ValidatedUser"),
                      CreatedUserID = Guid.Parse(row.Field<object>("CreatedUserID").ToString()),
                      ValidatedUserID = row.Field<object>("ValidateUserId") != null ? Guid.Parse(row.Field<object>("ValidateUserId").ToString()) : Guid.Empty,
                      Severity = row.Field<object>("Severity") != null ? int.Parse(row.Field<object>("Severity").ToString()) : 0,
                      Platform = (Platform)Enum.Parse(typeof(Platform), row.Field<object>("PlatformId").ToString())
                  }).ToList();
            }
            catch (Exception ex)
            {
            }

            return bugsList;
        }

        public User GetUserDetails(Guid userId)
        {
            var user = new User();
            try
            {
                var whereColumns = new List<ConditionColumn>
                {
                    new ConditionColumn
                    {
                        ColumnName = "Id",
                        Condition = Condition.Equals,
                        Value = userId
                    }
                };

                var result = DataProvider.ExecuteReaderQuery(queryBuilder.SelectTopRecordsFromTable("User", new List<SelectedColumn> { new SelectedColumn { TableName = "User", ColumnName = "*" } }, 1, whereColumns, null));

                user = result.DataTable.AsEnumerable()
                    .Select(row => new User
                    {
                        Id = Guid.Parse(row.Field<object>("Id").ToString()),
                        Email = row.Field<string>("Email"),
                        DisplayName = row.Field<string>("DisplayName"),
                        Platform = (Platform)Enum.Parse(typeof(Platform), row.Field<object>("PlatformId").ToString()),
                        Role = (UserRole)Enum.Parse(typeof(UserRole), row.Field<object>("UserRole").ToString()),
                    }).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }

            return user;
        }

        public bool IsExistingEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return false;
                }

                var result = DataProvider.ExecuteReaderQuery(queryBuilder.IsExistingEmailQuery(email));

                result.Status = result.DataTable.Rows.Count > 0;

                return result.Status;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public User GetUser(string email)
        {
            var data = new User();
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return null;
                }

                var result = DataProvider.ExecuteReaderQuery(queryBuilder.IsExistingEmailQuery(email));

                data = result.DataTable.AsEnumerable().Select(
                        row => new User
                        {
                            Id = Guid.Parse(row.Field<object>("Id").ToString()),
                            DisplayName = row.Field<string>("DisplayName"),
                            Email = row.Field<string>("Email"),
                            IsActive = bool.Parse(row.Field<object>("IsActive").ToString()),
                        }).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }

            return data;
        }

        public bool StartUp()
        {
            var sqlcedbScript = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + WebConfigurationManager.AppSettings["SystemConfigurationPath"] + "sql_tables.sql");
            var appDataFolderPath = AppDomain.CurrentDomain.BaseDirectory + WebConfigurationManager.AppSettings["AppDataPath"];

            if (Directory.Exists(appDataFolderPath) == false)
            {
                Directory.CreateDirectory(appDataFolderPath);
            }
            else
            {
                Array.ForEach(Directory.GetFiles(appDataFolderPath), System.IO.File.Delete);
            }

            var connStr = Connection.ConnectionString;
            using (var engine = new SqlCeEngine(connStr))
            {
                engine.CreateDatabase();
            }

            var script = sqlcedbScript.OpenText().ReadToEnd().TrimEnd();
            SqlCeConnection conn = null;
            try
            {
                conn = new SqlCeConnection(connStr);
                conn.Open();
                var cmd = conn.CreateCommand();
                var splitter = new[] { ";" };
                var commandTexts = script.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                foreach (string commandText in commandTexts)
                {
                    cmd.CommandText = commandText;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return true;
        }
    }
}