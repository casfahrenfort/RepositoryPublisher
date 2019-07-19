import { Component, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { PublicationService } from './services/publication.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PublishResult } from './models/publish-result.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'FrontEnd';

  private publishForm: FormGroup;

  private vcs: number = 0;
  private pid: number = 0;

  private publishing: boolean = false;

  private publishResult: PublishResult;
  
  @ViewChild('date', { static: false}) 
  date: ElementRef;

  @ViewChild("content", { static: true }) 
  content: NgbModal;

  constructor(private formBuilder: FormBuilder,
    private publicationService: PublicationService,
    private modalService: NgbModal) {

    this.publishForm = this.formBuilder.group({
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
    });
  }

  private setVCS(index: number) {
    this.vcs = index;
  }

  private setPID(index: number) {
    this.pid = index;
  }
  
  private openModal() {
    this.modalService.open(this.content, {ariaLabelledBy: 'modal-basic-title'});
  }

  private publishRepo() {
    this.publishing = true;
    this.publicationService.publishRepository({
      versionControl: this.publishForm.controls['vcs'].value,
      publishingSystem: this.publishForm.controls['ps'].value,
      repoName: this.publishForm.controls['name'].value,
      repoURL: this.publishForm.controls['url'].value,
      metaData: {
        author: this.publishForm.controls['author'].value,
        open_access: this.publishForm.controls['open_access'].value,
        contributors: this.publishForm.controls['contributors'].value,
        type: this.publishForm.controls['type'].value,
        description: this.publishForm.controls['description'].value,
        version: this.publishForm.controls['version'].value,
        date: this.date.nativeElement.value,
        name: this.publishForm.controls['name'].value,
      }
    }).then(result => {
      this.publishing = false;
      this.publishResult = result;
      this.openModal();
    });
  }
}
