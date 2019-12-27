namespace Bug.Bounty.LoginService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BoldApiEndPoints
    {
        public const string TokenEndPoint = "/token";

        public const string SyncfusionLogin = "/account/isvalidsyncfusionlogin";

        public const string UpdateEvaluation = "/download/logfreetrial";

        public const string GetUnlockKey = "/unlockkey";

        public const string LogInstalledKey = "/logkeyinstall";
    }
}
