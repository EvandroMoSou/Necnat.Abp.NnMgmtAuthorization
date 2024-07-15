using Necnat.Abp.NnLibCommon.Domains;

namespace Necnat.Abp.NnMgmtAuthorization
{
    public static class NnMgmtAuthorizationConsts
    {
        public const string UserAuthorizationUserId = "NnAuthz:UserId";
        public const string UserAuthorizationLHAC = "NnAuthz:LHAC ";
        public const string UserAuthorizationLHS = "NnAuthz:LHS";
        public const string UserAuthorizationLHC = "NnAuthz:LHC";

        public const string HttpClientName = "NnMgmtAuthorization";

        public const string NnEndpointTagGetListUser = "NnEndpoint:GetListUser";
        public const string NnEndpointTagGetListHierarchyComponentIdRecursive = "NnEndpoint:GetListHierarchyComponentIdRecursive";
        public const string NnEndpointTagGetUserAuthzInfoMy = "NnEndpoint:GetUserAuthzInfoMy";
        public const string NnEndpointTagGetHierarchyAuthzInfo = "NnEndpoint:GetHierarchyAuthzInfo";
        public const string NnEndpointTagHierarchyComponentContributor = "NnEndpoint:HierarchyComponentContributor";
        public const string NnEndpointTagHierarchyComponentTypeContributor = "NnEndpoint:HierarchyComponentTypeContributor";
    }
}