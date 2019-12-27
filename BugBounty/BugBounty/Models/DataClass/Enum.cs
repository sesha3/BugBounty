namespace BugBounty
{
    public enum Platform
    {
        BoldBI = 1,
        BoldReports,
        DataIntegrationPlatform,
        XamarinForms
    }

    public enum UserRole
    {
        Management = 1,
        PlatformManager,
        Engineer,
        General
    }

    public enum AggregateMethod
    {
        /// <summary>
        ///     Aggregation will not be applied
        /// </summary>
        None,

        /// <summary>
        ///     Returns the number of rows
        /// </summary>
        COUNT,

        /// <summary>
        ///     Returns the Maximum value in the given column
        /// </summary>
        MAX,

        /// <summary>
        ///     Returns the Minimum value in the given column
        /// </summary>
        MIN,

        /// <summary>
        ///     Returns the Average of the given column
        /// </summary>
        AVG,

        /// <summary>
        ///     Returns the SUM of the given column
        /// </summary>
        SUM,

        /// <summary>
        ///     Returns the Standard deviation of the given column
        /// </summary>
        STDEV,

        /// <summary>
        ///     Returns the variance of all the values in the given column
        /// </summary>
        VAR
    }

    /// <summary>
    ///     SQL Conditions
    /// </summary>
    public enum Condition
    {
        /// <summary>
        ///     No Condition will be applied
        /// </summary>
        None,

        /// <summary>
        ///     Applies Equal Operator
        /// </summary>
        Equals,

        /// <summary>
        ///     Applies Not Equal Operator
        /// </summary>
        NotEquals,

        /// <summary>
        ///     Applies Lesser than Operator
        /// </summary>
        LessThan,

        /// <summary>
        ///     Applies Greater than Operator
        /// </summary>
        GreaterThan,

        /// <summary>
        ///     Applies Lesser than or Equal Operator
        /// </summary>
        LessThanOrEquals,

        /// <summary>
        ///     Applies Greater than or Equals Operator
        /// </summary>
        GreaterThanOrEquals,

        /// <summary>
        ///     Applies for NULL values
        /// </summary>
        IS,

        /// <summary>
        ///     Applies for NULL values
        /// </summary>
        IN,

        /// <summary>
        ///     Applies for NULL values
        /// </summary>
        LIKE,

        NOTIN
    }

    /// <summary>
    ///     SQL Logical Operators
    /// </summary>
    public enum LogicalOperator
    {
        /// <summary>
        ///     No Condition Will be applied
        /// </summary>
        None,

        /// <summary>
        ///     Applies Logical OR operation
        /// </summary>
        OR,

        /// <summary>
        ///     Applies Logical AND operation
        /// </summary>
        AND,

        /// <summary>
        ///     Applies Logical IN operation
        /// </summary>
        IN,

        /// <summary>
        ///     Applies Logical LIKE operation
        /// </summary>
        LIKE,

        /// <summary>
        ///     Applies Logical NOT operation
        /// </summary>
        NOT
    }

    public enum OrderByType
    {
        /// <summary>
        ///     Default order will be used
        /// </summary>
        None,

        /// <summary>
        ///     Will be ordered in ascending order
        /// </summary>
        Asc,

        /// <summary>
        ///     Will be ordered in descending order
        /// </summary>
        Desc
    }

    public enum JoinType
    {
        /// <summary>
        ///     Inner Joins
        /// </summary>
        Inner,

        /// <summary>
        ///     Full Outer Join
        /// </summary>
        FullOuter,

        /// <summary>
        ///     Left Outer Join
        /// </summary>
        Left,

        /// <summary>
        ///     Right Outer Join
        /// </summary>
        RightOuter,

        /// <summary>
        ///     Cross Join
        /// </summary>
        Cross
    }
}