import { ModuleWithProviders, NgModule } from '@angular/core';
import { NN_MGMT_AUTHORIZATION_ROUTE_PROVIDERS } from './providers/route.provider';

@NgModule()
export class NnMgmtAuthorizationConfigModule {
  static forRoot(): ModuleWithProviders<NnMgmtAuthorizationConfigModule> {
    return {
      ngModule: NnMgmtAuthorizationConfigModule,
      providers: [NN_MGMT_AUTHORIZATION_ROUTE_PROVIDERS],
    };
  }
}
