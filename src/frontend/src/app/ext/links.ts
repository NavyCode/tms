export class Link
{
  path: string;
  isRemote: boolean = false;
  params: {[k: string]: string}; 
  public constructor(init?: Partial<Link>) {
      Object.assign(this, init);
  }

   contains(route: string): boolean
   {
     return !this.isRemote && this.path.toLowerCase() == route.toLowerCase()
   }
}