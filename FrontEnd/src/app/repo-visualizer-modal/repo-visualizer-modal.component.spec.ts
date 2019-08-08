import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RepoVisualizerModalComponent } from './repo-visualizer-modal.component';

describe('RepoVisualizerModalComponent', () => {
  let component: RepoVisualizerModalComponent;
  let fixture: ComponentFixture<RepoVisualizerModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RepoVisualizerModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RepoVisualizerModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
