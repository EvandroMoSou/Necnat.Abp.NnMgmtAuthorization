import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class NnMgmtAuthorizationService {
  apiName = 'NnMgmtAuthorization';

  constructor(private restService: RestService) {}

  sample() {
    return this.restService.request<void, any>(
      { method: 'GET', url: '/api/NnMgmtAuthorization/sample' },
      { apiName: this.apiName }
    );
  }
}
