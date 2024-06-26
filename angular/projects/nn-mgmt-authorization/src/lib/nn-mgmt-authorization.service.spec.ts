import { TestBed } from '@angular/core/testing';
import { NnMgmtAuthorizationService } from './services/nn-mgmt-authorization.service';
import { RestService } from '@abp/ng.core';

describe('NnMgmtAuthorizationService', () => {
  let service: NnMgmtAuthorizationService;
  const mockRestService = jasmine.createSpyObj('RestService', ['request']);
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: RestService,
          useValue: mockRestService,
        },
      ],
    });
    service = TestBed.inject(NnMgmtAuthorizationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
