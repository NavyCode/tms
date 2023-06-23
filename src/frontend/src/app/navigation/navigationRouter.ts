import { Router } from "@angular/router";

export class AppRouter
{
    constructor(private router: Router)
    {
        
    }

    Login(): void
    {
        this.router.navigate(["/login"]);
    }

    Navigation(): void
    {
        this.router.navigate([""]);
    }
}