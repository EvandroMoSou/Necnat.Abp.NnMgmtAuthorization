using AutoMapper;
using Necnat.Abp.NnMgmtAuthorization.Domains;

namespace Necnat.Abp.NnMgmtAuthorization;

public class NnMgmtAuthorizationApplicationAutoMapperProfile : Profile
{
    public NnMgmtAuthorizationApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<HierarchicalAccess, HierarchicalAccessDto>()
            .ForMember(x => x.UserName, opt => opt.Ignore())
            .ForMember(x => x.RoleName, opt => opt.Ignore())
            .ForMember(x => x.HierarchyComponentId, opt => opt.Ignore())
            .ForMember(x => x.DistributedAppName, opt => opt.Ignore());
        CreateMap<HierarchicalAccessDto, HierarchicalAccess>()
            .ForMember(x => x.TenantId, opt => opt.Ignore())
            .ForMember(x => x.LastModificationTime, opt => opt.Ignore())
            .ForMember(x => x.LastModifierId, opt => opt.Ignore())
            .ForMember(x => x.CreationTime, opt => opt.Ignore())
            .ForMember(x => x.CreatorId, opt => opt.Ignore())
            .ForMember(x => x.ExtraProperties, opt => opt.Ignore())
            .ForMember(x => x.ConcurrencyStamp, opt => opt.Ignore());

        CreateMap<HierarchicalStructure, HierarchicalStructureDto>()
            .ForMember(x => x.HierarchyComponentName, opt => opt.Ignore());
        CreateMap<HierarchicalStructureDto, HierarchicalStructure>()
            .ForMember(x => x.Hierarchy, opt => opt.Ignore())
            .ForMember(x => x.HierarchicalStructureParent, opt => opt.Ignore())
            .ForMember(x => x.TenantId, opt => opt.Ignore())
            .ForMember(x => x.LastModificationTime, opt => opt.Ignore())
            .ForMember(x => x.LastModifierId, opt => opt.Ignore())
            .ForMember(x => x.CreationTime, opt => opt.Ignore())
            .ForMember(x => x.CreatorId, opt => opt.Ignore())
            .ForMember(x => x.ExtraProperties, opt => opt.Ignore())
            .ForMember(x => x.ConcurrencyStamp, opt => opt.Ignore());

        CreateMap<Hierarchy, HierarchyDto>();
        CreateMap<HierarchyDto, Hierarchy>()
            .ForMember(x => x.TenantId, opt => opt.Ignore())
            .ForMember(x => x.LastModificationTime, opt => opt.Ignore())
            .ForMember(x => x.LastModifierId, opt => opt.Ignore())
            .ForMember(x => x.CreationTime, opt => opt.Ignore())
            .ForMember(x => x.CreatorId, opt => opt.Ignore())
            .ForMember(x => x.ExtraProperties, opt => opt.Ignore())
            .ForMember(x => x.ConcurrencyStamp, opt => opt.Ignore());

        CreateMap<HierarchyComponentGroup, HierarchyComponentGroupDto>();
        CreateMap<HierarchyComponentGroupDto, HierarchyComponentGroup>()
            .ForMember(x => x.TenantId, opt => opt.Ignore())
            .ForMember(x => x.LastModificationTime, opt => opt.Ignore())
            .ForMember(x => x.LastModifierId, opt => opt.Ignore())
            .ForMember(x => x.CreationTime, opt => opt.Ignore())
            .ForMember(x => x.CreatorId, opt => opt.Ignore())
            .ForMember(x => x.ExtraProperties, opt => opt.Ignore())
            .ForMember(x => x.ConcurrencyStamp, opt => opt.Ignore());
    }
}
