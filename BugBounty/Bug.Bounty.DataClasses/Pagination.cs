namespace Bug.Bounty.DataClasses
{
    /// <summary>
    ///     Default values for pagination
    /// </summary>
    public class Pagination
    {
        /// <summary>
        ///     Sets Default value 0 to skip parameter
        /// </summary>
        public const int DefaultSkip = 0;

        /// <summary>
        ///     Sets Default value 20 to take parameter
        /// </summary>
        public const int DefaultPageSize = 25;

        /// <summary>
        ///     Sets Default value 100
        /// </summary>
        public const int MaximumPageSize = 100;

        /// <summary>
        ///     Sets Default value 1
        /// </summary>
        public const int MinimumPageNumber = 1;

        /// <summary>
        ///     Sets Default value 1
        /// </summary>
        public const int MinimumPageSize = 1;
    }
}
