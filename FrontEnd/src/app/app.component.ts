import { Component, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { PublicationService } from './services/publication.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PublishResult } from './models/publish-result.model';
import { Publication } from './models/publication.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'FrontEnd';

  private publishForms: FormGroup[];

  private vcs: number = 0;
  private pid: number = 0;

  private publishing: boolean = false;

  private publishResult: PublishResult;

  @ViewChild('date', { static: false })
  date: ElementRef;

  @ViewChild("content", { static: true })
  content: NgbModal;

  constructor(private formBuilder: FormBuilder,
    private publicationService: PublicationService,
    private modalService: NgbModal) {

    this.publishForms = [this.formBuilder.group({
      vcs: ['git', Validators.required],
      ps: ['b2share', Validators.required],
      url: ['', Validators.required],
      name: ['', Validators.required],
      author: ['', Validators.required],
      contributors: '',
      version: '',
      description: ['', Validators.required],
      open_access: [false, Validators.required],
      type: ['software', Validators.required],
    })];
  }

  private addRepo() {
    this.publishForms.push(this.formBuilder.group({
      vcs: ['svn', Validators.required],
      ps: ['b2share', Validators.required],
      url: ['https://svn.riouxsvn.com/thesistest', Validators.required],
      name: ['Naampje', Validators.required],
      author: ['Iemand', Validators.required],
      contributors: '',
      version: '',
      description: ['Beschrijvingkje', Validators.required],
      open_access: [true, Validators.required],
      type: ['software', Validators.required],
    }));
  }

  private setVCS(index: number) {
    this.vcs = index;
  }

  private setPID(index: number) {
    this.pid = index;
  }

  private openModal() {
    this.modalService.open(this.content, { ariaLabelledBy: 'modal-basic-title' });
  }

  private publishAllRepos() {
    this.publishing = true;

    if (this.publishForms.length == 1) {
      let publishForm = this.publishForms[0];

      this.publicationService.publishRepository({
        versionControl: publishForm.controls['vcs'].value,
        publishingSystem: publishForm.controls['ps'].value,
        repoName: publishForm.controls['name'].value,
        repoURL: publishForm.controls['url'].value,
        metaData: {
          author: publishForm.controls['author'].value,
          open_access: publishForm.controls['open_access'].value,
          contributors: publishForm.controls['contributors'].value,
          type: publishForm.controls['type'].value,
          description: publishForm.controls['description'].value,
          version: publishForm.controls['version'].value,
          date: this.date.nativeElement.value,
          name: publishForm.controls['name'].value,
        }
      }).then(result => {
        this.publishing = false;
        this.publishResult = result;
        this.openModal();
      });
    } else {
      let publications: Publication[] = [];

      for (let i = 0; i < this.publishForms.length; i++) {
        let publishForm = this.publishForms[i];
        publications.push({
          versionControl: publishForm.controls['vcs'].value,
          publishingSystem: publishForm.controls['ps'].value,
          repoName: publishForm.controls['name'].value,
          repoURL: publishForm.controls['url'].value,
          metaData: {
            author: publishForm.controls['author'].value,
            open_access: publishForm.controls['open_access'].value,
            contributors: publishForm.controls['contributors'].value,
            type: publishForm.controls['type'].value,
            description: publishForm.controls['description'].value,
            version: publishForm.controls['version'].value,
            date: this.date.nativeElement.value,
            name: publishForm.controls['name'].value,
          }
        });
      }

      this.publicationService.publishMultipleRepositories(publications)
        .then(result => {
          this.publishing = false;
          this.publishResult = result;
          this.openModal();
        });
    }
  }

  private getTabName(form: FormGroup, index: number) {
    if (form.controls['name'].value == '') {
      return "Repo " + (index + 1);
    } else {
      return form.controls['name'].value;
    }
  }

  private getTabVcs(form: FormGroup) {
    return form.controls['vcs'].value;
  }

  private getButtonText() {
    if (this.publishForms.length == 1) {
      return 'repository';
    } else {
      return 'repositories';
    }
  }

  private canPublish(): boolean {
    for (let i = 0; i < this.publishForms.length; i++) {
      if (!this.publishForms[i].valid) {
        return false;
      }
    }
    return true;
  }
}
