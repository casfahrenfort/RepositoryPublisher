import { Component, OnInit, Input } from '@angular/core';
import { RepoCommit } from '../models/repo-tree.model';
import { RepoWeekVisualizerComponent } from '../repo-week-visualizer/repo-week-visualizer.component';

@Component({
  selector: 'app-repo-commit-visualizer',
  templateUrl: './repo-commit-visualizer.component.html',
  styleUrls: ['./repo-commit-visualizer.component.css']
})
export class RepoCommitVisualizerComponent implements OnInit {

  @Input()
  commit: RepoCommit;

  @Input()
  parent: RepoWeekVisualizerComponent;

  expanded = false;

  constructor() {
  }

  ngOnInit() {
  }

  public select() {
    this.parent.selectCommit(this.commit);
  }

}
