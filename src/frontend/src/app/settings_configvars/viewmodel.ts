import { Guid } from "guid-typescript";
import { List } from "linqts";
import { ConfigVarItem, ConfigVarValueItem, GetConfigsVarsResponse } from "../api/reports/swagger";
import { NavigationLink } from "../navigation/ViewModel";

export class ViewModel {
    Refresh(response: GetConfigsVarsResponse) {
        this.vars = [];
        response.vars.forEach(p => this.vars.push(ConfigVarVm.FromApi(p)));
    }
    isAddOrEditVarModalVisible: any;
    isDeleteVarModalVisible: any;
    constructor(init?: Partial<ViewModel>) {
        Object.assign(this, init);
    }
    vars: ConfigVarVm[];
    selectedVar: ConfigVarVm;
    projectId: number;
    navigationLinks: NavigationLink[] = [];
}


export class ConfigVarVm {
    static FromApi(p: ConfigVarItem): ConfigVarVm {
        let result = new ConfigVarVm({
            id: p.id,
            comment: p.comment,
            name: p.name
        })
        p.values.forEach(v => result.values.push(ConfigVarVal.FromApi(v)))
        return result;
    }
    constructor(init?: Partial<ConfigVarVm>) {
        Object.assign(this, init);
    }
    id: number;
    name: string;
    comment: string;
    get strValues(): string {
        return new List(this.values).Select(p => p.name).ToArray().join(", ");
    }
    values: ConfigVarVal[] = [];
    selectedValue: ConfigVarVal;
}


export class ConfigVarVal {
    uid: Guid = Guid.create();
    static FromApi(v: ConfigVarValueItem): ConfigVarVal {
        return new ConfigVarVal({
            id: v.id,
            name: v.value
        })
    }
    constructor(init?: Partial<ConfigVarVal>) {
        Object.assign(this, init);
    }
    id: number;
    name: string;
}  
