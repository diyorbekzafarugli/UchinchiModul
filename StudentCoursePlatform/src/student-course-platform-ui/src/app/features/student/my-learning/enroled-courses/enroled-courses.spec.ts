import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EnroledCourses } from './enroled-courses';

describe('EnroledCourses', () => {
  let component: EnroledCourses;
  let fixture: ComponentFixture<EnroledCourses>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EnroledCourses],
    }).compileComponents();

    fixture = TestBed.createComponent(EnroledCourses);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
