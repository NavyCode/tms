import { Link } from "src/app/ext/links";
import { NavigationLink, ViewModel } from "../viewmodel";

export class ViewModelTemplate {
    static WithLiks(): ViewModel {
        return new ViewModel({
            links: [
                new NavigationLink({
                    id: 1,
                    name: 'Development',
                    url: new Link({ path: "/primecost" }),
                }),
                new NavigationLink({
                    id: 2,
                    name: 'TestPlans',
                    url: new Link({ path: "/expenses" }),
                }),
                new NavigationLink({
                    id: 3,
                    name: 'ALL Tests',
                    url: null,
                })
            ]
        })
    }
} 