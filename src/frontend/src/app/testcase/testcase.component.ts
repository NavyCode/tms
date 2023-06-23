
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { TestStepVm, ViewModel } from './viewmodel';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportClients } from '../api/reports/ReportClientFactory';
import { Location } from '@angular/common';
import { environment } from 'src/environments/environment';
import { AddOrUpdateTestCaseRequest, AddOrUpdateTestStep, AutomationStatus, IReportClient, WiState } from '../api/reports/swagger';
import { CookieService } from 'ngx-cookie';
import { RequestManager } from '../api/reports/RequestManager';
import { DocumentStyleFixer } from '../ext/documentFixer';
import { Auth } from '../login/auth';
import { ViewModelTemplate } from './templates/ViewModelTemplate';
import { List } from 'linqts';

@Component({
  selector: 'testcase',
  templateUrl: './testcase.component.html',
  styleUrls: ['./testcase.component.css']
})

export class TestCaseComponent implements OnChanges{
  api: IReportClient; 
  data: ViewModel;
  requests: RequestManager;
  @Input('id') id: number; 
  @Input('projectId') projectId: number; 
  @Input('isModal') isModal: boolean= false; 
  @Input('isReadOnly') isReadOnly: boolean = false; 
  @Output() created = new EventEmitter<number>();

  constructor(http: HttpClient
    , private location: Location
    , private route: ActivatedRoute
    , private cookieService: CookieService
    , private router: Router) {
    this.data = new ViewModel();
    this.api = ReportClients.ReportClientFactory.Create(http, cookieService);
    this.requests = new RequestManager();
  }
  ngOnChanges(changes: SimpleChanges): void {
    this.data.id = this.id;
    this.data.projectId = this.projectId;
    this.LoadTestCase();
  }

  ngOnInit(): void {
    new DocumentStyleFixer(document);
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

  InitAfterAccess(): any { 
    this.LoadTestCase()
  }  
  
  addStep(): void
  {
     this.data.steps.push(new TestStepVm({
      number: this.data.steps.length +1
     }));
  }

  async Create()
  {
    if(environment.design)
    {

    }
    else
    {
      try
      {
        let request = this.CreateRequest();
        let id = await this.api.testCases_AddTest(this.data.projectId, request);
        this.data.id = id;
        this.created.emit(this.data.id);
      }
      catch (err) {
        this.requests.NotifyError(err, "Create testcase error");
      }
    }
  }

async Refresh()
{
  if(environment.design)
  {

  }
  else
  {
    await this.LoadTestCase();
  }
}

  async Save()
  {
    if(environment.design)
    {

    }
    else
    {
      try
      {
      let request = this.CreateRequest();
      await this.api.testCases_UpdateTest(this.data.projectId, this.data.id, request );
    }
    catch (err) {
      this.requests.NotifyError(err, "Save testcase error");
    }
    }
  } 

  private CreateRequest() {
    let request = new AddOrUpdateTestCaseRequest({
      state: this.data.state,
      title: this.data.title,
      postcondition: this.data.postconditions,
      precondition: this.data.preconditions,
      steps: [],
      assignedTo: 1,
      automationStatus: AutomationStatus.Manual,
      priority: 1,
    });
    this.data.steps.forEach(p => {
      request.steps.push(new AddOrUpdateTestStep({
        order: p.number,
        action: p.action,
        id: p.id,
        result: p.result
      }));
    });
    return request;
  }

  moveStepsUp(): void
  {
    let count = this.data.selectedSteps.length;
    let nextNumber = new List(this.data.selectedSteps).OrderBy(p => p.number).First().number - 1;
    let i = 1;
    console.info(nextNumber);
    new List(this.data.steps).Where(p => p.number <= nextNumber)
    .OrderByDescending(p => p.number).ForEach(p => 
      {
        if(i++ == 1)
          p.number += count
        else
          p.number -= count;
      });
     new List(this.data.selectedSteps).OrderBy(p => p.number)
      .ForEach(p => p.number -= count);
     this.data.ReorderSteps();
  }

  
  moveStepsDown(): void
  {
    let count = this.data.selectedSteps.length;
    let nextNumber = new List(this.data.selectedSteps).OrderBy(p => p.number).Last().number + 1;
    let i = 1;
    new List(this.data.steps).Where(p => p.number >= nextNumber)
      .OrderBy(p => p.number).ForEach(p => 
      {
        if(i++ == 1)
          p.number -= count
        else
          p.number += count;
      });
     new List(this.data.selectedSteps).OrderBy(p => p.number)
      .ForEach(p => p.number += count);
     this.data.ReorderSteps();
  }

  deleteSteps(): void
  {
     this.data.selectedSteps.forEach(p => { 
      this.data.steps.splice(this.data.steps.indexOf(p), 1);
     });
     this.data.selectedSteps = [];
     this.data.ReorderSteps();
  }

  async LoadTestCase(): Promise<void> { 
    if(this.data.id == null)
      return;
    try { 
      let tc = await this.api.testCases_GetTestCase(this.data.projectId, this.data.id );
      if(tc != null)
        this.data.Refresh(tc);
    }
    catch (err) {
      this.requests.NotifyError(err, "Load testcase error");
    }
  }
 
} 