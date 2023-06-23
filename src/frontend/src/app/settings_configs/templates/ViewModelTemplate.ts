import { ConfigVarVal, ConfigVarVm } from "src/app/settings_configvars/viewmodel";
import { ConfigParam, ConfigVm, ViewModel } from "../viewmodel";

export class ViewModelTemplate {
    static WithSettings(): ViewModel {
        let result = new ViewModel({
            projectId: 1,
            configVarList: [
                new ConfigVarVm({
                    comment: "Comment",
                    id: 1,
                    name: "Name",
                    values: [
                        new ConfigVarVal({ id: 1, name: "Value 1" }),
                        new ConfigVarVal({ id: 2, name: "Value 2" })
                    ]
                })
            ]
        });
        result.configs = [
            new ConfigVm({
                id: 1,
                comment: "Comment",
                name: "Windows 10",
                params: [
                    new ConfigParam({
                        id: 1,
                        var: result.configVarList[0],
                        val: result.configVarList[0].values[0]
                    })
                ]
            })
        ]
        return result;
    }


    static ShowEditConfig(): ViewModel {
        let result = this.WithSettings();
        result.selectedConfig = result.configs[0];
        result.isAddOrEditConfigModalVisible = true;
        return result;
    }
}
