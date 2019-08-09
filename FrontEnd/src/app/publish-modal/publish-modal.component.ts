import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { PublicationService } from '../services/publication.service';
import { PublishResult } from '../models/publish-result.model';
import { Publication } from '../models/publication.model';

@Component({
  selector: 'app-publish-modal',
  templateUrl: './publish-modal.component.html',
  styleUrls: ['./publish-modal.component.css']
})
export class PublishModalComponent implements OnInit {


  @ViewChild("content", { static: true })
  public content: NgbModal;

  @Input()
  public publishForms: FormGroup[];

  public bundleInfo: FormGroup;

  public publishing = false;
  public requireBundleInfo = false;
  public publishResult: PublishResult = undefined;

  constructor(private modalService: NgbModal,
    private publicationService: PublicationService,
    private formBuilder: FormBuilder) {
    this.bundleInfo = this.formBuilder.group({
      ps: ['b2share', Validators.required],
      name: ['', Validators.required],
      author: ['', Validators.required],
      contributors: '',
      version: '',
      description: ['', Validators.required],
      open_access: [false, Validators.required],
      type: ['software', Validators.required],
      date: ['']
    });
  }

  ngOnInit() {
  }

  public openModal() {
    this.publishResult = undefined;
    this.modalService.open(this.content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
  }

  public publishSingleRepository(publishForm: FormGroup) {
    this.publishing = true;
    this.openModal();

    this.publicationService.publishRepository({
      versionControl: publishForm.controls['vcs'].value,
      publishingSystem: publishForm.controls['ps'].value,
      repoName: publishForm.controls['name'].value,
      repoURL: publishForm.controls['url'].value,
      snapshotId: publishForm.controls['snapshot'].value,
      metaData: {
        author: publishForm.controls['author'].value,
        open_access: publishForm.controls['open_access'].value,
        contributors: publishForm.controls['contributors'].value,
        type: publishForm.controls['type'].value,
        description: publishForm.controls['description'].value,
        version: publishForm.controls['version'].value,
        // date: this.date.nativeElement.value,
        date: '01-01-2019',
        name: publishForm.controls['name'].value,
      }
    }).then(result => {
      this.publishing = false;
      this.publishResult = result;
    });
  }

  public openMultipleRepositories() {
    this.requireBundleInfo = true;
    this.openModal();
  }

  public publishMultipleRepositories() {
    this.publishing = true;
    this.requireBundleInfo = false;

    let publications: Publication[] = [];

    for (let i = 0; i < this.publishForms.length; i++) {
      let publishForm = this.publishForms[i];
      publications.push({
        versionControl: publishForm.controls['vcs'].value,
        publishingSystem: publishForm.controls['ps'].value,
        repoName: publishForm.controls['name'].value,
        repoURL: publishForm.controls['url'].value,
        snapshotId: publishForm.controls['snapshot'].value,
        metaData: {
          author: publishForm.controls['author'].value,
          open_access: publishForm.controls['open_access'].value,
          contributors: publishForm.controls['contributors'].value,
          type: publishForm.controls['type'].value,
          description: publishForm.controls['description'].value,
          version: publishForm.controls['version'].value,
          // date: this.date.nativeElement.value,
          date: '01-01-2019',
          name: publishForm.controls['name'].value,
        }
      });
    }

    publications.push({
      versionControl: '',
      publishingSystem: this.bundleInfo.controls['ps'].value,
      repoName: this.bundleInfo.controls['name'].value,
      repoURL: '',
      snapshotId: '',
      metaData: {
        author: this.bundleInfo.controls['author'].value,
        open_access: this.bundleInfo.controls['open_access'].value,
        contributors: this.bundleInfo.controls['contributors'].value,
        type: this.bundleInfo.controls['type'].value,
        description: this.bundleInfo.controls['description'].value,
        version: this.bundleInfo.controls['version'].value,
        // date: this.date.nativeElement.value,
        date: '01-01-2019',
        name: this.bundleInfo.controls['name'].value,
      }
    });

    this.publicationService.publishMultipleRepositories(publications)
      .then(result => {
        this.publishing = false;
        this.publishResult = result;
      });
  }
}
