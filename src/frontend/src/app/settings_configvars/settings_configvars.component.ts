
import { Component } from '@angular/core';
import { ConfigVarVal, ConfigVarVm, ViewModel } from './viewmodel';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportClients } from '../api/reports/ReportClientFactory';
import { Location } from '@angular/common';
import { environment } from 'src/environments/environment';
import { AddOrUpdateConfigVarRequest, AddOrUpdateConfigVarValue, IReportClient } from '../api/reports/swagger';
import { CookieService } from 'ngx-cookie';
import { RequestManager } from '../api/reports/RequestManager';
import { DocumentStyleFixer } from '../ext/documentFixer';
import { Auth } from '../login/auth';
import { ViewModelTemplate } from './templates/ViewModelTemplate';
import { MenuItem } from 'primeng/api';
import { UrlParser } from '../navigation/urlparsel';

@Component({
  selector: 'settings_configvars',
  templateUrl: './settings_configvars.component.html',
  styleUrls: ['./settings_configvars.component.css']
})

export class SettingsConfigVarsComponent {
  async deleteValue(): Promise<void> {
    this.data.selectedVar.values.splice(this.data.selectedVar.values.indexOf(this.data.selectedVar.selectedValue), 1);
  }
  async addValue(): Promise<void> {
    this.data.selectedVar.values.push(new ConfigVarVal({
      name: "New value"
    }));
  }


  contextMenuItems: MenuItem[];


  private CreateUpdateRequest(selected: ConfigVarVm) {
    let request = new AddOrUpdateConfigVarRequest({
      comment: selected.comment,
      name: selected.name,
      values: []
    });
    selected.values.forEach(p => {
      request.values.push(new AddOrUpdateConfigVarValue({
        id: p.id,
        value: p.name
      }));
    });
    return request;
  }

  showDeleteVarWindow() {
    this.data.isDeleteVarModalVisible = !this.data.isDeleteVarModalVisible;
  }
  showEditVarWindow() {
    this.data.isAddOrEditVarModalVisible = !this.data.isAddOrEditVarModalVisible;
  }
  showAddVarWindow() {
    this.data.selectedVar = new ConfigVarVm({
      name: "New variable",
      comment: "Comment",
    });
    this.data.isAddOrEditVarModalVisible = !this.data.isAddOrEditVarModalVisible;
  }
  async deleteVar(): Promise<void> {
    if (environment.design) {
      this.data.vars.splice(this.data.vars.indexOf(this.data.selectedVar), 1);
    }
    else {
      try {
        await this.api.testConfigs_DeleteConfigVariable(this.data.projectId, this.data.selectedVar.id );
        this.LoadSettings();
        this.data.isDeleteVarModalVisible = false;
      }
      catch (err) {
        this.requests.NotifyError(err, "Delete variable error");
      }
    }
    this.data.isDeleteVarModalVisible = false;
  }
  async addVar(): Promise<void> {
    if (environment.design) {
      this.data.vars.push(this.data.selectedVar);
      this.data.isAddOrEditVarModalVisible = false;
    }
    else {
      let request = this.CreateUpdateRequest(this.data.selectedVar);
      try {
        await this.api.testConfigs_AddConfigVariable(this.data.projectId, request);
        this.LoadSettings();
        this.data.isAddOrEditVarModalVisible = false;
      }
      catch (err) {
        this.requests.NotifyError(err, "Add variable error");
      }
    }
  }

  async updateVar(): Promise<void> {
    if (!environment.design) {
      let request = this.CreateUpdateRequest(this.data.selectedVar);
      try {
        await this.api.testConfigs_UpdateConfigVariable(this.data.projectId, this.data.selectedVar.id, request );
        this.LoadSettings();
        this.data.isAddOrEditVarModalVisible = false;
      }
      catch (err) {
        this.requests.NotifyError(err, "Update variable error");
      }
    }
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
    if (environment.design) {
      this.data = ViewModelTemplate.ShowEditVar();
    }
    else {
      let auth = new Auth({ api: this.api, router: this.router });
      auth.Login().then(() => this.InitAfterAccess());
    }

    this.contextMenuItems = [
      { label: 'Edit', icon: 'bi bi-pencil', command: () => { this.showEditVarWindow() } },
      { label: 'Delete', icon: 'bi bi-x-lg', command: () => { this.showDeleteVarWindow(); } }
    ];
  } 

  async InitAfterAccess(): Promise<void> {
    this.PaserUrl();
    await this.LoadSettings(); 
  } 

  async LoadSettings(): Promise<void> {
    let response = await this.api.testConfigs_GetConfigsVars(this.data.projectId);
    this.data.Refresh(response);
  }

  PaserUrl() {
    let parser = new UrlParser(this.route.snapshot.url);
    this.data.projectId = parser.GetProjectId();
  }
} 