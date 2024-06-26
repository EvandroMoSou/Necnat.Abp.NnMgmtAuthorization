import { NgModule, NgModuleFactory, ModuleWithProviders } from '@angular/core';
import { CoreModule, LazyModuleFactory } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NnMgmtAuthorizationComponent } from './components/nn-mgmt-authorization.component';
import { NnMgmtAuthorizationRoutingModule } from './nn-mgmt-authorization-routing.module';

@NgModule({
  declarations: [NnMgmtAuthorizationComponent],
  imports: [CoreModule, ThemeSharedModule, NnMgmtAuthorizationRoutingModule],
  exports: [NnMgmtAuthorizationComponent],
})
export class NnMgmtAuthorizationModule {
  static forChild(): ModuleWithProviders<NnMgmtAuthorizationModule> {
    return {
      ngModule: NnMgmtAuthorizationModule,
      providers: [],
    };
  }

  static forLazy(): NgModuleFactory<NnMgmtAuthorizationModule> {
    return new LazyModuleFactory(NnMgmtAuthorizationModule.forChild());
  }
}
