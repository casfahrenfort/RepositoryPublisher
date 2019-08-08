import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { RepoTree, RepoCommit } from '../models/repo-tree.model';
import { PublicationFormComponent } from '../publication-form/publication-form.component';

@Component({
  selector: 'app-repo-visualizer-modal',
  templateUrl: './repo-visualizer-modal.component.html',
  styleUrls: ['./repo-visualizer-modal.component.css']
})
export class RepoVisualizerModalComponent implements OnInit {

  @ViewChild("content", { static: true })
  public content: NgbModal;

  @Input()
  public repoTree: RepoTree;

  public selectedCommit: RepoCommit;

  @Input()
  public parent: PublicationFormComponent;
  
  constructor(private modalService: NgbModal) {
  }

  ngOnInit() {
  }

  public openModal() {
    if (this.repoTree) {
      for(let i = 1; i< this.repoTree.commits.length; i++) {
        this.repoTree.commits[i].selected = false;
      }

      this.selectedCommit = this.repoTree.commits[0];
      this.selectedCommit.selected = true;
    }
    this.modalService.open(this.content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
  }

  public closeModal() {
    this.modalService.dismissAll();
  }

  public selectCommit(commit: RepoCommit): void {
    this.selectedCommit.selected = false;
    commit.selected = true;
    this.selectedCommit = commit;

    this.parent.selectCommit(commit);
  }

}
