import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { RepoTree, RepoCommit } from '../models/repo-tree.model';
import { RepoService } from '../services/repo.service';
import { PublishModalComponent } from '../publish-modal/publish-modal.component';

@Component({
  selector: 'app-publication-form',
  templateUrl: './publication-form.component.html',
  styleUrls: ['./publication-form.component.css']
})
export class PublicationFormComponent implements OnInit {

  @Input()
  public publishForm: FormGroup;

  public repoTree: RepoTree;
  public selectedCommit: RepoCommit;

  public loadingRepoTree = false;
  public loadingRepoTreeError = false;

  @ViewChild('repoTreeModal', { static: true })
  modal: PublishModalComponent;

  constructor(private repoService: RepoService) { }

  ngOnInit() {
  }

  public getTabVcs(form: FormGroup) {
    return this.publishForm.controls['vcs'].value;
  }

  public getRepoTree() {
    if(!this.publishForm.controls['url'].valid) {
      return;
    }

    this.loadingRepoTreeError = false;

    this.loadingRepoTree = true;
    this.repoService.getRepoTree({
      repoUrl: this.publishForm.controls['url'].value,
      versionControl: this.publishForm.controls['vcs'].value
    }).then(result => {
      this.repoTree = result;
      this.loadingRepoTree = false;
      this.selectedCommit = this.repoTree.commits[0];
    }).catch(result => {
      this.loadingRepoTree = false;
      this.loadingRepoTreeError = true;
    });
  }

  public selectCommit(commit: RepoCommit) {
    this.selectedCommit = commit;
  }

}
