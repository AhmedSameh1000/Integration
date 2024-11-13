import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListdbfromComponent } from './listdbfrom.component';

describe('ListdbfromComponent', () => {
  let component: ListdbfromComponent;
  let fixture: ComponentFixture<ListdbfromComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ListdbfromComponent]
    });
    fixture = TestBed.createComponent(ListdbfromComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
