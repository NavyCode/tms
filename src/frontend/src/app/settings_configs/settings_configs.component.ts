import { Component } from '@angular/core';
import { ConfigParam, ConfigVm, ViewModel } from './viewmodel';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportClients } from '../api/reports/ReportClientFactory';
import { Location } from '@angular/common';
import { environment } from 'src/environments/environment';
import { AddOrUpdateConfigsRequest, AddOrUpdateConfigVariable, IReportClient } from '../api/reports/swagger';
import { CookieService } from 'ngx-cookie';
import { RequestManager } from '../api/reports/RequestManager';
import { DocumentStyleFixer } from '../ext/documentFixer';
import { Auth } from '../login/auth';
import { ViewModelTemplate } from './templates/ViewModelTemplate';
import { MenuItem } from 'primeng/api';
import { List } from 'linqts';
import { UrlParser } from '../navigation/urlparsel';

@Component({
  selector: 'setting_configs',
  templateUrl: './settings_configs.component.html',
  styleUrls: ['./settings_configs.component.css']
})

export class SettingConfigsComponent {
  deleteParam() {
    this.data.selectedConfig.params.splice(this.data.selectedConfig.params.indexOf(this.data.selectedConfig.selectedParam), 1);
  }
  addParam() {
    if (this.data.configVarList.length > 0) {
      this.data.selectedConfig.params.push(new ConfigParam({
        var: this.data.configVarList[0],
        val: this.data.configVarList[0].values[0],
      }));
    }
  }
  contextMenuItems: MenuItem[];

  showDeleteConfigWindow() {
    this.data.isDeleteConfigModalVisible = !this.data.isDeleteConfigModalVisible;
  }
  showEditConfigWindow() {
    this.data.isAddOrEditConfigModalVisible = !this.data.isAddOrEditConfigModalVisible;
  }
  showAddConfigWindow() {
    this.data.selectedConfig = new ConfigVm({
      name: "New config",
      comment: "Comment",
    });
    this.data.isAddOrEditConfigModalVisible = !this.data.isAddOrEditConfigModalVisible;
  }
  async deleteConfig(): Promise<void> {
    if (environment.design) {
      this.data.configs.splice(this.data.configs.indexOf(this.data.selectedConfig), 1);
      this.data.isDeleteConfigModalVisible = false;
    }
    else
    {
      try
      {
        await this.api.testConfigs_DeleteConfig(this.data.projectId, this.data.selectedConfig.id );
        this.LoadSettings();
        this.data.isDeleteConfigModalVisible = false;
      }
      catch (err) {
        this.requests.NotifyError(err, "Delete config error");
      }
    }
  }
  async addConfig(): Promise<void> {
    if (environment.design) {
      this.data.configs.push(this.data.selectedConfig);
      this.data.isAddOrEditConfigModalVisible = false;
    }
    else
    {
      try
      {
        let request = this.CreateUpdateRequest(this.data.selectedConfig);
        await this.api.testConfigs_AddConfig(this.data.projectId, request);
        this.LoadSettings();
        this.data.isAddOrEditConfigModalVisible = false;
      }
      catch (err) {
        this.requests.NotifyError(err, "Add config error");
      }
    }
  }

  private CreateUpdateRequest(selected: ConfigVm) {
    let request = new AddOrUpdateConfigsRequest({
      isDefault: selected.isDefault,
      comment: selected.comment,
      name: selected.name,
      variables: []
    });
    selected.params.forEach(p => {
      request.variables.push(new AddOrUpdateConfigVariable({
        id: p.id,
        valueId: p.val.id,
        variableId: p.var.id
      }));
    });
    return request;
  }

  async updateConfig(): Promise<void> {
    if (environment.design) {
      this.data.isAddOrEditConfigModalVisible = false;
    }
    else
    {
      try
      {
        let request = this.CreateUpdateRequest(this.data.selectedConfig);
        await this.api.testConfigs_UpdateConfig(this.data.projectId, this.data.selectedConfig.id, request );
        this.LoadSettings();
        this.data.isAddOrEditConfigModalVisible = false;
      }
      catch (err) {
        this.requests.NotifyError(err, "Add config error");
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
      this.data = ViewModelTemplate.ShowEditConfig();
    }
    else {
      let auth = new Auth({ api: this.api, router: this.router });
      auth.Login().then(() => this.InitAfterAccess());
    }

    this.contextMenuItems = [
      { label: 'Edit', icon: 'bi bi-pencil', command: () => { this.showEditConfigWindow() }, },
      { label: 'Delete', icon: 'bi bi-x-lg', command: () => { this.showDeleteConfigWindow(); } }
    ];
  }

  InitAfterAccess(): any {
    this.PaserUrl();
    this.LoadSettings();
  }

  async LoadSettings(): Promise<void> {
    let response = await this.api.testConfigs_GetConfigs(this.data.projectId);
    let variables = await this.api.testConfigs_GetConfigsVars(this.data.projectId);
    this.data.Refresh(response, variables);
  }

  PaserUrl() {
    let parser = new UrlParser(this.route.snapshot.url);
    this.data.projectId = parser.GetProjectId();
  }
} 