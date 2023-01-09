/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { palestrantesComponent } from './palestrantes.component';

describe('palestrantesComponent', () => {
  let component: palestrantesComponent;
  let fixture: ComponentFixture<palestrantesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ palestrantesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(palestrantesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
