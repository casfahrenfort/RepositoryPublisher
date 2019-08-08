import { Component, OnInit, Input } from '@angular/core';
import { RepoDirectory } from '../models/repo-tree.model';

@Component({
  selector: 'app-repo-directory-visualizer',
  templateUrl: './repo-directory-visualizer.component.html',
  styleUrls: ['./repo-directory-visualizer.component.css']
})
export class RepoDirectoryVisualizerComponent implements OnInit {

  @Input()
  directory: RepoDirectory;

  private expanded = false;

  constructor() { }

  ngOnInit() {
  }

}
