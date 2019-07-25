import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Publication } from '../models/publication.model';
import { environment } from 'src/environments/environment';
import { PublishResult } from '../models/publish-result.model';

@Injectable({
    providedIn: 'root'
})
export class PublicationService {
    constructor(private httpClient: HttpClient) {}

    public publishRepository(publication: Publication): Promise<PublishResult> {
        return this.httpClient.post(environment.apiUrl + 'publishing', publication)
            .toPromise()
            .then(result => <PublishResult>result)
            .catch(result => <PublishResult>result);
    }

    public publishMultipleRepositories(publications: Publication[]) : Promise<PublishResult> {
        return this.httpClient.post(environment.apiUrl + 'publishing/multiple', publications)
            .toPromise()
            .then(result => <PublishResult>result)
            .catch(result => <PublishResult>result);
    }

}