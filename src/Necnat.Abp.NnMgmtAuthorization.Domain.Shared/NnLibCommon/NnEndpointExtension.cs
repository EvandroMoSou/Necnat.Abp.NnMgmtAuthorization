using Necnat.Abp.NnLibCommon.Domains;
using System.Linq;

namespace Necnat.Abp.NnMgmtAuthorization.Debug
{
    public static class NnEndpointExtension
    {
        public static bool HasParameter(this NnEndpointModel nnEndpoint)
        {
            return nnEndpoint.Tag.Contains(":");
        }

        public static string GetParameter(this NnEndpointModel nnEndpoint, int index)
        {
            return nnEndpoint.Tag.Split(":")[index];
        }

        public static bool IsUrl(this NnEndpointModel nnEndpoint)
        {
            return nnEndpoint.UrlUri.ToCharArray().Count(c => c == '/') < 3;
        }
    }
}
