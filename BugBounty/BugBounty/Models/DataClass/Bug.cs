namespace BugBounty
{
    using System;

    public class Bug
    {
        public Guid Id
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public Guid CreatedUserID
        {
            get;
            set;
        }

        public Guid ValidatedUserID
        {
            get;
            set;
        }

        public Platform Platform
        {
            get;
            set;
        }

        public int Severity
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        }

        public string CreatedUser
        {
            get;
            set;
        }

        public string ValidatedUser
        {
            get;
            set;
        }
    }
}