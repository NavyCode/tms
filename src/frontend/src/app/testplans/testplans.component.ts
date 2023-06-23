
import { Component } from '@angular/core';
import { TestPlanVm, ViewModel } from './viewmodel';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportClients } from '../api/reports/ReportClientFactory';
import { Location } from '@angular/common';
import { environment } from 'src/environments/environment';
import { AddOrUpdatePlanRequest, IReportClient, PlanState } from '../api/reports/swagger';
import { CookieService } from 'ngx-cookie';
import { RequestManager } from '../api/reports/RequestManager';
import { DocumentStyleFixer } from '../ext/documentFixer';
import { Auth } from '../login/auth';
import { ViewModelTemplate } from './templates/ViewModelTemplate';
import { NavigationLink } from '../navigation/ViewModel';
import { MenuItem } from 'primeng/api';
import { UrlParser } from '../navigation/urlparsel';
import { List } from 'linqts';

@Component({
  selector: 'testplans',
  templateUrl: './testplans.component.html',
  styleUrls: ['./testplans.component.css']
})


export class TestPlansComponent {
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
    if (environment.design) {
      this.data = ViewModelTemplate.ShowEditProjectModal();
    }
    else {
      let auth = new Auth({ api: this.api, router: this.router });
      auth.Login().then(() => this.InitAfterAccess());
    }
    this.contextMenuItems = [
      { label: 'Edit', icon: 'bi bi-pencil', command: () => { this.showEditPlanWindow() } },
      { label: 'Delete', icon: 'bi bi-x-lg', command: () => { this.showDeletePlanWindow(); } }
    ];

  }

  showAddPlanWindow(): void {
    let newplan = new TestPlanVm({
      comment: null,
      name: "New testplan",
      state: PlanState.Active
    });
    this.data.selectedPlan = newplan;
    this.data.isAddPlanModalVisible = true;
  }
  async showEditPlanWindow(): Promise<void> {
    if (!environment.design) {
      let response = await this.api.projectUsers_GetUsers(this.data.projectId);
      this.data.users = new List(response.users).Select(p => p.identity).ToArray();

      this.data.isDeletePlanModalVisible = false;
    }
    this.data.isEditPlanModalVisible = true;
  }

  showDeletePlanWindow(): void {
    this.data.isDeletePlanModalVisible = true;
  }

  async editPlan(): Promise<void> {
    if (!environment.design) {
      let plan = this.data.selectedPlan;
      await this.api.plans_UpdatePlan(this.data.projectId, plan.id, new AddOrUpdatePlanRequest(
        {
          state: plan.state,
          description: plan.comment,
          name: plan.name,
          assignedTo: plan.assignedTo.id
        }
      ) );
      await this.LoadTestPlans();
    }
    this.data.isEditPlanModalVisible = false;
  }


  async deletePlan(): Promise<void> {
    this.data.isDeletePlanModalVisible = false;
    if (environment.design) {
      this.data.plans.splice(this.data.plans.indexOf(this.data.selectedPlan), 1);
      this.data.isDeletePlanModalVisible = false;
    }
    else {
      await this.api.plans_DeletePlan(this.data.projectId, this.data.selectedPlan.id );
      await this.LoadTestPlans();
    }
  }

  async InitAfterAccess(): Promise<void> {
    await this.LoadNavigationLinks();
    this.LoadTestPlans()
  }

  private async LoadNavigationLinks() {
    let parser = new UrlParser(this.route.snapshot.url);
    this.data.projectId = parser.GetProjectId();
    let projectInfo = await this.api.projects_GetProjectInfo(this.data.projectId);
    this.data.navigationLinks = [
      NavigationLink.Root(),
      NavigationLink.Project(projectInfo.id, projectInfo.name),
      NavigationLink.Current("Test plans"),
    ];
    console.info(this.data.navigationLinks.length);
  }

  async addPlan(): Promise<void> {
    this.data.isAddPlanModalVisible = false;
    if (environment.design) {
      this.data.AddPlan(this.data.selectedPlan);
    }
    else {
      let response = await this.api.plans_AddPlan(this.data.projectId, new AddOrUpdatePlanRequest({
        state: PlanState.Active,
        description: this.data.selectedPlan.comment,
        name: this.data.selectedPlan.name,
        assignedTo: null
      }));
      await this.LoadTestPlans();
    }
  }

  FocusNewPlanName(): void{
    document.getElementById("tbNewPlanName").focus();
  }
  async LoadTestPlans(): Promise<void> {
    this.requests.ShowLoader();
    try {
      let response = await this.api.plans_GetPlans(this.data.projectId);
      this.data.Refresh(response);
      this.requests.HideLoader();
    }
    catch (err) {
      this.requests.NotifyError(err, "Test plans load error");
    }
  }

} 