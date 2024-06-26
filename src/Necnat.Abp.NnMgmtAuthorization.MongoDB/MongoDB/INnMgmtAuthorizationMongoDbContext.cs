﻿using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Necnat.Abp.NnMgmtAuthorization.MongoDB;

[ConnectionStringName(NnMgmtAuthorizationDbProperties.ConnectionStringName)]
public interface INnMgmtAuthorizationMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
