import { Message } from "primeng/api";
import { RequestManager } from "../api/reports/RequestManager";



export class ViewModel {
  login: string;
  pass: string;
  isRegister: string;
  requests: RequestManager = new RequestManager();



  init() {

  }

  constructor(init?: Partial<ViewModel>) {
    Object.assign(this, init);
  }
}