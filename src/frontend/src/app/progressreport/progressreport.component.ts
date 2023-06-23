
import { Component } from '@angular/core';
import { TestSuiteTreeVm, ViewModel } from './viewmodel';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportClients } from '../api/reports/ReportClientFactory';
import { Location } from '@angular/common';
import { environment } from 'src/environments/environment';
import { IReportClient, Outcome } from '../api/reports/swagger';
import { CookieService } from 'ngx-cookie';
import { RequestManager } from '../api/reports/RequestManager';
import { DocumentStyleFixer } from '../ext/documentFixer';
import { Auth } from '../login/auth';
import { ViewModelTemplate } from './templates/ViewModelTemplate'; 
import {  ChartConfiguration, ChartData } from 'chart.js';
import { UrlParser } from '../navigation/urlparsel';
import { NavigationLink } from '../navigation/ViewModel';
import { List } from 'linqts';

@Component({
  selector: 'progressreport',
  templateUrl: './progressreport.component.html',
  styleUrls: ['./progressreport.component.css']
})

export class ProgressReportComponent {

   
  public pieChartOptions: ChartConfiguration<'doughnut'>['options'] = {
    responsive: true,
    cutout: '75%',
    plugins: {
      legend: {
        display: false,
        position: 'bottom',
        
      },
      filler:{
        propagate: true,
        drawTime: "beforeDatasetsDraw"
      },

    }
  };

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

    this.route.queryParams.subscribe(params => {
        this.data.planId = params['plan'];
        this.LoadPlanReport();
    }); 
  }

  
  SetResultFilter(value: Outcome) : void
  {
      if(value == Outcome.Passed)
        this.data.filter.passed = !this.data.filter.passed;
      else if(value == Outcome.Failed)
        this.data.filter.failed = !this.data.filter.failed;
      else if(value == Outcome.Blocked)
        this.data.filter.blocked = !this.data.filter.blocked;
      else if(value == Outcome.Skipped)
        this.data.filter.skipped = !this.data.filter.skipped;
      else if(value == Outcome.Unknow)
        this.data.filter.other = !this.data.filter.other;
     this.data.RefhreshAllByFilter();
  }

  
  SetFilter(value: string) : void
  {
      if(value == "SubSuites")
        this.data.filter.withSubSuites = !this.data.filter.withSubSuites;
     else if(value == "AlwaysExpand")
        this.data.filter.isAlwaysExpand = !this.data.filter.isAlwaysExpand;

     this.data.RefhreshAllByFilter();

  }

  public get TestOutcome(): typeof Outcome {
    return Outcome; 
  }

  ngOnInit(): void {
    new DocumentStyleFixer(document);
    if (environment.design)
    {
      this.data = ViewModelTemplate.WithPointTabSelected();
    }
    else
    {
      let auth = new Auth({ api: this.api, router: this.router });
      auth.Login();
    }
  } 
 

  
  private async LoadNavigationLinks() {
    let parser = new UrlParser(this.route.snapshot.url);
    this.data.projectId = parser.GetProjectId(); 
    let projectInfo = await this.api.projects_GetProjectInfo(this.data.projectId);
    let planInfo = await this.api.plans_GetPlanInfo(this.data.projectId, this.data.planId );
    this.data.navigationLinks = [
      NavigationLink.Root(),
      NavigationLink.Project(projectInfo.id, projectInfo.name),
      NavigationLink.TestPlans(projectInfo.id, "Test plans"),
      NavigationLink.TestPlan(projectInfo.id, this.data.planId, planInfo.name),
      NavigationLink.Current("Progress report"),  
    ];
    console.info(this.data.navigationLinks.length);
  }
  
  async LoadPlanReport(): Promise<void> {
    if(!environment.design)
    {    
      await this.LoadNavigationLinks();
      try { 
        let suites = await this.api.plan_GetSuitesTree(this.data.projectId, this.data.planId );
        let points = await this.api.testPoints_GetSuiteTestPoints(this.data.projectId, new List(suites.suites).First(p => p.parentId == null).id
          , true, this.data.projectId.toString() );

        this.data.Refresh(suites, points);
      }
      catch (err) {
        this.requests.NotifyError(err, "Error report load");
      }
    }
  }

  
 
} 