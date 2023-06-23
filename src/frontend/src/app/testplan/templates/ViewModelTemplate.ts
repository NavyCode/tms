import { NavigationLink } from "src/app/navigation/ViewModel";
import { AddTestCasesModalSearchResultVm, AssignConfigModalSearchResultVm, AssignTesterSearchResultVm, DefineTestCaseVm, ExecuteTestCaseVm, TestSuiteVm, ViewModel } from "../viewmodel";
import { Outcome, WiState } from "src/app/api/reports/swagger";

export class ViewModelTemplate {
    static Create(): ViewModel {
        let result = new ViewModel({ 
            projectId: 1,
            planId: 1,
            navigationLinks: [
                NavigationLink.Root(),
                NavigationLink.Project(1, "Demo project"),
                NavigationLink.Current("Test plan: All Tests"),
            ],
            testPlanName: "All Tests",
            rootSuite: new TestSuiteVm({
                name: "All Tests",
                id: 321321,
                expanded: true,
                children: [
                    new TestSuiteVm({
                        name: "Gmail",
                        expanded: true,
                        children: [
                            new TestSuiteVm({
                                name: "Авторизация"
                            }),
                            new TestSuiteVm({
                                name: "Отправка сообщений",
                                expanded: true,
                                children: [
                                    new TestSuiteVm({
                                        name: "Из черновиков"
                                    }),
                                    new TestSuiteVm({
                                        name: "Из шаблонов"
                                    })
                                ]
                            })
                        ]
                    }),
                    new TestSuiteVm({
                        name: "Drive"
                    })
                ]
            }),
            defineTests: [
                new DefineTestCaseVm({ id: 101111, selected: true, name: "Авторизация", order: 1, state: WiState.Design, priority: 1,  }),
                new DefineTestCaseVm({ id: 101112, selected: true, name: "Отправка сообщения", order: 2, state: WiState.Ready, priority: 2 }),
                new DefineTestCaseVm({ id: 101113, selected: true, name: "Отправка сообщения", order: 3, state: WiState.Closed, priority: 3 }),
                new DefineTestCaseVm({ id: 101114, selected: true, name: "Прием сообщений", order: 4, state: WiState.Closed, priority: 4 }),
            ],
            executeTests:[
                new ExecuteTestCaseVm({ pointId: 1, testcaseId: 101111, selected: true, name: "Авторизация", order: 1, configuration: "Windows 10", priority: 1, outcome: Outcome.Passed, tester: "Иван Иванов"  }),
                new ExecuteTestCaseVm({ pointId: 2, testcaseId: 101112, selected: true, name: "Отправка сообщения", order: 2, configuration: "Windows 10", priority: 1, outcome: Outcome.Failed, tester: "Иван Иванов"  })
            ]

        });
        result.assignConfigModal.searchText = "Windows",
        result.assignConfigModal.result = [
            new AssignConfigModalSearchResultVm({id: 1, selected: true, name: "Windows 10 x64", params: "OS: Windows 10, Bit operation system: x64"}),
            new AssignConfigModalSearchResultVm({id: 2, name: "Windows 7 x64", params: "OS: Windows 7, Bit operation system: x64"}),
        ]
        result.suitesTree = [result.rootSuite];
        result.selectedSuite = result.suitesTree[0];
        
        return result;
    }
    
 
    static ShowExecuteTab(): ViewModel { 
        let result = this.Create();
        result.tabSelectedIndex = 1;
        return result;
    }

    
    static ShowResultsTab(): ViewModel { 
        let result = this.Create();
        result.tabSelectedIndex = 2;
        return result;
    }
 
    static ShowAddExistTestCaseModal(): ViewModel { 
        let result = this.Create();
        result.isAddTestCasesModal = true;
        result.addTestCasesModal.searchText = "Авторизация",
        result.addTestCasesModal.result = [
            new AddTestCasesModalSearchResultVm({id: 1, name: "Авторизация", priority: 1}),
            new AddTestCasesModalSearchResultVm({id: 2, name: "Отправка сообщения", priority: 1}),
            new AddTestCasesModalSearchResultVm({id: 3, name: "Отправка сообщения", priority: 2})
        ]
        return result;
    }

    static ShowAssignConfigModal(): ViewModel { 
        let result = this.Create();
        result.isAssignConfigModal = true; 
        return result;
    }

    
    static ShowAssignTesterModal(): ViewModel { 
        let result = this.Create();
        result.isAssignTesterModal = true;
        result.tabSelectedIndex = 1;
        result.assignTesterModal.searchText = "Иван Иванов",
        result.assignTesterModal.result = [
            new AssignTesterSearchResultVm({id: 1, name: "Иванов Иван Иванович"}),
            new AssignTesterSearchResultVm({id: 2, name: "Петров Петр Петрович"}),
        ]
        result.assignTesterModal.selected = result.assignTesterModal.result[0];
        return result;
    }


    static ShowEditSuite(): ViewModel { 
        let result = this.Create();
        result.editSuite = result.selectedSuite;
        result.isAddOrEditSuiteModalVisible = true;
        return result;
    }

    
    static ShowEditSuiteConfig(): ViewModel { 
        let result = this.Create();
        result.isAssignConfigModal = true;
        return result;
    }
}
