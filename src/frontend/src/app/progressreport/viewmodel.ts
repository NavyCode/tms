
import { List } from 'linqts';
import { Subject } from 'rxjs';
import { NavigationLink } from '../navigation/ViewModel';
import { GetSuitesTreeResponse, GetSuiteTestPointsResponse, Outcome } from '../api/reports/swagger';
import { ChartData } from 'chart.js';

export class ViewModel {
    Refresh(suites: GetSuitesTreeResponse, points: GetSuiteTestPointsResponse) {
        console.info("Suites " + suites.suites.length);
        console.info("Points " + points.points.length);

        suites.suites.forEach(s => this.suites.push(new TestSuiteFlatVm({
            id: s.id,
            parentId: s.parentId,
            name: s.name
        })));
        points.points.forEach(p => {
            this.points.push(new TestPointVm({
                id: p.id,
                testCaseId: p.testCaseId,
                configuration: p.configuration,
                name: p.name,
                outcome: p.outcome,
                order: p.order,
                suiteId: p.testSuiteId,
                tester: p.tester,
                description: p.description
            }))
        })
        this.RefhreshAllByFilter();
        this.selectedSuite = this.suitesTree[0]; 
        
    }
    planId: number;
    RefhreshAllByFilter() {
        this.CalcSuiteStats();
        this.RefreshTestList();
        this.RefreshSuitesTree();
        this.RefreshCharts();
    }
    RefreshCharts() {
        this.charts = [];
        let selectedSuitesIds = new List<number>(); 
        this.GetSelectedSuites(selectedSuitesIds, this.selectedSuite); 
        let suitePoints: TestPointVm[] = [];
        this.points.forEach(p => {
            if (selectedSuitesIds.Contains(p.suiteId))
                suitePoints.push(p);
        });
        let g = new List(suitePoints).GroupBy(p => p.configuration);
        for (let key in g) { 
            let chart = new TestSuiteChart(g[key])
            this.charts.push(chart)
        }
    }
    CalcSuiteStats() {
        this.suites.forEach(s => {
            s.blocked = 0;
            s.failed = 0;
            s.passed = 0;
            s.skipped = 0;
            s.unknow = 0;

            let tests = new List(this.points).Where(p => p.suiteId == s.id).ToList();
            if (tests.Any()) {
                s.blocked = tests.Count(p => p.outcome == Outcome.Blocked);
                s.failed = tests.Count(p => p.outcome == Outcome.Failed);
                s.passed = tests.Count(p => p.outcome == Outcome.Passed);
                s.skipped = tests.Count(p => p.outcome == Outcome.Skipped);
                s.unknow = tests.Count(p => p.outcome == Outcome.Unknow || p.outcome == Outcome.Planed);
            }
        })

        if (this.filter.withSubSuites) {
            let fastSuites = new List(this.suites).ToDictionary(p => p.id);
            this.suites.forEach(s => {
                let currentSuite = s;
                while (currentSuite != null) {
                    let parent = fastSuites[currentSuite.parentId];

                    if (parent != null) {
                        parent.blocked += s.blocked;
                        parent.failed += s.failed;
                        parent.passed += s.passed;
                        parent.skipped += s.skipped;
                        parent.unknow += s.unknow;
                    }
                    currentSuite = parent;
                }
            })
        }
    }

    private RefreshTestList() {
        let selectedSuitesIds = new List<number>(); 
        this.GetSelectedSuites(selectedSuitesIds, this.selectedSuite); 
        this.visiblePoints = [];
        this.points.forEach(p => {
            if (selectedSuitesIds.Count() > 0 && !selectedSuitesIds.Contains(p.suiteId))
                return;
            if (this.filter.blocked == false && p.outcome == Outcome.Blocked)
                return;
            if (this.filter.failed == false && p.outcome == Outcome.Failed)
                return;
            if (this.filter.skipped == false && p.outcome == Outcome.Skipped)
                return;
            if (this.filter.other == false && (p.outcome == Outcome.Unknow || p.outcome == Outcome.Planed))
                return;
            if (this.filter.passed == false && p.outcome == Outcome.Passed)
                return;
            this.visiblePoints.push(p);
        });
        this.selectedPoint = this.visiblePoints[0];
    }


    GetSelectedSuites(result: List<number>, suite: TestSuiteTreeVm) {
        if (suite != null) {
            result.Add(suite.id);
            if (this.filter.withSubSuites)
                suite.children.forEach(p => this.GetSelectedSuites(result, p));
        }
    }

    RefreshSuitesTree() {
        let selectedId = this.selectedSuite?.id;
        let expandedIds = new List(this.suitesFlatList).Where(p => p.expanded).Select(p => p.id);

        let visibleSuites: TestSuiteFlatVm[] = [];
        this.suites.forEach(p => {
            let visible = false;
            if (this.filter.blocked == true && p.blocked > 0)
                visible = true;
            if (this.filter.failed == true && p.failed > 0)
                visible = true;
            if (this.filter.skipped == true && p.skipped > 0)
                visible = true;
            if (this.filter.other == true && p.unknow > 0)
                visible = true;
            if (this.filter.passed == true && p.passed > 0)
                visible = true;
            if (visible == true)
                visibleSuites.push(p);
        });


        this.suitesFlatList = [];
        let root: TestSuiteTreeVm;

        visibleSuites.forEach(s => {
            let currentSuite = s;
            let prevChildSuite: TestSuiteTreeVm;
            while (currentSuite != null) {
                let treeItem = this.suitesFlatList.find(p => p.id == currentSuite.id);
                if (treeItem == null) {
                    treeItem = new TestSuiteTreeVm(currentSuite);
                    if (this.filter.isAlwaysExpand || expandedIds.Contains(treeItem.id))
                        treeItem.expanded = true;
                    this.suitesFlatList.push(treeItem);
                }
                if (prevChildSuite != null) {
                    if (treeItem.children.indexOf(prevChildSuite) < 0)
                        treeItem.children.push(prevChildSuite);
                }
                if (selectedId != null && selectedId == treeItem.id)
                    this.selectedSuite = treeItem;
                if (currentSuite.parentId == null)
                    root = treeItem;

                prevChildSuite = treeItem;
                currentSuite = this.suites.find(p => p.id == currentSuite.parentId);
            }
        });


        this.suitesTree = [];
        if (root != null)
            this.suitesTree = [root];
    }
    constructor(init?: Partial<ViewModel>) {
        Object.assign(this, init);
    }

