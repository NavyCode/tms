
import { Component } from '@angular/core';
import { AddTestCasesModalSearchResultVm, AssignConfigModalSearchResultVm, AssignConfigModalVm, AssignTesterSearchResultVm, TestSuiteVm, ViewModel } from './viewmodel';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportClients } from '../api/reports/ReportClientFactory';
import { Location } from '@angular/common';
import { environment } from 'src/environments/environment';
import { AddOrUpdateSuiteRequest, AddTestCaseToSuiteRequest, DeleteTestCaseFromSuiteRequest, IReportClient, Outcome, SetDescriptionRequest, SetOutcomeRequest, SetTestCaseConfigRequest, SetTestSuiteConfigRequest, SetTesterRequest } from '../api/reports/swagger';
import { CookieService } from 'ngx-cookie';
import { RequestManager } from '../api/reports/RequestManager';
import { DocumentStyleFixer } from '../ext/documentFixer';
import { Auth } from '../login/auth';
import { ViewModelTemplate } from './templates/ViewModelTemplate';
import { MenuItem } from 'primeng/api';
import { UrlParser } from '../navigation/urlparsel';
import { NavigationLink } from '../navigation/ViewModel';
import { List } from 'linqts';

@Component({
  selector: 'testplans',
  templateUrl: './testplan.component.html',
  styleUrls: ['./testplan.component.css']
})

 
export class TestPlanComponent {
  async searchTestCases() {
    let testCases = await this.api.testCases_SearchTestCases(this.data.projectId, this.data.addTestCasesModal.searchText);
    this.data.addTestCasesModal.result = [];
    testCases.forEach(p => this.data.addTestCasesModal.result.push(AddTestCasesModalSearchResultVm.FromApi(p)));
  }
  async testCaseCreated(id: number) {
    try {
      await this.api.testSuite_AddTestCaseSuite(this.data.projectId, this.data.selectedSuite.id, new AddTestCaseToSuiteRequest({
        testCasesIds: [id]
      }), this.data.planId.toString());
      await this.data.RefreshSelectedSuite();
      this.data.isCreateTestCaseModal = false;

    }
    catch (err) {
      this.requests.NotifyError(err, "Add test case error");
    }
  }
  contextMenuDefineItems: MenuItem[];
  contextMenuExecuteItems: MenuItem[];
  contextMenuSuiteItems: MenuItem[];

  UpdateTestsWithChildrenSuites() {
    this.data.isShowTestsWithChildrenSuite = !this.data.isShowTestsWithChildrenSuite;
  }

  collapseSelectedSuite() {
    this.setSuiteExpandStateRecoursive(this.data.selectedSuite, false);
  }
  expandSelectedSuite() {
    this.setSuiteExpandStateRecoursive(this.data.selectedSuite, true);
  }

