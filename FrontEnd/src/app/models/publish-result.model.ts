export interface PublishResult {
    message: string;
    publishUrl?: string;
    duplicatePublicationUrl?: string;
    b2shareResponse?: B2ShareResult; 
    error?: any;
}

export interface B2ShareResult {
    status: string;
    message: string;
}