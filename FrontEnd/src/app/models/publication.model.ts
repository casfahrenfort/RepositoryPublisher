import { MetaData } from './metadata.model';

export interface Publication {
    versionControl: string;
    publishingSystem: string;
    repoURL: string;
    repoName: string;
    metaData: MetaData;
}