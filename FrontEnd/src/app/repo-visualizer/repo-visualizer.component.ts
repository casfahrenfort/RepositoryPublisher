import { Component, OnInit, Input } from '@angular/core';
import { RepoTree, RepoCommit } from '../models/repo-tree.model';
import { RepoVisualizerModalComponent } from '../repo-visualizer-modal/repo-visualizer-modal.component';

@Component({
  selector: 'app-repo-visualizer',
  templateUrl: './repo-visualizer.component.html',
  styleUrls: ['./repo-visualizer.component.css']
})
export class RepoVisualizerComponent implements OnInit {

  @Input()
  repoTree: RepoTree

  @Input()
  parent: RepoVisualizerModalComponent

  constructor() { }

  ngOnInit() {
  }

  public selectCommit(commit: RepoCommit) {
    this.parent.selectCommit(commit);
  }

}
