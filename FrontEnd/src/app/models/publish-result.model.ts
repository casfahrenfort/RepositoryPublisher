export interface PublishResult {
    message: string;
    publishUrl?: string;
    b2shareResponse?: B2ShareResult; 
}

export interface B2ShareResult {
    status: string;
    message: string;
}