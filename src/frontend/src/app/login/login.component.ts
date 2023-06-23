import { Component } from '@angular/core';
import { ViewModel } from './viewmodel';
import { HttpClient } from '@angular/common/http'; 
import { ActivatedRoute, Router } from '@angular/router';
import { ReportClients } from '../api/reports/ReportClientFactory';
import { Location } from '@angular/common';
import { environment } from 'src/environments/environment';
import { ViewModelTemplate } from './templates/viewmodel';
import { RequestManager } from '../api/reports/RequestManager';
import { IReportClient, RegisterOrUpdateUserRequest } from '../api/reports/swagger';
import { AppRouter } from '../navigation/navigationRouter';
import { CookieService } from 'ngx-cookie';

@Component({ 
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
 
export class LoginComponent
{
    api: IReportClient; 
    title: string;
    data: ViewModel;   
    
    constructor(http: HttpClient
      , private location: Location
      , private route: ActivatedRoute
      , private cookieService: CookieService
      , private router: Router) {  
      this.data = new ViewModel(); 
      if (environment.design)
        this.data = ViewModelTemplate.WithLoginError();
      this.api =  ReportClients.ReportClientFactory.Create(http, cookieService);

      console.info("constructor")
        this.route.queryParams.subscribe(params => {
          // todo
          // this.data.apiKey = params['token'];
          // if(this.data.apiKey != null)
          // {
          //   console.info("login by token");
          //   this.login();
          // }
      });
      
    }

    ngOnInit(): void 
    { 
      document.body.classList.add('bodyfix');
      document.getElementById("tbLogin").focus();
      document.getElementsByTagName('html')[0].classList.add('htmlfix');
      this.title = 'Авторизация';
    }  

    
  async login(): Promise<void> 
  {
    this.data.requests.ShowLoader();  
    try
    {
      if(environment.design)
      {
        this.data.requests.NotifyError(null, "Incorrect user or password"); 
      }
      else
      {
        let session = await this.api.users_Login(this.data.login, this.data.pass);
        this.cookieService.put(ReportClients.ClientAuthHttpClient.SessionHeaderKey, session)
        this.data.requests.HideLoader();
        new AppRouter(this.router).Navigation();
      }
    }
    catch(err)
    {
      this.data.requests.NotifyError(err, "Incorrect user or password"); 
    }
  } 

  async register(): Promise<void> 
  {
    this.data.requests.ShowLoader();  
    try
    {
      if(environment.design)
      {
        this.data.requests.messages = [{severity:'info', summary:'Confirmed', detail:'You have accepted'}]
      }
      else
      {
          let id = await this.api.users_RegisterUser(new RegisterOrUpdateUserRequest({
          login: this.data.login,
          pass: this.data.pass
        }));
          this.login();
        // this.cookieService.put(ReportClients.ClientAuthHttpClient.SessionHeaderKey, session)
        // this.requests.HideLoader();
        // new AppRouter(this.router).Navigation();
      }
    }
    catch(err)
    {
      this.data.requests.NotifyInfo("Сhoose another login"); 
    }
  } 
} 