import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MangeDataBaseColumnsComponent } from './mange-data-base-columns.component';

describe('MangeDataBaseColumnsComponent', () => {
  let component: MangeDataBaseColumnsComponent;
  let fixture: ComponentFixture<MangeDataBaseColumnsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MangeDataBaseColumnsComponent]
    });
    fixture = TestBed.createComponent(MangeDataBaseColumnsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
