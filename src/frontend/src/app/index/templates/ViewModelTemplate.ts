import { ProjectVm, ViewModel } from "../viewmodel";

export class ViewModelTemplate {
    static WithProjects(): ViewModel {
        return new ViewModel({
            projects: [
                new ProjectVm(
                    {
                        id: 1,
                        name: "Dev",
                        comment: 'Разработка ПО'
                    }),
                new ProjectVm(
                    {
                        id: 1,
                        name: "Qa",
                        comment: 'Системное тестирование'
                    }),
                new ProjectVm(
                    {
                        id: 1,
                        name: "UAT",
                        comment: 'Выходной контроль'
                    }),
                new ProjectVm(
                    {
                        id: 1,
                        name: "IaC",
                        comment: 'Применение подхода Infrastructure as Code для создания сред разработки, тестирования, наладки и техподдержки'
                    })
                ]
        })
    }

    static ShowAddProjectModal(): ViewModel { 
        let result = this.WithProjects();
        result.isAddProjectModalVisible = true;
        return result;
    }
}
