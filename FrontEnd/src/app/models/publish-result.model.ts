export interface PublishResult {
    message: string;
    publishUrl?: string;
    duplicatePublicationUrl?: string;
    publishingSystemResponse?: PublishingSystemResult; 
    error?: any;
}

export interface PublishingSystemResult {
    status?: string;
    code?: string;
    message: string;
}