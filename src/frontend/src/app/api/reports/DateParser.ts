import moment from 'moment';
import { DateExt } from 'src/app/ext/dateext';
export class DateParser {
    static FromApi(date: string): Date {
        if (date == null)
            return null;
        let myMoment: moment.Moment = moment(date, "YYYY-MM-DD");
        let result = myMoment.toDate();
        if (isNaN(result.getTime()))
            return DateExt.NowRoundByDay();
        return result;
    }
    static ToApi(date: Date): string {
        if (date == null)
            return "";
        let myMoment: moment.Moment = moment(date);
        return myMoment.format("YYYY-MM-DD");
    }
}
