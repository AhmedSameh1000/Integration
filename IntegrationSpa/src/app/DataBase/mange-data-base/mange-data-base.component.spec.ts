import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MangeDataBaseComponent } from './mange-data-base.component';

describe('MangeDataBaseComponent', () => {
  let component: MangeDataBaseComponent;
  let fixture: ComponentFixture<MangeDataBaseComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MangeDataBaseComponent]
    });
    fixture = TestBed.createComponent(MangeDataBaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
