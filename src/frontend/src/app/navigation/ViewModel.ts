import { Link } from '../ext/links';


export class ViewModel {
  routePath: string;

  constructor(init?: Partial<ViewModel>) {
    Object.assign(this, init);
  }

  links: NavigationLink[] = []
}

export class NavigationLink {

  static Text(id: number, text: string): NavigationLink
  {
    return  new NavigationLink({id: id, name: text});
  }

  constructor(init?: Partial<NavigationLink>) {
    Object.assign(this, init);
  }
  id: number;
  name: string;
  url: Link;

  static Root(): NavigationLink {
    return new NavigationLink({id: 1, name: "Navy", url: new Link({path: "/"})})
  } 

  static Project(id: number, name: string): NavigationLink {
    return new NavigationLink({id: 2, name: name, url: new Link({path: `/projects/${id}/testplans/`})})
  }   
  
  static TestPlans(projectId: number, name: string): NavigationLink {
    return new NavigationLink({id: 2, name: name, url: new Link({path: `/projects/${projectId}/testplans/`})})
  }   
  
  static TestPlan(projectId: number, planId: number, planName: string): NavigationLink {
    return new NavigationLink({id: 2, name: planName, url: new Link({path: `/projects/${projectId}/testplans/${planId}`})})
  }  

  static ProgressReportPlan(projectId: number, planId: number): NavigationLink {
    return new NavigationLink({id: 2, name: null, url: new Link({path: `/projects/${projectId}/analytics/progressreport`,
  params:{
    'plan': planId?.toString()
  }})})
  }  

  
  static Current(name: string): NavigationLink {
    return new NavigationLink({id: 9999, name: name})
  } 
}

