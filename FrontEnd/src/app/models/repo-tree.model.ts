export interface RepoTree {
    repoWeeks: RepoWeek[];
}

export interface RepoWeek {
    weekNumber: number;
    yearNumber: number;
    commits: RepoCommit[];
}

export interface RepoCommit {
    selected: boolean;
    commitId: string;
    message: string;
    date: string;
    directories: RepoDirectory[];
    files: string[];
}

export interface RepoDirectory {
    name: string;
    directories: RepoDirectory[];
    files: string[];
}