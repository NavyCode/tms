
import { Subject } from 'rxjs';
import { NavigationLink } from '../navigation/ViewModel';
import { ConfigsForAssign, GetSuitesTreeResponse, IReportClient, Outcome, SearchTestCaseItem, SuiteTestCaseItem, SuitesTreeItem, WiState } from '../api/reports/swagger';
import { List } from 'linqts';
import { environment } from 'src/environments/environment';
import { EventEmitter, ɵbypassSanitizationTrustResourceUrl } from '@angular/core';

export class ViewModel
{
    SelectSuite(suiteId: number) {
        console.info('Show suiteId')
        let suite = this.suitesFlatList.find(p => p.id == suiteId);
        let parent = suite;
        while(parent != null) 
        {
            console.info("Expand" + parent.name)
            parent.expanded = true;
            parent = parent.parent; 
        }
        if(suite != null)
            this.selectedSuite = suite;
    }
    suitesFlatList: TestSuiteVm[] = [];  
    onUpdateUrl: EventEmitter<String>  = new EventEmitter<String>();

    async RefreshSelectedSuite() 
    {
        if (!environment.design) 
        {
            this.defineTests = [];
            this.executeTests = [];
            if(this.isDefineTabSelected)
            {
                let response = await this.api.testSuite_GetSuiteTestCases(this.projectId, this.selectedSuite.id, this.isShowTestsWithChildrenSuite,   this.planId.toString());
                response.tests.forEach(p => {
                    this.defineTests.push(DefineTestCaseVm.FromApi(p));
                })
            }
            if(this.isExecuteTabSelected)
            {
                let points = await this.api.testPoints_GetSuiteTestPoints(this.projectId, this.selectedSuite.id, this.isShowTestsWithChildrenSuite,  this.planId.toString());
                points.points.forEach(p => {
                    this.executeTests.push(new ExecuteTestCaseVm(
                        {
                            configuration: p.configuration,
                            testcaseId: p.testCaseId,
                            name: p.name,
                            order: p.order,
                            outcome: p.outcome,
                            pointId: p.id,
                            priority: p.priority, 
                            tester: p.tester
                        }
                    ))
                })
            }
        }
    } 
    api: IReportClient;
    Refresh(response: GetSuitesTreeResponse) {
        this.suitesFlatList = [];
        this.rootSuite = TestSuiteVm.FromApi(response.suites.find(p => p.parentId == null), null) ;
        this.suitesFlatList.push(this.rootSuite);
        this.LoadChildren(response, this.rootSuite);
        this.suitesTree = [this.rootSuite]; 
    }
    

    get reportLink(): NavigationLink
    { 
        return NavigationLink.ProgressReportPlan(this.projectId, this.planId);
    }
    
    planId: number;
    constructor(init?: Partial<ViewModel>) {
        Object.assign(this, init);
    }  
    rootSuite: TestSuiteVm;

    projectId: number;
    navigationLinks: NavigationLink[];

    get isShowTestsWithChildrenSuite(): boolean
    {
        return this._isShowTestsWithChildrenSuite;
    }
    set isShowTestsWithChildrenSuite(value: boolean)
    {
        this._isShowTestsWithChildrenSuite = value;
        this.RefreshSelectedSuite();
    }
    _isShowTestsWithChildrenSuite: boolean;

    isAddOrEditSuiteModalVisible : boolean; 
    isDeleteSuiteModalVisible: boolean;
    
    isAddTestCasesModal : boolean;
    addTestCasesModal : AddTestCasesModalVm = new AddTestCasesModalVm();

    isAssignConfigModal : boolean;
    assignConfigModal : AssignConfigModalVm = new AssignConfigModalVm();
 
    isAssignTesterModal : boolean;
    assignTesterModal : AssignTesterModalVm = new AssignTesterModalVm();
    
    isCreateTestCaseModal : boolean;
    isDeleteTestCaseModalVisible: boolean;

