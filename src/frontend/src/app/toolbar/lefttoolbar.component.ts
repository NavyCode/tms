import { Component, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
// import { ReportsApi } from '../api/reports/nswagclient';
import { Location } from '@angular/common';
import { ReportClients } from '../api/reports/ReportClientFactory';
import { IReportClient } from '../api/reports/swagger';
import { RequestManager } from '../api/reports/RequestManager';
import { Auth } from '../login/auth';
import { DocumentStyleFixer } from '../ext/documentFixer';
import { CookieService } from 'ngx-cookie';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'lefttoolbar',
  templateUrl: './lefttoolbar.component.html',
  styleUrls: ['./lefttoolbar.component.css']
})

export class LeftToolbarComponent {
  api: IReportClient;
  rm: RequestManager; 
  @Input('projectid') projectid: number;

  constructor(http: HttpClient
    , private location: Location
    , private route: ActivatedRoute
    , private cookieService: CookieService
    , private router: Router) {
    this.api = ReportClients.ReportClientFactory.Create(http, cookieService);
  }

  ngOnInit(): void {
    new DocumentStyleFixer(document);
  }
} 