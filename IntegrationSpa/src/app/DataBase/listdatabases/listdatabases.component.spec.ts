import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListdatabasesComponent } from './listdatabases.component';

describe('ListdatabasesComponent', () => {
  let component: ListdatabasesComponent;
  let fixture: ComponentFixture<ListdatabasesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ListdatabasesComponent]
    });
    fixture = TestBed.createComponent(ListdatabasesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
