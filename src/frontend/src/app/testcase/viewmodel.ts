
import { Guid } from 'guid-typescript';
import { List } from 'linqts';
import { Subject } from 'rxjs';
import { GetTestCaseResponse, WiState } from '../api/reports/swagger';

export class ViewModel
{
    Refresh(tc: GetTestCaseResponse) {
        this.title = tc.title;
        this.preconditions = tc.precondition;
        this.postconditions = tc.postcondition;
        this.state = tc.state;
        this.steps = [];
        tc.steps.forEach(p => {
            this.steps.push(new TestStepVm({
                id: p.id,
                action: p.action,
                number: p.order,
                result: p.result
            }))
        });
        this.ReorderSteps();
    }
    ReorderSteps() {
        let i = 1;
        new List(this.steps).OrderBy(p => p.number).ForEach(p => p.number = i++);
    }
    constructor(init?: Partial<ViewModel>) {
        Object.assign(this, init);
        if(this.steps.length == 0)
        this.steps.push(new TestStepVm({
            number: 1
        }))
    }  
    id: number;
    projectId: number;
    steps: TestStepVm[] = [];
    selectedSteps: TestStepVm[] = [];
    title: string;
    preconditions: string;
    description: string;
    postconditions: string;
    assignedTo: string;
    users: string[] = [];
    state: WiState = WiState.Design;
    states: WiStateListItem[] = [
        new WiStateListItem(WiState.Design), 
        new WiStateListItem(WiState.Ready), 
        new WiStateListItem(WiState.Closed)
    ];
}
 


class WiStateListItem
{
    constructor(value: WiState) 
    {
        this.value = value;
        this.name = WiState[value];
    }
    name: string;
    value: WiState;
}


export class TestStepVm  
{
    constructor(init?: Partial<TestStepVm>) 
    {
        Object.assign(this, init);
    }

    id: number;
    uid: Guid = Guid.create();
    number: number;
    action: string;
    result: string;
} 

