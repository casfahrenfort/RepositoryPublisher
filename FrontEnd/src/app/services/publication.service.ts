import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Publication } from '../models/publication.model';
import { environment } from 'src/environments/environment';
import { PublishResult } from '../models/publish-result.model';
import { FormGroup } from '@angular/forms';

@Injectable({
    providedIn: 'root'
})
export class PublicationService {
    constructor(private httpClient: HttpClient) { }

    public publishRepository(publication: Publication): Promise<PublishResult> {
        return this.httpClient.post(environment.apiUrl + 'publishing', publication)
            .toPromise()
            .then(result => <PublishResult>result)
            .catch(result => <PublishResult>result);
    }

    public publishMultipleRepositories(publications: Publication[]): Promise<PublishResult> {
        return this.httpClient.post(environment.apiUrl + 'publishing/multiple', publications)
            .toPromise()
            .then(result => <PublishResult>result)
            .catch(result => <PublishResult>result);
    }

    public createPublication(publishForm: FormGroup): Publication {
        let month = publishForm.controls['date'].value.month;
        if (month < 10) {
            month = "0" + month;
        }
        let day = publishForm.controls['date'].value.month;
        if (day < 10) {
            day = "0" + day;
        }
        let date = publishForm.controls['date'].value.year + '-' + month + '-' + day;
        
        return {
            versionControl: publishForm.controls['vcs'].value,
            publishingSystem: publishForm.controls['ps'].value,
            repoName: publishForm.controls['name'].value,
            repoURL: publishForm.controls['url'].value,
            snapshotId: publishForm.controls['snapshot'].value,
            token: publishForm.controls['token'].value,
            metaData: {
                author: publishForm.controls['author'].value,
                open_access: publishForm.controls['open_access'].value,
                contributors: publishForm.controls['contributors'].value,
                type: publishForm.controls['type'].value,
                description: publishForm.controls['description'].value,
                date: date,
                title: publishForm.controls['name'].value,
                subject: publishForm.controls['subject'].value,
                license: publishForm.controls['license'].value,
                keywords: publishForm.controls['keywords'].value,
                related: publishForm.controls['related'].value,
                language: publishForm.controls['language'].value,
            }

        };
    }
}