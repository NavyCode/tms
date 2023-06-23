
import { Component } from '@angular/core';
import { ProjectVm, ViewModel } from './viewmodel';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportClients } from '../api/reports/ReportClientFactory';
import { Location } from '@angular/common';
import { environment } from 'src/environments/environment';
import { AddOrUpdateProjectRequest, IReportClient } from '../api/reports/swagger';
import { CookieService } from 'ngx-cookie';
import { RequestManager } from '../api/reports/RequestManager';
import { DocumentStyleFixer } from '../ext/documentFixer';
import { Auth } from '../login/auth';
import { ViewModelTemplate } from './templates/ViewModelTemplate';
import { NavigationLink } from '../navigation/ViewModel';

@Component({
  selector: '',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css']
})

export class IndexComponent {
  api: IReportClient; 
  data: ViewModel;
  navigationLinks: NavigationLink[] = [];

  constructor(http: HttpClient
    , private location: Location
    , private route: ActivatedRoute
    , private cookieService: CookieService
    , private router: Router) {
    this.data = new ViewModel(); 


    this.api = ReportClients.ReportClientFactory.Create(http, cookieService); 
  }

  ngOnInit(): void {
    new DocumentStyleFixer(document);
    this.navigationLinks = [
      NavigationLink.Root(),
      NavigationLink.Text(2, "Projects")
    ]

    if (environment.design)
    {
      this.data = ViewModelTemplate.WithProjects();
    }
    else
    {
      let auth = new Auth({ api: this.api, router: this.router });
      auth.Login().then(() => this.InitAfterAccess());
    }
  }

  showAddProjectWindow(): void{
    this.data.isAddProjectModalVisible = true; 
  }

  FocusNewProjectName(): void{
    document.getElementById("tbNewProjectName").focus();
  }

  InitAfterAccess(): any {
    this.LoadProjects()
  } 

  newProjectName: string = null;
  newProjectComment: string = null;
  
  async addProject(): Promise<void>
  {
    this.data.isAddProjectModalVisible = false;
    if(environment.design)
    {
      this.data.projects.push(new ProjectVm({
        id: 5,
        comment: this.newProjectComment,
        name: this.newProjectName
      }));
    }
    else
    {
      try {  
        await this.api.projects_AddProject(new AddOrUpdateProjectRequest({
          name: this.newProjectName,
          description: this.newProjectComment
        }));
        await this.LoadProjects();
      }
      catch (err) {
        this.data.requests.NotifyError(err, "Add project error");
      }
    }
  }

  async LoadProjects(): Promise<void> {
    this.data.requests.ShowLoader();
    try {  
      let response = await this.api.projects_GetProjects();
      this.data.Refresh(response);
      this.data.requests.HideLoader();
    }
    catch (err) {
      this.data.requests.NotifyError(err, "Load error");
    }
  }
 
} 