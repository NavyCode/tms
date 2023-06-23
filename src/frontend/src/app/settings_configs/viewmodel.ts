import { List } from "linqts";
import { GetConfigsResponse, GetConfigsVarsResponse } from "../api/reports/swagger";
import { NavigationLink } from "../navigation/ViewModel";
import { ConfigVarVal, ConfigVarVm } from "../settings_configvars/viewmodel";
import { Guid } from "guid-typescript";

export class ViewModel
{
    configVarList: ConfigVarVm[] = [];
    Refresh(response: GetConfigsResponse, vars: GetConfigsVarsResponse) {
      this.configs = [];
      this.configVarList = new List(vars.vars).Select(p => ConfigVarVm.FromApi(p)).ToArray();
        response.configs.forEach(c => {
        let config = new ConfigVm({
            id: c.id,
            comment: c.comment,
            isDefault: c.isDefault,
            name: c.name,
            params: [],
        })
        c.variables.forEach(v => {
            let configParam = new ConfigParam(
                {
                    id: v.id,
                    var: this.configVarList.find(p => p.id == v.variableId),
                }
            );

            configParam.val = configParam.var?.values.find(p => p.id == v.valueId);
            config.params.push(configParam)
        })
        this.configs.push(config);
      })
    }
    isAddOrEditConfigModalVisible: any;
    isDeleteConfigModalVisible: any;
    constructor(init?: Partial<ViewModel>) {
        Object.assign(this, init);
    }  
    configs: ConfigVm[] = [];
    selectedConfig: ConfigVm;
    projectId: number;
    navigationLinks: NavigationLink[] = []; 
}
 

export class ConfigVm
{
    constructor(init?: Partial<ConfigVm>) 
    {
        Object.assign(this, init);
    }
    id: number;
    isDefault: boolean = false;
    name: string;
    comment: string;
    params: ConfigParam[] = [];
    selectedParam: ConfigParam;
    get strParams(): string{
        return  new List(this.params).Where(p => p != null && p.val != null && p.var != null)
            .Select(p => p.var.name + ": " + p.val.name).ToArray().join(", ");
    }
}   

export class ConfigParam
{
    constructor(init?: Partial<ConfigParam>) 
    {
        Object.assign(this, init);
    }
    id: number;
    uid: Guid = Guid.create();
    var: ConfigVarVm;
    val: ConfigVarVal;
}  