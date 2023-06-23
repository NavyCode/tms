import { Message } from "primeng/api";

export class RequestManager {
    loadingVisible: boolean; 
    
    messages: Message[] = [];

    ShowLoader(): void
    {
        this.loadingVisible = true;
    }
    
    HideLoader(): void
    {
        this.loadingVisible = false;
    }

    NotifyError(err: any, message: string): void
    {
        this.HideLoader();
        let error:Error= err; 
        console.error(error);
        const noticeTime: number = 5000; 

        this.messages = [{severity:'error', summary:'', detail: message}]
    }

    NotifyInfo(message: string): void
    {
        this.HideLoader();
        const noticeTime: number = 5000; 
        this.messages = [{severity:'info', summary:'', detail: message}]
    }
}
