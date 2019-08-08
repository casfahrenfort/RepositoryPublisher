import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RepoVisualizerComponent } from './repo-visualizer.component';

describe('RepoVisualizerComponent', () => {
  let component: RepoVisualizerComponent;
  let fixture: ComponentFixture<RepoVisualizerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RepoVisualizerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RepoVisualizerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
