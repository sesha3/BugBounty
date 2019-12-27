namespace Bug.Bounty.DataClasses
{
    using System;
    using System.Data;

    public class Result
    {
        public Result()
        {
            this.DataTable = new DataTable();
        }

        public DataTable DataTable
        {
            get;
            set;
        }

        public Exception Exception
        {
            get;
            set;
        }

        public object ReturnValue
        {
            get;
            set;
        }

        public bool Status
        {
            get;
            set;
        }
    }
}