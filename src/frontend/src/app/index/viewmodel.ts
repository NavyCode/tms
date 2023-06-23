
import { List } from 'linqts';
import { Subject } from 'rxjs';
import { RequestManager } from '../api/reports/RequestManager';
import { GetProjectsResponse, ProjectItem } from '../api/reports/swagger';
import { Link } from '../ext/links';

export class ViewModel
{
    Refresh(response: GetProjectsResponse) {
        this.projects = [];
        new List(response.projects).OrderBy(p => p.name).ToArray()
            .forEach(p => this.projects.push(ProjectVm.FromApi(p)));
    }
    constructor(init?: Partial<ViewModel>) {
        Object.assign(this, init);
    }  
    projects: ProjectVm[] = [];
    requests: RequestManager = new RequestManager();
    isAddProjectModalVisible: boolean; 
}

export class ProjectVm  
{
    static FromApi(data: ProjectItem): ProjectVm
    {
        let result = new ProjectVm({
            id: data.id,
            comment: data.comment,
            name: data.name
        })
        return result;
    }

    constructor(init?: Partial<ProjectVm>) 
    {
        Object.assign(this, init);
        this.link = new Link({path: `/projects/${this.id}/testplans`})
    }

    id: number;
    link: Link;
    name: string;
    comment: string;
} 