    _isEditTestCaseModal : boolean; 
    get isEditTestCaseModal(): boolean
    {
        return this._isEditTestCaseModal;
    }
    set isEditTestCaseModal(value: boolean)
    {
        this._isEditTestCaseModal = value;
        if(this._isEditTestCaseModal == false)
            this.RefreshSelectedSuite();
    }

    editTestCaseId: number;
 
    _tabSelectedIndex : number = 0; 
    get tabSelectedIndex(): number
    { 
        return this._tabSelectedIndex;
    }
    set tabSelectedIndex(value: number)
    { 
        if(this._tabSelectedIndex == value)
            return;
        this._tabSelectedIndex = value;
        this.onUpdateUrl.emit();
        this.RefreshSelectedSuite();
    }

    get isDefineTabSelected(): boolean
    { 
        return this._tabSelectedIndex == 0;
    }
    get isExecuteTabSelected(): boolean
    { 
        return this._tabSelectedIndex == 1;
    }
    get isReportTabSelected(): boolean
    { 
        return this._tabSelectedIndex == 2;
    }
 
    suitesTree: TestSuiteVm[] = []; 

    editSuite: TestSuiteVm;

    _selectedSuite: TestSuiteVm;
    get selectedSuite(): TestSuiteVm
    {
        return this._selectedSuite;
    }
    set selectedSuite(value: TestSuiteVm)
    {
        if(this._selectedSuite != value)
            this._selectedSuite = value;
        this.RefreshSelectedSuite();
        this.onUpdateUrl.emit();
    }
    defineSelectAll: boolean; 
    

    testPlanName: string;
  
    defineTests: DefineTestCaseVm[] = [];
    get defineTestCasesSelectAll(): boolean
    {
        return this._defineTestCasesSelectAll;
    }
    set defineTestCasesSelectAll(value: boolean)
    {
        this._defineTestCasesSelectAll = value;
        this.defineTests.forEach(p => p.selected = this._defineTestCasesSelectAll);
    }
    _defineTestCasesSelectAll: boolean = false;

     
    defineSelectedTests(): DefineTestCaseVm[] 
    {
        return this.defineTests.filter(p => p.selected);
    }

    executeTests:  ExecuteTestCaseVm[] = [];
    executeSelectedTests():  ExecuteTestCaseVm[]
    {
        return this.executeTests.filter(p => p.selected);
    } 
    get executeTestCasesSelectAll(): boolean
    {
        return this._executeTestCasesSelectAll;
    }
    set executeTestCasesSelectAll(value: boolean)
    {
        this._executeTestCasesSelectAll = value;
        this.executeTests.forEach(p => p.selected = this._executeTestCasesSelectAll);
    }
    _executeTestCasesSelectAll: boolean = false;


    resultsTree: ResultTestSuiteVm[];

    private LoadChildren(response: GetSuitesTreeResponse, suite: TestSuiteVm ) {
        suite.children = new List(response.suites).Where(p => p.parentId == suite.id)
            .Select(p => TestSuiteVm.FromApi(p, suite)).ToArray();
        suite.children.forEach(p => 
            {
                this.suitesFlatList.push(p);
                this.LoadChildren(response, p)
            });
    }

    isShowSetCommentModal: boolean;
    testPointCommentModal: string;
}
 

export class TestSuiteVm  
{
    static FromApi(p: SuitesTreeItem, parent: TestSuiteVm): TestSuiteVm {
       return new TestSuiteVm({
        id: p.id,
        expanded: false,
        name: p.name,
        parentId: p.parentId,
        parent: parent
       })
    }
    constructor(init?: Partial<TestSuiteVm>) 
    {
        Object.assign(this, init);
    }
    id: number;
    parentId: number;
    parent: TestSuiteVm;
    name: string;
    children: TestSuiteVm[] = [];
    expanded: boolean = false;
} 

