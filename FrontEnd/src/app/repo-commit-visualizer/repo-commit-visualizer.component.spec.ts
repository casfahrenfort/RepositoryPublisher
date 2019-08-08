import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RepoCommitVisualizerComponent } from './repo-commit-visualizer.component';

describe('RepoCommitVisualizerComponent', () => {
  let component: RepoCommitVisualizerComponent;
  let fixture: ComponentFixture<RepoCommitVisualizerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RepoCommitVisualizerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RepoCommitVisualizerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
