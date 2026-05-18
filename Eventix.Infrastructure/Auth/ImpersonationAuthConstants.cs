using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Infrastructure.Auth
{
    public static class ImpersonationAuthConstants
    {
        public const string IsImpersonationClaim = "isImpersonation";
        public const string ImpersonationSessionIdClaim = "impersonationSessionId";
    }
}