export class DefineTestCaseVm  
{
    static FromApi(p: SuiteTestCaseItem): DefineTestCaseVm
    {
        return new DefineTestCaseVm({
            id: p.id,
            testCaseId: p.testCaseId,
            name: p.name,
            order: p.order,
            priority: p.priority,
            state: p.state
        })
    }
    constructor(init?: Partial<DefineTestCaseVm>) 
    {
        Object.assign(this, init);
    }
    id: number;
    name: string;
    testCaseId: number;
    order: number;
    state: WiState;
    get stateStr(): string
    {
        return WiState[this.state];
    }
    priority: number;
    selected: boolean = false;
}  

export class ExecuteTestCaseVm  
{
    selected: boolean;
    constructor(init?: Partial<ExecuteTestCaseVm>) 
    {
        Object.assign(this, init);
    }
    pointId: number;
    testcaseId: number;
    name: string;
    order: number;
    priority: number;
    outcome: Outcome;
    expanded: boolean = false;
    get outcomeStr(): string
    {
        return Outcome[this.outcome];
    }
    configuration: string;
    tester: string;
} 


export class ResultTestSuiteVm  
{
    constructor(init?: Partial<ResultTestSuiteVm>) 
    {
        Object.assign(this, init);
    }
    id: number;
    name: string;
    passedCount: number;
    failedCount: number;
    brokenCount: number;
    otherCount: number;
    children: TestSuiteVm[] = [];
} 


export class ResultTestCaseVm 
{
    constructor(init?: Partial<ResultTestCaseVm>) 
    {
        Object.assign(this, init);
    }
    id: number;
    name: string;
    state: string;
    duration: number;
    number: number;
} 


export class ResultTestCaseInfoVm
{
    constructor(init?: Partial<ResultTestCaseInfoVm>) 
    {
        Object.assign(this, init);
    }
    id: number;
    name: string;
    state: string;
    duration: number;
    comment: string;
} 

export class AddTestCasesModalVm
{
    selectAll: boolean;
    constructor(init?: Partial<AddTestCasesModalVm>) 
    {
        Object.assign(this, init);
    }
    searchText: string = "";
    result: AddTestCasesModalSearchResultVm[] = [];
}

export class AddTestCasesModalSearchResultVm
{
    static FromApi(p: SearchTestCaseItem): AddTestCasesModalSearchResultVm {
      return new AddTestCasesModalSearchResultVm({
        id: p.id,
        name: p.name,
        priority: p.priority
      })
    }
    constructor(init?: Partial<AddTestCasesModalSearchResultVm>) 
    {
        Object.assign(this, init);
    }
    id: number;
    name: string;
    priority: number;
    selected: boolean = false;
}


export class AssignConfigModalVm
{
    get selectAll(): boolean
    {
        return this._selectAll;
    }
    set selectAll(value: boolean)
    {
        this._selectAll = value;
        this.result.forEach(p => p.selected = this._selectAll);
    }
    _selectAll: boolean = false;

    get isSuite(): boolean
    {
        return this.suiteId > 0;
    }
    suiteId: number;
    
    constructor(init?: Partial<AssignConfigModalVm>) 
    {
        Object.assign(this, init);
    }
    searchText: string = "";
    result: AssignConfigModalSearchResultVm[] = [];
}

export class AssignConfigModalSearchResultVm
{
    static FromApi(p: ConfigsForAssign): any {
        return new AssignConfigModalSearchResultVm({
            id: p.id,
            name: p.name,
            params: p.values
        })
    }
    constructor(init?: Partial<AssignConfigModalSearchResultVm>) 
    {
        Object.assign(this, init);
    }
    id: number;

    selected: boolean = false;
    name: string;
    params: string;

    ChangeSelection(): void
    {
        this.selected = !this.selected;
    }
}


export class AssignTesterModalVm
{
    selectAll: boolean;
    constructor(init?: Partial<AssignTesterModalVm>) 
    {
        Object.assign(this, init);
    }
    searchText: string = "";
    
    selected: AssignTesterSearchResultVm;
    result: AssignTesterSearchResultVm[] = [];
}

export class AssignTesterSearchResultVm
{
    constructor(init?: Partial<AssignTesterSearchResultVm>) 
    {
        Object.assign(this, init);
    }
    id: number;
    name: string;
}