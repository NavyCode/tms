import { NavigationLink } from "src/app/navigation/ViewModel";
import { TestPointVm, TestPlanVm, TestSuiteFlatVm, TestSuiteTreeVm, ViewModel } from "../viewmodel";
import { Outcome } from "src/app/api/reports/swagger";

export class ViewModelTemplate {
    static WithProjects(): ViewModel {
        let result = new ViewModel({
            projectId: 1,
            navigationLinks: [
                NavigationLink.Root(),
                NavigationLink.Project(1, "Demo project"),
                NavigationLink.Current("Report: All Tests"),
            ],
            testplanList: [
                new TestPlanVm({id: 1, name: "AllTests"})
            ], 
            suites: [
                new TestSuiteFlatVm({ id: 1, parentId: null, name: "All Tests", blocked: 10, failed: 3, skipped: 2, passed: 100, unknow: 3}),

                new TestSuiteFlatVm({ id: 2, parentId: 1, name: "Gmail", failed: 3, passed: 100}),

                new TestSuiteFlatVm({ id: 3, parentId: 2, name: "Авторизация", failed: 3, passed: 100}),
                new TestSuiteFlatVm({ id: 4, parentId: 2, name: "Отправка сообщений", skipped: 2, failed: 3, passed: 2}),

                new TestSuiteFlatVm({ id: 5, parentId: 4, name: "Из черновиков", failed: 3, passed: 100}),
                new TestSuiteFlatVm({ id: 6, parentId: 4, name: "Из шаблонов", failed: 3, passed: 100}), 

                new TestSuiteFlatVm({ id: 7, parentId: 1, name: "Drive", failed: 3, passed: 100}),
            ], 
            points: [
                new TestPointVm({ id: 1, isSelected: true, name: "Авторизация", suiteId: 4, order: 1, configuration: "Windows", outcome: Outcome.Failed, tester: "Иван Иван"
                    , description: "Test method Dapsy.Tests.DapsyTests.MinputWithMistrustInFormulaArgument threw exception: Monitel.Qa.Core.Assertion.AssertException: Assert.AreEqual failed. Expected:<2>. Actual:<1>. 0x70000002 2023-02-06T00:01:39.8490000Z. . TimeOut 5 sec ---> Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException: Assert.AreEqual failed. Expected:<2>. Actual:<1>. 0x70000002 2023-02-06T00:01:39.8490000Z"
                    , stacktrace: 'at AlarmService.Tests.RtdbAPI.<>c__DisplayClass3_0.<AssertValue>b__0() in e:Sources\\App.Tests\\Deploy\\Sources\\App\\API\\API.cs:line 36'+
                    '\r\nat Monitel.Qa.Core.Assertion.Asserts.<>c__DisplayClass29_0.<WaitNoError>b__0()'+
                    '\r\n--- End of inner exception stack trace ---'+
                    '\r\nat Monitel.Qa.Core.Assertion.Asserts.WaitNoError(Action action, TimeSpan timeout, String message, Nullable 1 interval, Func 1 comment)'+
                    '\r\nat AlarmService.Tests.RtdbAPI.AssertValue in e:\\a3\\b4_3_4\\_work\\3\\s\\TestBox\\Tests\\Sources\\App.Tests\\Deploy\\Sources\\App\\API\\API.cs:line 32'
                }),
                new TestPointVm({ id: 2, isSelected: true, name: "Отправка сообщения / Passed", suiteId: 3, order: 2, configuration: "Linux", outcome: Outcome.Passed }),
                new TestPointVm({ id: 3, isSelected: true, name: "Отправка сообщения / Blocked", suiteId: 3, order: 2, configuration: "Linux", outcome: Outcome.Blocked }),
                new TestPointVm({ id: 4, isSelected: true, name: "Отправка сообщения / NotApplicable", suiteId: 3, order: 2, configuration: "Linux", outcome: Outcome.Skipped }),
                new TestPointVm({ id: 5, isSelected: true, name: "Отправка сообщения / Paused", suiteId: 3, order: 2, configuration: "Linux", outcome: Outcome.Passed }),
                new TestPointVm({ id: 6, isSelected: true, name: "Отправка сообщения / Other", suiteId: 3, order: 2, configuration: "Linux", outcome: Outcome.Unknow }),
                new TestPointVm({ id: 2, isSelected: true, name: "Очень длинное сообщение фыв фдыов рдфывр фырв дфоыр вдофыв дофырв длофыр вдофрывд рф рвфыв ", suiteId: 3, order: 2, configuration: "Очень длинная конфигурация фыовр фылорв флоырв р", outcome: Outcome.Passed }),
                new TestPointVm({ id: 2, isSelected: true, name: "ОченьДлинноеСообщениеВ1СтрочкуОченьДлинноеСообщениеВ1СтрочкуОченьДлинноеСообщениеВ1СтрочкуОченьДлинноеСообщениеВ1Строчку", suiteId: 3, order: 2, configuration: "ОченьДлиннаяКонфигурацияВ1СтрочкуОченьДлиннаяКонфигурацияВ1Строчку", outcome: Outcome.Passed }),

            ]
        });
        result.RefhreshAllByFilter();
        result.selectedSuite = result.suitesTree[0];  
        return result;
    }

    static WithFilter(): ViewModel {
        let result = this.WithProjects();
        result.filter.passed = true;
        result.filter.failed = true;
        result.filter.blocked = false;
        result.filter.other = false;
        result.filter.skipped = false;
        result.filter.withSubSuites = true;
        return  result;
    }

    static WithChartSelected(): ViewModel
    {
        let result = this.WithFilter();
        result.isChartTabSelected = true;
        result.isPointTabSelected = false;
        return result;
    }
    

    static WithPointTabSelected(): ViewModel
    {
        let result = this.WithFilter();
        result.isChartTabSelected = false;
        result.isPointTabSelected = true;
        return result;
    }
}
  