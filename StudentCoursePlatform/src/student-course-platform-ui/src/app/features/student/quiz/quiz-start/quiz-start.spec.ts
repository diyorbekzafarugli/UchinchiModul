import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuizStart } from './quiz-start';

describe('QuizStart', () => {
  let component: QuizStart;
  let fixture: ComponentFixture<QuizStart>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuizStart],
    }).compileComponents();

    fixture = TestBed.createComponent(QuizStart);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
