import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RepoWeekVisualizerComponent } from './repo-week-visualizer.component';

describe('RepoWeekVisualizerComponent', () => {
  let component: RepoWeekVisualizerComponent;
  let fixture: ComponentFixture<RepoWeekVisualizerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RepoWeekVisualizerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RepoWeekVisualizerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
