export interface RepoTree {
    commits: RepoCommit[];
}

export interface RepoCommit {
    selected: boolean;
    commitId: string;
    date: string;
    directories: RepoDirectory[];
    files: string[];
}

export interface RepoDirectory {
    name: string;
    directories: RepoDirectory[];
    files: string[];
}