    projectId: number;
    navigationLinks: NavigationLink[];

    testplanList: TestPlanVm[] = [];
    suites: TestSuiteFlatVm[] = [];
    TestSuiteFlatVm

    suitesTree: TestSuiteTreeVm[] = [];
    suitesFlatList: TestSuiteTreeVm[] = [];

    _selectedSuite: TestSuiteTreeVm
    get selectedSuite(): TestSuiteTreeVm {
        return this._selectedSuite;
    }
    set selectedSuite(value: TestSuiteTreeVm) {
        let isChange = value?.id > 0 && value.id != this._selectedSuite?.id;
        if (isChange) {
            this.RefhreshAllByFilter();
        }
        this._selectedSuite = value;
        if (isChange)
            this.RefhreshAllByFilter();
    }

    points: TestPointVm[] = [];
    selectedPoint: TestPointVm;
    visiblePoints: TestPointVm[] = [];

    filter: FilterVm = new FilterVm();
    charts: TestSuiteChart[] = [];

    isPointTabSelected = false;
    isChartTabSelected = true;
}

export class FilterVm {
    passed: boolean = true;
    failed: boolean = true;
    blocked: boolean = true;
    skipped: boolean = true;
    other: boolean = true;

    withSubSuites: boolean = true;
    isAlwaysExpand: boolean = false;
}

export class TestPlanVm {
    constructor(init?: Partial<TestPlanVm>) {
        Object.assign(this, init);
    }
    id: number;
    name: string;
}


export class TestSuiteFlatVm {
    constructor(init?: Partial<TestSuiteFlatVm>) {
        Object.assign(this, init);
    }
    id: number;
    parentId: number;
    name: string;
    level: number;

    passed: number = 0;
    failed: number = 0;
    blocked: number = 0;
    skipped: number = 0;
    unknow: number = 0;
}

export class TestSuiteTreeVm {
    constructor(item: TestSuiteFlatVm) {
        this.item = item;
    }
    item: TestSuiteFlatVm;
    get id(): number { return this.item.id; }
    get name(): string { return this.item.name; }

    get passed() { return this.item.passed; }
    get failed() { return this.item.failed; }
    get blocked() { return this.item.blocked; }
    get notapplicable() { return this.item.skipped; }
    get other() { return this.item.unknow; }

    children: TestSuiteTreeVm[] = [];
    expanded: boolean = false;
}

export class TestPointVm {
    constructor(init?: Partial<TestPointVm>) {
        Object.assign(this, init);
    }
    id: number;
    testCaseId: number;
    name: string;
    order: number;
    outcome: Outcome;
    configuration: string;
    tester: string;
    isSelected: boolean; 
    expanded: boolean = false;
    stacktrace: string;
    description: string;
    suiteId: number;
    isStackVisible: boolean = false;

    showStack(): void {
        this.isStackVisible = !this.isStackVisible
    }
}

export class TestSuiteChart {
    constructor(points: TestPointVm[])
    {
        this.total = points.length;
        this.configuration = points[0].configuration;
        let g = new List(points).GroupBy(p => p.outcome);
        for(let key in g)
        {
            this.outcomes.push(new OutcomeChartConfig({
                color: this.GetColor(g[key][0].outcome),
                count: g[key].length,
                outcome: g[key][0].outcome
            }))
        }
        this.refresh();
    }
    configuration: string;
    total: number;
    outcomes: OutcomeChartConfig[] = [];
    refresh() {
        let data: number[] = [];
        let bg: string[] = [];
        let lables: string[] = [];
        this.outcomes.forEach(p => {
            this.doughnutChartLabels.push(Outcome[p.outcome]);
            data.push(p.count);
            bg.push(this.GetColor(p.outcome));
        });
        this.doughnutChartLabels = lables;
        this.doughnutChartData.datasets[0].data = data;
        this.doughnutChartData.datasets[0].backgroundColor = bg; 
    }

    GetColor(outcome: Outcome): string {
        if (outcome == Outcome.Passed)
            return "#269624"
        else if (outcome == Outcome.Failed)
            return "#b31b1b"
        else if (outcome == Outcome.Blocked)
            return "#e76d00"
        else if (outcome == Outcome.Skipped)
            return "#9A9483"
        else
            return "#9C719F"
    }

    doughnutChartLabels: string[] = [];
    doughnutChartData: ChartData<'doughnut'> = {
        labels: this.doughnutChartLabels,
        datasets: [
            {
                data: [],
                backgroundColor: [],
            },
        ]
    };
}

export class OutcomeChartConfig {
    constructor(init?: Partial<OutcomeChartConfig>) {
        Object.assign(this, init);
    }
    get outcomeStr(): string {
        return Outcome[this.outcome]
    }
    outcome: Outcome;
    color: string;
    count: number;
}