import { UserRole } from "src/app/api/reports/swagger";
import { NavigationLink } from "src/app/navigation/ViewModel";
import { UserVm, ViewModel } from "../viewmodel";

export class ViewModelTemplate {
    static Default(): ViewModel {
        let result = new ViewModel({
            projectId: 1,
            navigationLinks: [
                NavigationLink.Root(),
            ],
            users: [
                new UserVm(
                    {
                        id: 1,
                        name: "admin",
                        login: "admin",
                        mail: "admin@localhost", 
                        role: UserRole.Owner
                    }),
                new UserVm(
                    {
                        id: 2,
                        name: "viewer",
                        login: "viewer",
                        mail: "", 
                        isVirtual: true,
                        role: UserRole.TestManager
                    }),
                new UserVm(
                    {
                        id: 3,
                        name: "tester",
                        login: "tester",
                        mail: "", 
                        isVirtual: true,
                        role: UserRole.Guest
                    }),
                new UserVm(
                    {
                        id: 4,
                        name: "manager",
                        login: "manager",
                        mail: "", 
                        isVirtual: true,
                        role: UserRole.Tester
                    })
            ]
        })
        return result;
    }

    static ShowAddUserModal(): ViewModel {
        let result = this.Default();
        result.selectedUser = new UserVm({
            login: "log",
            name: "Иван Иванов",
            role: UserRole.TestManager,
            pass: "asm"
        })
        result.isAddUserModalVisible = true;
        return result;
    }
}
