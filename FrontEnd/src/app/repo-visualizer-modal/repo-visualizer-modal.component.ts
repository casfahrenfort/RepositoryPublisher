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

  @Input()
  public parent: PublicationFormComponent;
  
  constructor(private modalService: NgbModal) {
  }

  ngOnInit() {
  }

  public openModal() {
    this.modalService.open(this.content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
  }

  public closeModal() {
    this.modalService.dismissAll();
  }

  public selectCommit(commit: RepoCommit): void {
    this.parent.selectCommit(commit);
  }

}
