import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MangeModuleComponent } from './mange-module.component';

describe('MangeModuleComponent', () => {
  let component: MangeModuleComponent;
  let fixture: ComponentFixture<MangeModuleComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MangeModuleComponent]
    });
    fixture = TestBed.createComponent(MangeModuleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
