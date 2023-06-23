import { PlanState, UserIdentity, UserItem, UserRole } from "src/app/api/reports/swagger";
import { NavigationLink } from "src/app/navigation/ViewModel";
import { TestPlanVm, ViewModel } from "../viewmodel";

export class ViewModelTemplate {

    static WithProjects(): ViewModel {
        let users = [
            new UserIdentity({ id: 1,  name: "Иванов Иван Иванович"}),
            new UserIdentity({ id: 2,  name: "Петров Петр Петрович"}),
            new UserIdentity({ id: 3,  name: "Сидоров Сидор Сидорович"})
        ];
        let result = new ViewModel({ 
            projectId: 1,
            navigationLinks: [
                NavigationLink.Root(),
                NavigationLink.Project(1, "Demo project"),
                NavigationLink.Current("Test plans"),
            ],
            plans: [
                new TestPlanVm(
                    {
                        id: 1,
                        name: "All Tests",
                        comment: 'Актуальный список тестов',
                        assignedTo: users[1],
                        state: PlanState.Active
                    }),
                new TestPlanVm(
                    {
                        id: 2,
                        name: "1.1.0",
                        comment: 'UAT для Заказчика 1',
                        assignedTo: users[1],
                        state: PlanState.Active
                    }),
                    new TestPlanVm(
                        {
                            id: 3,
                            name: "1.2.0",
                            comment: 'Системное тестирование',
                            assignedTo: users[1],
                            state: PlanState.Closed
                        })
                ],
                users:[
                    new UserIdentity({ id: 1, name: "Иванов Иван Иванович" }),
                    new UserIdentity({ id: 2, name: "Петров Петр Петрович" }),
                    new UserIdentity({ id: 3, name: "Сидоров Сидор Сидорович" })
                ]
        })
        result.plans.forEach(p => p.link = result.GetTestPlanLink(p.id));
        return result;
    }

    static ShowAddProjectModal(): ViewModel { 
        let result = this.WithProjects();
        result.selectedPlan = new TestPlanVm({

        })
        result.isAddPlanModalVisible = true;
        return result;
    }

    
    static ShowEditProjectModal(): ViewModel { 
        let result = this.WithProjects();
        result.selectedPlan = result.plans[0];
        result.isEditPlanModalVisible = true;
        return result;
    }
}
