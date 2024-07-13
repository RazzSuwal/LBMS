import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditbooksComponent } from './editbooks.component';

describe('EditbooksComponent', () => {
  let component: EditbooksComponent;
  let fixture: ComponentFixture<EditbooksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EditbooksComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EditbooksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
