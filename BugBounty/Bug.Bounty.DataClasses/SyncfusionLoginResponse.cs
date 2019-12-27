namespace Bug.Bounty.DataClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SyncfusionLoginResponse : SyncfusionLoginRequest
    {
        public string CustomerName
        {
            get;
            set;
        }

        public string Company
        {
            get;
            set;
        }

        public string Phone
        {
            get;
            set;
        }

        public int StatusCode
        {
            get;
            set;
        }

        public bool Status
        {
            get;
            set;
        }

        public bool IsActivated
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public bool HavePrivacyConsent
        {
            get;
            set;
        }
    }
}
