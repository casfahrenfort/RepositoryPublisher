import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { RepoTreeRequest } from '../models/repo-tree-request.model';
import { RepoTree } from '../models/repo-tree.model';

@Injectable({
    providedIn: 'root'
})
export class RepoService {
    constructor(private httpClient: HttpClient) {}

    public getRepoTree(repoTreeRequest: RepoTreeRequest): Promise<RepoTree> {
        return this.httpClient.post(environment.apiUrl + 'repo', repoTreeRequest)
            .toPromise()
            .then(result => <RepoTree>result);
    }
}