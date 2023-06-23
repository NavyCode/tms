import { NavigationLink } from "src/app/navigation/ViewModel";
import { ViewModel } from "../viewmodel";

export class ViewModelTemplate {
    static WithSettings(): ViewModel {
        let result = new ViewModel({
            projectId: 1,
            navigationLinks: [
                NavigationLink.Root(),
                NavigationLink.Project(1, "Demo project"),
                NavigationLink.Current("Settings"),
            ],
        });
        return result;
    }
}
