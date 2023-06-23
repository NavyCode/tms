
import { Subject } from 'rxjs';
import { GetPlansResponse, PlanState, TestPlanItem, UserIdentity, UserItem } from '../api/reports/swagger';
import { Link } from '../ext/links';
import { NavigationLink } from '../navigation/ViewModel';

export class ViewModel
{
    Refresh(response: GetPlansResponse) {
        this.plans = [];
        response.plans.forEach(p => {
            let plan = TestPlanVm.FromApi(p);
            plan.link = this.GetTestPlanLink(plan.id);
            this.plans.push(plan);
        })
    }
    AddPlan(plan: TestPlanVm) {
        
        plan.link = this.GetTestPlanLink(plan.id);
        this.plans.push(plan);

    }
    constructor(init?: Partial<ViewModel>) {
        Object.assign(this, init);
    }  
    projectId: number;
    selectedPlan: TestPlanVm;
    plans: TestPlanVm[] = [];
    isAddPlanModalVisible: boolean; 
    isDeletePlanModalVisible: boolean; 
    isEditPlanModalVisible: boolean; 
    users: UserIdentity[] = [];
    states: StatesListItem[] = [
        new StatesListItem(PlanState.Planed),
        new StatesListItem(PlanState.Active),
        new StatesListItem(PlanState.Closed),
    ];
    navigationLinks: NavigationLink[] = [];
    GetTestPlanLink(id: number): Link
    {
        return new Link({path: `/projects/${this.projectId}/testplans/${id}`})
    }
}

class StatesListItem
{
    constructor(value: PlanState) 
    {
        this.value = value;
        this.name = PlanState[value];
    }
    name: string;
    value: PlanState;
}



export class TestPlanVm  
{
    static FromApi(p: TestPlanItem): TestPlanVm {
        let result = new TestPlanVm({
            id: p.id,
            assignedTo: p.assignedTo,
            comment: p.comment,
            name: p.name,
            state: p.state
        });
        return result;
    }
    constructor(init?: Partial<TestPlanVm>) 
    {
        Object.assign(this, init);
    }

    id: number;
    name: string;
    comment: string;
    state: PlanState;
    get stateStr(): string
    {
        return PlanState[this.state];
    }
    assignedTo: UserIdentity;
    link: Link;
} 

