import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { NnMgmtAuthorizationComponent } from './components/nn-mgmt-authorization.component';
import { NnMgmtAuthorizationService } from '@necnat.Abp/nn-mgmt-authorization';
import { of } from 'rxjs';

describe('NnMgmtAuthorizationComponent', () => {
  let component: NnMgmtAuthorizationComponent;
  let fixture: ComponentFixture<NnMgmtAuthorizationComponent>;
  const mockNnMgmtAuthorizationService = jasmine.createSpyObj('NnMgmtAuthorizationService', {
    sample: of([]),
  });
  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [NnMgmtAuthorizationComponent],
      providers: [
        {
          provide: NnMgmtAuthorizationService,
          useValue: mockNnMgmtAuthorizationService,
        },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NnMgmtAuthorizationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
