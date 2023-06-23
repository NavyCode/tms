 


export class DateExt
{
  static GetTimeZone(): number {
      return new Date().getTimezoneOffset()
  }
  static NowRoundByDay(): Date {
    let timeStamp = Date.now(); 
    timeStamp -= timeStamp % (24 * 60 * 60 * 1000);
    timeStamp += new Date().getTimezoneOffset() * 60 * 1000;
    return new Date(timeStamp);
  }
    static GetInterval(from: Date, to: Date): Date[]
    {
        let result: Date[] = [];
        let current = new Date(from);
        while(current <= to)
        {
            let copiedDate = new Date(current.getTime()); 
            result.push(copiedDate);
            current.setDate(current.getDate() + 1);
        }
        return result;
    }
    
    static Locale: string = "ru";

    static Equals(from: Date, to: Date): boolean
    {
        if(from == null && to == null)
            return true;
        if(from != null || to != null)
            return false;
        return from.getTime() == to.getTime();
    } 

    static GetNumberOfWeekInMonth(date: Date): number { 
        let result = Math.ceil(date.getDate() / 7);
        if(result == 0)
            result = 1;
        return result;
    }

    static getWeek(date: Date): number { 
        let onejan = new Date(1970, 0, 1);
        return Math.ceil((((date.getTime() - onejan.getTime()) / 86400000) + onejan.getDay()) / 7);
    }
 

    
    static IsLess(from: Date, to: Date): boolean
    {
        return from.getTime() < to.getTime();
    }

 
    static RoundDateByMonth(date: Date): Date {
        return new Date(date.getFullYear(), date.getMonth(), 1);
    }

    static ToLocalTime(date: Date): Date {
        return new Date(date.getTime() - date.getTimezoneOffset() * 3600 * 100);
    }

    static ToSystemTime(date: Date): Date {
        return new Date(date.getTime() + date.getTimezoneOffset() * 3600 * 100);
    }
} 