import { Component, OnInit, Input } from '@angular/core';
import { RepoWeek, RepoCommit } from '../models/repo-tree.model';
import { RepoVisualizerComponent } from '../repo-visualizer/repo-visualizer.component';

@Component({
  selector: 'app-repo-week-visualizer',
  templateUrl: './repo-week-visualizer.component.html',
  styleUrls: ['./repo-week-visualizer.component.css']
})
export class RepoWeekVisualizerComponent implements OnInit {

  @Input()
  week: RepoWeek;

  @Input()
  parent: RepoVisualizerComponent

  expanded = false;

  constructor() { }

  ngOnInit() {
  }

  public selectCommit(commit: RepoCommit) {
    this.parent.selectCommit(commit);
  }

}
