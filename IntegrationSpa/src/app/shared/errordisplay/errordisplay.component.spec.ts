import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ERRORDisplayComponent } from './errordisplay.component';

describe('ERRORDisplayComponent', () => {
  let component: ERRORDisplayComponent;
  let fixture: ComponentFixture<ERRORDisplayComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ERRORDisplayComponent]
    });
    fixture = TestBed.createComponent(ERRORDisplayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
