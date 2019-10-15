import { Component, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { PublishModalComponent } from './publish-modal/publish-modal.component';
import { Title } from '@angular/platform-browser';
import { InformationModalComponent } from './information-modal/information-modal.component';

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

  @ViewChild('infoModal', { static: true })
  infoModal: InformationModalComponent;

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
      token: ['', Validators.required],
      url: ['', Validators.required],
      snapshot: ['none', Validators.required],
      name: ['', Validators.required],
      author: ['', Validators.required],
      contributors: '',
      description: ['', Validators.required],
      open_access: [true, Validators.required],
      type: ['software', Validators.required],
      date: [''],
      subject: '',
      language: '',
      related: '',
      license: '',
      keywords: '',
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
      if(this.publishForms.length == 1) {
        if(!this.publishForms[0].valid) {
          return false;
        }
      } else {
        for (let i = 0; i < this.publishForms.length; i++) {

          if (!this.publishForms[i].valid) {
    
            let onlyTokenInvalid = true;
            let keys = Object.keys(this.publishForms[i].controls);
            for(let j = 0; j < keys.length; j++) {
              if (!this.publishForms[i].controls[keys[j]].valid) {
                if (keys[j] != 'token') {
                  onlyTokenInvalid = false;
                }
              }
            }
    
            if(onlyTokenInvalid) {
              continue;
            } else {
              return false;
            }
          }

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