  setSuiteExpandStateRecoursive(selectedSuite: TestSuiteVm, value: boolean): void {
    selectedSuite.expanded = value;
    selectedSuite.children.forEach(p => this.setSuiteExpandStateRecoursive(p, value));
  }
  async addTestCases() {
    if(environment.design)
    {
      this.data.isAddTestCasesModal = false;
    }
    else
    { 
      this.data.isAddTestCasesModal = false;
      let ids = new List(this.data.addTestCasesModal.result).Where(p => p.selected).Select(p => p.id).ToArray();
      if (ids.length == 0)
        return;
      await this.api.testSuite_AddTestCaseSuite(this.data.projectId, this.data.selectedSuite.id,
        new AddTestCaseToSuiteRequest({testCasesIds: ids}), this.data.planId.toString());
        this.data.RefreshSelectedSuite();
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
    this.data.onUpdateUrl.asObservable().subscribe(value => {
      this.updateUrl();
    })
    this.api = ReportClients.ReportClientFactory.Create(http, cookieService);
    this.data.api = this.api;
    this.requests = new RequestManager();
  }

  
  updateUrl(): void{
console.info("updateUrl");
    
    let activeTab = "";
    if(this.data.tabSelectedIndex == 0)
      activeTab = "define";
    else if(this.data.tabSelectedIndex == 1)
      activeTab = "execute";
    else if(this.data.tabSelectedIndex == 2)
      activeTab = "result";
    let suiteId = "";
    if(this.data.selectedSuite?.id > 0)
      suiteId = this.data.selectedSuite.id.toString()
    this.router.navigate([], { 
      
      relativeTo: this.route,
      queryParams: {
        suietId: suiteId,
        tab: activeTab
      },
      queryParamsHandling: 'merge'
     });
  }

  urlSuiteId: number;
  urlTab: string; 
  ApplyUrlParams() {
    console.info(this.route.snapshot.queryParams);
    console.info("tab " + this.urlTab);
    if(this.urlSuiteId > 0)
      this.data.SelectSuite(this.urlSuiteId);
    if(this.urlTab == "execute")
       this.data.tabSelectedIndex = 1
    else if(this.urlTab == "result")
      this.data.tabSelectedIndex = 2
  }

  ngOnInit(): void {
    new DocumentStyleFixer(document);
    if (environment.design) {
      this.data = ViewModelTemplate.ShowAssignTesterModal();
    }
    else {
      let auth = new Auth({ api: this.api, router: this.router });
      auth.Login().then(() => this.InitAfterAccess());
    }

    this.contextMenuDefineItems = [
      { label: 'Edit', icon: 'bi bi-pencil', command: () => { this.showEditTestCase() } },
      { label: 'Assing config', icon: 'bi bi-list-check', command: () => { this.showTestCaseConfig() } },
      { label: 'Remove', icon: 'bi bi-x-lg', command: () => { this.showRemoveTestCase() } }
    ];

    this.contextMenuExecuteItems = [
      { label: 'Pass', icon: 'bi bi-check-lg', command: () => { this.SetStateSelectedTests(Outcome.Passed); } },
      { label: 'Fail', icon: 'bi bi-x-octagon', command: () => { this.SetStateSelectedTests(Outcome.Failed); } },
      { label: 'Skip', icon: 'bi bi-skip-end', command: () => { this.SetStateSelectedTests(Outcome.Skipped); } },
      { label: 'Сomment', icon: 'bi bi-people', command: () => { this.showSetComment() } },
      { label: 'Assing tester', icon: 'bi bi-people', command: () => { this.showAssignTester() } },
    ];

    this.contextMenuSuiteItems = [
      { label: 'New Suite', icon: 'bi bi-pencil', command: () => this.showAddSuite() },
      { label: 'Assing config', icon: 'bi bi-list-check', command: () => this.showEditSuiteConfigs() },
      { label: 'Edit', icon: 'bi bi-pencil', command: () => this.showEditSuiteName() },
      { label: 'Delete', icon: 'bi bi-x-lg', command: () => this.showDeleteSuite() }
    ];



  }
  showSetComment() {
    this.data.isShowSetCommentModal = true;
  } 

  async SetComment() { 
    if (environment.design) {
      this.data.isShowSetCommentModal = false; 
    }
    else {
      await this.api.testPoints_SetComment(this.data.projectId, new SetDescriptionRequest({
        description: this.data.testPointCommentModal,
        testPointIds: new List(this.data.executeSelectedTests()).Select(p => p.pointId).ToArray()
      }), this.data.planId.toString(), this.data.projectId.toString()); 
      this.data.isShowSetCommentModal = false;
      this.data.testPointCommentModal = "";
    }
  }


  async showAssignTester() {
    this.data.isAssignTesterModal = true;
    if (!environment.design) {
      this.data.assignTesterModal.result = [];
      this.data.assignTesterModal.selected = null;
      let users = await this.api.testPoints_GetUserListForAssign(this.data.projectId);

      let listResult: AssignTesterSearchResultVm[] = [];
      users.forEach(p => listResult.push(new AssignTesterSearchResultVm({
        id: p.id,
        name: p.name
      })));
      this.data.assignTesterModal.result = listResult;

      console.info(this.data.assignTesterModal.result);

    }
  }

  async AssignUserSelected() {
    if (this.data.assignTesterModal.selected == null)
      return;
    if (environment.design) {
      this.data.isAssignTesterModal = false;
      this.data.executeSelectedTests().forEach(p => p.tester = this.data.assignTesterModal.selected.name);
    }
    else {
      await this.api.testPoints_SetTester(this.data.projectId, new SetTesterRequest({
        testerId: this.data.assignTesterModal.selected.id,
        testPointIds: new List(this.data.executeSelectedTests()).Select(p => p.pointId).ToArray()
      }), this.data.projectId.toString());
      this.data.RefreshSelectedSuite();
      this.data.isAssignTesterModal = false;
    }
  }

  async showTestCaseConfig() {
    if (environment.design) {
      console.info("id" + this.data.selectedSuite.id);
      this.data.isAssignConfigModal = true;
    }
    else {
      let allConfigs = await this.api.testSuite_GetConfigsForAssign(this.data.projectId);
      this.data.assignConfigModal = new AssignConfigModalVm({
        result: new List(allConfigs.configs).Select(p => AssignConfigModalSearchResultVm.FromApi(p)).ToArray()
      });
      let suiteConfigs = await this.api.testSuite_GetSuiteConfigs(this.data.projectId, this.data.selectedSuite.id, this.data.planId.toString());
      this.data.assignConfigModal.result.forEach(p => {
        if (suiteConfigs.includes(p.id))
          p.selected = true;
      })
      this.data.isAssignConfigModal = true;
    }
  }
  showRemoveTestCase() {
    this.data.isDeleteTestCaseModalVisible = true;
  }
  showEditTestCase() { 
    let tcs = this.data.defineSelectedTests();
    if (tcs.length != 1)
    {
      this.requests.NotifyInfo("Select single test case");
      return;
    }
    this.data.editTestCaseId = tcs[0].testCaseId;
    this.data.isEditTestCaseModal = true;
  }

  async deleteTestCases() {
    if (environment.design) {
      this.data.defineSelectedTests().forEach(p => {
        this.data.defineTests.splice(this.data.defineTests.indexOf(p, 1));
      })
      this.data.isDeleteTestCaseModalVisible = false;
    }
    else {
      try {
        this.api.testSuite_DeleteTestCaseFromSuite(this.data.projectId, new DeleteTestCaseFromSuiteRequest({
          suiteTestCasesIds: new List(this.data.defineSelectedTests()).Select(p => p.id).ToArray(),
        }), this.data.planId.toLocaleString(), this.data.selectedSuite.id.toString())
        this.data.isDeleteTestCaseModalVisible = false;
        this.data.RefreshSelectedSuite();
      }
      catch (err) {
        this.requests.NotifyError(err, "Delete tests error");
      }
    }
  }

  async showEditSuiteConfigs() {
    if (environment.design) {
      console.info("id" + this.data.selectedSuite.id);
      this.data.assignConfigModal.suiteId = this.data.selectedSuite.id;
      this.data.isAssignConfigModal = true;
    }
    else {
      let allConfigs = await this.api.testSuite_GetConfigsForAssign(this.data.projectId);
      this.data.assignConfigModal = new AssignConfigModalVm({
        result: new List(allConfigs.configs).Select(p => AssignConfigModalSearchResultVm.FromApi(p)).ToArray(),
        suiteId: this.data.selectedSuite.id
      });
      let suiteConfigs = await this.api.testSuite_GetSuiteConfigs(this.data.projectId, this.data.selectedSuite.id, this.data.planId.toString());
      console.info(suiteConfigs);

      this.data.assignConfigModal.result.forEach(p => {
        if (suiteConfigs.includes(p.id))
          p.selected = true;
      })
      this.data.isAssignConfigModal = true;
    }
  }

  showEditSuiteName(): void {
    this.data.editSuite = this.data.selectedSuite;
    this.data.isAddOrEditSuiteModalVisible = true;
  }

  async setSuiteConfigs() {
    if (environment.design) {
      this.data.isAssignConfigModal = false;
    }
    else {
      try {
        let modal = this.data.assignConfigModal;
        await this.api.testSuite_SetSuiteConfig(this.data.projectId, modal.suiteId, new SetTestSuiteConfigRequest({
          testConfigIds: new List(modal.result).Where(p => p.selected).Select(p => p.id).ToArray()
        }), this.data.planId.toString());
        this.data.RefreshSelectedSuite();
        this.data.isAssignConfigModal = false;
      }
      catch (err) {
        this.requests.NotifyError(err, "Set suite config error");
      }
    }
  }


  async setTestConfigs() {
    if (environment.design) {
      this.data.isAssignConfigModal = false;
    }
    else {
      try {
        let modal = this.data.assignConfigModal;
        await this.api.testSuite_SetTestCaseConfig(this.data.projectId, new SetTestCaseConfigRequest({
          suiteTestCaseIds: new List(this.data.defineSelectedTests()).Select(p => p.id).ToArray(),
          testConfigIds: new List(modal.result).Where(p => p.selected).Select(p => p.id).ToArray()
        }), this.data.planId.toString(), this.data.selectedSuite.id.toString());
        this.data.RefreshSelectedSuite();
        this.data.isAssignConfigModal = false;
      }
      catch (err) {
        this.requests.NotifyError(err, "Set test config error");
      }
    }
  }

  showDeleteSuite(): void {
    this.data.editSuite = this.data.selectedSuite;
    this.data.isDeleteSuiteModalVisible = true;
  }

  async deleteSuite() {
    let parent = this.data.selectedSuite.parent;

    if (environment.design) {
      parent.children.splice(parent.children.indexOf(this.data.selectedSuite, 1));
      this.data.selectedSuite = parent;
      this.data.isDeleteSuiteModalVisible = false;
    }
    else {
      try {
        await this.api.testSuites_DeleteSuite(this.data.projectId, this.data.selectedSuite.id, this.data.planId.toString());
        parent.children.splice(parent.children.indexOf(this.data.selectedSuite, 1));
        this.data.selectedSuite = parent;
        this.data.isDeleteSuiteModalVisible = false;
      }
      catch (err) {
        this.requests.NotifyError(err, "Delete suite error");
      }
    }
  }

  showAddSuite(): void {
    this.data.editSuite = new TestSuiteVm({
      name: "Suite",
      parentId: this.data.selectedSuite.id,
    });
    this.data.selectedSuite.children.push(this.data.editSuite);
    this.data.selectedSuite.expanded = true;
    this.data.isAddOrEditSuiteModalVisible = true;
  }


  async SetStateSelectedTests(value: Outcome) {
    if (environment.design) {
      this.data.executeSelectedTests().forEach(p => p.outcome = value);
    }
    else {
      try {
        await this.api.testPoints_SetOutcome(this.data.projectId, new SetOutcomeRequest({
          outcome: value,
          testPointIds: new List(this.data.executeSelectedTests()).Select(p => p.pointId).ToArray()
        }), this.data.planId.toString(), this.data.selectedSuite.id.toString())
        await this.data.RefreshSelectedSuite();
      }
      catch (err) {
        this.requests.NotifyError(err, "Set test result error");
      }
    }
  }

  get OutCome(): typeof Outcome {
    return Outcome;
  }

  showAddExistTestCaseWindow(): void {
    this.data.isAddTestCasesModal = true;
  }

  showAddTestCaseWindow(): void {
    this.data.isCreateTestCaseModal = true;
  }

  async InitAfterAccess() {
    this.urlSuiteId = this.route.snapshot.queryParams['suietId'];
    this.urlTab = this.route.snapshot.queryParams['tab']; 
    await this.LoadNavigationLinks();
    await this.LoadTestSuitesTree();
    this.ApplyUrlParams();  
  }

  private async LoadNavigationLinks() {
    let parser = new UrlParser(this.route.snapshot.url);
    this.data.projectId = parser.GetProjectId();
    this.data.planId = parser.GetPlanId();
    let projectInfo = await this.api.projects_GetProjectInfo(this.data.projectId);
    let planInfo = await this.api.plans_GetPlanInfo(this.data.projectId, this.data.planId);
    this.data.navigationLinks = [
      NavigationLink.Root(),
      NavigationLink.Project(projectInfo.id, projectInfo.name),
      NavigationLink.TestPlans(projectInfo.id, "Test plans"),
      NavigationLink.Current(planInfo.name),
    ];
    console.info(this.data.navigationLinks.length);
  }




  async addSuite() {
    if (environment.design) {
      this.data.isAddOrEditSuiteModalVisible = false;
    }
    else {
      try {
        await this.api.testSuites_AddSuite(this.data.projectId, this.data.planId, new AddOrUpdateSuiteRequest({
          parentId: this.data.editSuite.parentId,
          name: this.data.editSuite.name
        }))
        this.data.RefreshSelectedSuite();
        this.data.isAddOrEditSuiteModalVisible = false;
      }
      catch (err) {
        this.requests.NotifyError(err, "Add suite error");
      }
    }
  }

  async updateSuite() {
    if (environment.design) {
      this.data.isAddOrEditSuiteModalVisible = false;
    }
    else {
      try {
        let response = this.api.testSuites_UpdateSuite(this.data.projectId, this.data.editSuite.id, new AddOrUpdateSuiteRequest({
          parentId: this.data.editSuite.parentId,
          name: this.data.editSuite.name
        }), this.data.planId.toString())
        this.data.isAddOrEditSuiteModalVisible = false;
      }
      catch (err) {
        this.requests.NotifyError(err, "Update suite error");
      }
    }
  }

  async LoadTestSuitesTree(): Promise<void> {
    try {
      let response = await this.api.plan_GetSuitesTree(this.data.projectId, this.data.planId);
      this.data.Refresh(response);
      this.data.selectedSuite = this.data.rootSuite;
    }
    catch (err) {
      this.requests.NotifyError(err, "Error testplan load");
    }
  }

} 