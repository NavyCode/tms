import { NavigationLink } from "../navigation/ViewModel";

export class ViewModel {
    isConfigsTabSelected: boolean;
    isConfigVarsTabSelected: boolean;
    navigationLinks: NavigationLink[] = [];
    constructor(init?: Partial<ViewModel>) {
        Object.assign(this, init);
    }
    projectId: number;
}