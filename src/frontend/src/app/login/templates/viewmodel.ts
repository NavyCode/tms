import { ViewModel } from "../viewmodel";

export class ViewModelTemplate
{
    static WithApiKey(): ViewModel{
        return new ViewModel({
            login: "login", 
            pass: "pass", 
        })
    }

    static WithLoginError(): ViewModel{
        let result = new ViewModel({
            login: "login", 
            pass: "pass",
        })
        
        result.requests.messages = [{severity:'info', summary:'Confirmed', detail:'You have accepted'}]
        return result;
    }
}
