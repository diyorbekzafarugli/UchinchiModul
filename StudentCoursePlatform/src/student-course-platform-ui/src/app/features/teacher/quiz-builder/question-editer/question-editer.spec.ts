import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuestionEditer } from './question-editer';

describe('QuestionEditer', () => {
  let component: QuestionEditer;
  let fixture: ComponentFixture<QuestionEditer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuestionEditer],
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionEditer);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
