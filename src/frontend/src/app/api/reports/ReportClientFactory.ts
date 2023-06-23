import { HttpClient } from '@angular/common/http';
import { CookieService } from 'ngx-cookie';
import { environment } from 'src/environments/environment';
import { IReportClient, ReportClient } from './swagger';


export namespace ReportClients 
{   
    export class ReportClientFactory  
    { 
        static Create(http: HttpClient, cookieService: CookieService): IReportClient
        {  
            let url = window.location.protocol + "//" + window.location.hostname + environment.apiUri
            console.info(url);
            let authHttpClient = new ClientAuthHttpClient(cookieService);
            let result = new ReportClient(url, authHttpClient); 
            return result;
        } 
    } 
    
    export class ClientAuthHttpClient
    {
        private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    
        constructor(private cookieService: CookieService) {
            this.http = window as any;
        }

        static SessionHeaderKey: string = "session";
        async fetch(url: RequestInfo, init?: RequestInit): Promise<Response>
        {
            init.headers[ClientAuthHttpClient.SessionHeaderKey]=this.cookieService.get(ClientAuthHttpClient.SessionHeaderKey);
            return await this.http.fetch(url, init);
        }
    }
}
