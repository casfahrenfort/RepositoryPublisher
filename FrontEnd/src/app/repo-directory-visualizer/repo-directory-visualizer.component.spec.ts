import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RepoDirectoryVisualizerComponent } from './repo-directory-visualizer.component';

describe('RepoDirectoryVisualizerComponent', () => {
  let component: RepoDirectoryVisualizerComponent;
  let fixture: ComponentFixture<RepoDirectoryVisualizerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RepoDirectoryVisualizerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RepoDirectoryVisualizerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
