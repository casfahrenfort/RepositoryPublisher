import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';


import { AppComponent } from './app.component';
import { PublicationService } from './services/publication.service';
import { PublishModalComponent } from './publish-modal/publish-modal.component';
import { PublicationFormComponent } from './publication-form/publication-form.component';
import { RepoVisualizerComponent } from './repo-visualizer/repo-visualizer.component';
import { RepoCommitVisualizerComponent } from './repo-commit-visualizer/repo-commit-visualizer.component';
import { RepoDirectoryVisualizerComponent } from './repo-directory-visualizer/repo-directory-visualizer.component';
import { RepoVisualizerModalComponent } from './repo-visualizer-modal/repo-visualizer-modal.component';

@NgModule({
  declarations: [
    AppComponent,
    PublishModalComponent,
    PublicationFormComponent,
    RepoVisualizerComponent,
    RepoCommitVisualizerComponent,
    RepoDirectoryVisualizerComponent,
    RepoVisualizerModalComponent
  ],
  imports: [
    BrowserModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [
    PublicationService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
