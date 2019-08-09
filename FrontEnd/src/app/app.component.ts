import { Component, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { PublicationService } from './services/publication.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PublishResult } from './models/publish-result.model';
import { Publication } from './models/publication.model';
import { PublishModalComponent } from './publish-modal/publish-modal.component';
import { Title } from '@angular/platform-browser';
import { RepoTree } from './models/repo-tree.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Repository Publisher';

  public publishForms: FormGroup[];

  @ViewChild('date', { static: false })
  date: ElementRef;

  @ViewChild('publishModal', { static: true })
  modal: PublishModalComponent;

  constructor(private formBuilder: FormBuilder,
    private titleService: Title) {
    
    this.titleService.setTitle('Repository Publisher');

    this.publishForms = [];
    this.addRepo();
  }

  public addRepo() {
    this.publishForms.push(this.formBuilder.group({
      vcs: ['git', Validators.required],
      ps: ['b2share', Validators.required],
      url: ['', Validators.required],
      name: ['', Validators.required],
      snapshot: ['none', Validators.required],
      author: ['', Validators.required],
      contributors: '',
      version: '',
      description: ['', Validators.required],
      open_access: [true, Validators.required],
      type: ['software', Validators.required],
      date: [''],
      pubSnap: ['false']
    }));
  }

  public publishAllRepos() {
    if (this.publishForms.length == 1) {
      this.modal.publishSingleRepository(this.publishForms[0]);
    } else {
      this.modal.openMultipleRepositories();
    }
  }

  public getTabName(form: FormGroup, index: number) {
    if (form.controls['name'].value == '') {
      return "Repo " + (index + 1);
    } else {
      return form.controls['name'].value;
    }
  }

  public getButtonText() {
    if (this.publishForms.length == 1) {
      return 'repository';
    } else {
      return 'repositories';
    }
  }

  public canPublish(): boolean {
    for (let i = 0; i < this.publishForms.length; i++) {
      if (!this.publishForms[i].valid) {
        return false;
      }
    }
    return true;
  }

  public canAdd(): boolean {
    if(this.modal.publishing) {
      return false;
    } else {
      return true;
    }
  }

  public closeTab(index: number, event): void {
    if(this.publishForms.length == 1) {
      return;
    } else {
      this.publishForms.splice(index, 1);
      event.preventDefault();
    }
  }
}
