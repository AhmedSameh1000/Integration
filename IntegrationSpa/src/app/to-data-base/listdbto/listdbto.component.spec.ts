import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListdbtoComponent } from './listdbto.component';

describe('ListdbtoComponent', () => {
  let component: ListdbtoComponent;
  let fixture: ComponentFixture<ListdbtoComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ListdbtoComponent]
    });
    fixture = TestBed.createComponent(ListdbtoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
