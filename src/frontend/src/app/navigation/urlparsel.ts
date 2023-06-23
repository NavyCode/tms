import { UrlSegment } from "@angular/router";


export class UrlParser
{
    constructor(url: UrlSegment[])
    {
        this.url = url;
    }
    url: UrlSegment[];
    GetProjectId(): number{
        return +this.url[1]
    } 
    
    GetPlanId(): number{
        return +this.url[3]
    } 
}
 

