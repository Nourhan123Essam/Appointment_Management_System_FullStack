import { TestBed } from '@angular/core/testing';

import { DoctorStateService } from './doctor-state.service';

describe('DoctorStateService', () => {
  let service: DoctorStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DoctorStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
