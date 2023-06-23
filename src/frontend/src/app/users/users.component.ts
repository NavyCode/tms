
import { Component } from '@angular/core';
import { UserVm, ViewModel } from './viewmodel';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportClients } from '../api/reports/ReportClientFactory';
import { Location } from '@angular/common';
import { environment } from 'src/environments/environment';
import { AddOrUpdateVirtualUserRequest, IReportClient, UserRole } from '../api/reports/swagger';
import { CookieService } from 'ngx-cookie';
import { RequestManager } from '../api/reports/RequestManager';
import { DocumentStyleFixer } from '../ext/documentFixer';
import { Auth } from '../login/auth';
import { ViewModelTemplate } from './templates/ViewModelTemplate';
import { NavigationLink } from '../navigation/ViewModel';
import { MenuItem } from 'primeng/api'; 
import { UrlParser } from '../navigation/urlparsel';

@Component({
  selector: 'users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
}) 

export class UsersComponent {
  api: IReportClient; 
  data: ViewModel;
  requests: RequestManager;
  contextMenuItems: MenuItem[];

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
      this.data = ViewModelTemplate.Default();
    }
    else
    {
      let auth = new Auth({ api: this.api, router: this.router });
      auth.Login().then(() => this.InitAfterAccess());
    }
    this.contextMenuItems = [
        {label: 'Edit', icon: 'bi bi-pencil', command: () => { this.showEditUserWindow() }},
        {label: 'Delete', icon: 'bi bi-x-lg', command: () => { this.showDeleteUserWindow();}}
    ];

  }

  showAddUserWindow(): void{
    let newplan = new UserVm({
      id: 0,
      name: "New User",
      login: "newuser",
      role: UserRole.Guest,
      mail: "", 
    });
    this.data.selectedUser = newplan;
    this.data.isAddUserModalVisible = true;
  }

  public get userRole(): typeof UserRole {
    return UserRole; 
  }
 
  showInviteUserWindow(): void{
    this.data.isInviteUserModalVisible = true;
  } 

  showEditUserWindow(): void{
    this.data.isEditUserModalVisible = true;
  }

  showDeleteUserWindow(): void{
    this.data.isDeleteUserModalVisible = true;
  }
   
 async editUser() {
    
    if(environment.design)
    {
      this.data.isEditUserModalVisible = false;
    }
    else
    {
      let request = this.CreateUpdateUserRequest()
      await this.api.projectUsers_UpdateVirtualUser(this.data.projectId, this.data.selectedUser.id, request );
      this.data.isEditUserModalVisible = false;
    }
  }
  

  async deleteUser() {
    if(environment.design)
    {
      this.data.users.splice(this.data.users.indexOf(this.data.selectedUser), 1);
      this.data.isDeleteUserModalVisible = false;
    }
    else
    {
      await this.api.projectUsers_DeleteUser(this.data.projectId, this.data.selectedUser.id );
      this.LoadUsers();
    }
  }
 
  async InitAfterAccess(): Promise<void> { 
    await this.LoadNavigationLinks();
    await this.LoadUsers()
  }  

  async LoadUsers(): Promise<void> {
    try { 
      let response = await this.api.projectUsers_GetUsers(this.data.projectId);
      this.data.Refresh(response);
      this.requests.HideLoader();
    }
    catch (err) {
      this.requests.NotifyError(err, "Test plans load error");
    }
  }
  
  private async LoadNavigationLinks() {
    let parser = new UrlParser(this.route.snapshot.url);
    this.data.projectId = parser.GetProjectId();
    let projectInfo = await this.api.projects_GetProjectInfo(this.data.projectId);
    this.data.navigationLinks = [
      NavigationLink.Root(),
      NavigationLink.Project(projectInfo.id, projectInfo.name),
      NavigationLink.Current("Users"),
    ];
    console.info(this.data.navigationLinks.length);
  }

  
  async addUser()
  {
    if(environment.design)
    {
      this.data.users.push(this.data.selectedUser);
      this.data.isAddUserModalVisible = false;
    }
    else
    {
      let request = this.CreateUpdateUserRequest()
      await this.api.projectUsers_AddVirtualUser(this.data.projectId, request);
      this.data.isEditUserModalVisible = false;
    }
  }
 
  private CreateUpdateUserRequest() {
    return new AddOrUpdateVirtualUserRequest({
      role: this.data.selectedUser.role,
      login: this.data.selectedUser.login,
      mail: this.data.selectedUser.mail,
      name: this.data.selectedUser.name,
      pass: this.data.selectedUser.pass
    });
  }
} 