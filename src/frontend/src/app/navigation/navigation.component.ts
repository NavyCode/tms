import { Component, Input, OnChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http'; 
import { ActivatedRoute, Router } from '@angular/router';
// import { ReportsApi } from '../api/reports/nswagclient';
import { Location } from '@angular/common';
import { NavigationLink, ViewModel } from './ViewModel';
import { ReportClients } from '../api/reports/ReportClientFactory';
import { IReportClient } from '../api/reports/swagger';
import { RequestManager } from '../api/reports/RequestManager';
import { Auth } from '../login/auth';
import { DocumentStyleFixer } from '../ext/documentFixer';
import { CookieService } from 'ngx-cookie';
import { environment } from 'src/environments/environment';

@Component({ 
    selector: 'navigation',
    templateUrl: './navigation.component.html',
    styleUrls: ['./navigation.component.css']
})
 
export class NavigationComponent implements OnChanges
{   
    data: ViewModel = new ViewModel();
    api: IReportClient;
    rm: RequestManager;  
    @Input('links') links: NavigationLink[]; 
 
    constructor(http: HttpClient
      , private location: Location
      , private route: ActivatedRoute
      , private cookieService: CookieService
      , private router: Router) {  
        this.api =  ReportClients.ReportClientFactory.Create(http, cookieService);
    } 
      
    ngOnInit(): void 
    {  
      new DocumentStyleFixer(document);
      this.data.links = this.links;
    } 

    ngOnChanges(changes) {
      if(this.links != null)
        this.data.links = this.links;
    }

  isNavbarCollapsed: boolean = true;
  
  isChatEnable: boolean = environment.production;
} 