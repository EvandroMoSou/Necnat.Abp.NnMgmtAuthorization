import { Component, OnInit } from '@angular/core';
import { NnMgmtAuthorizationService } from '../services/nn-mgmt-authorization.service';

@Component({
  selector: 'lib-nn-mgmt-authorization',
  template: ` <p>nn-mgmt-authorization works!</p> `,
  styles: [],
})
export class NnMgmtAuthorizationComponent implements OnInit {
  constructor(private service: NnMgmtAuthorizationService) {}

  ngOnInit(): void {
    this.service.sample().subscribe(console.log);
  }
}
