import { MetaData } from './metadata.model';

export interface Publication {
    versionControl: string;
    publishingSystem: string;
    repoURL: string;
    repoName: string;
    snapshotId: string;
    token: string;
    metaData: MetaData;
}