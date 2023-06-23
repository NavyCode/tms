 
import { IReportClient } from '../api/reports/swagger';
import { Router } from '@angular/router';
import { AppRouter } from '../navigation/navigationRouter';

export class Auth
{  
    api: IReportClient; 
    router: Router;

    constructor(init?: Partial<Auth>) {
        Object.assign(this, init);
    }  
 
    async Login() {
      try
      {
        await this.api.users_UserInfo(); 
      }
      catch
      {
        new AppRouter(this.router).Login();
      }
  }
}