
import { Component } from '@angular/core';
import { ViewModel } from './viewmodel'; 
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportClients } from '../api/reports/ReportClientFactory';
import { Location } from '@angular/common';
import { environment } from 'src/environments/environment';
import { IReportClient } from '../api/reports/swagger';
import { CookieService } from 'ngx-cookie';
import { RequestManager } from '../api/reports/RequestManager';
import { DocumentStyleFixer } from '../ext/documentFixer';
import { Auth } from '../login/auth';
import { ViewModelTemplate } from './templates/ViewModelTemplate';
import { UrlParser } from '../navigation/urlparsel';
import { NavigationLink } from '../navigation/ViewModel';

@Component({
  selector: 'settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})

export class SettingsComponent {
addTestCases() {
throw new Error('Method not implemented.');
}
  api: IReportClient; 
  data: ViewModel;
  requests: RequestManager;

  constructor(http: HttpClient
    , private location: Location
    , private route: ActivatedRoute
    , private cookieService: CookieService
    , private router: Router) {
    this.data = new ViewModel();
    this.api = ReportClients.ReportClientFactory.Create(http, cookieService);
    this.requests = new RequestManager();
  }

  ngOnInit(): void {
    new DocumentStyleFixer(document);
    if (environment.design)
    {
      this.data = ViewModelTemplate.WithSettings();
    }
    else
    {
      let auth = new Auth({ api: this.api, router: this.router });
      auth.Login().then(() => this.InitAfterAccess());
    }
  }
 
  async InitAfterAccess(): Promise<void> { 
    await this.LoadNavigationLinks();
  }   
  
  private async LoadNavigationLinks(): Promise<void>  {
    let parser = new UrlParser(this.route.snapshot.url);
    this.data.projectId = parser.GetProjectId();
    let projectInfo = await this.api.projects_GetProjectInfo(this.data.projectId);
    this.data.navigationLinks = [
      NavigationLink.Root(),
      NavigationLink.Project(projectInfo.id, projectInfo.name),
      NavigationLink.Current("Settings"),
    ];
    console.info(this.data.navigationLinks.length);
  }
} 