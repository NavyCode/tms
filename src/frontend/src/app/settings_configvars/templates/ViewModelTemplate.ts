import { ConfigVarVal, ConfigVarVm, ViewModel } from "../viewmodel";

export class ViewModelTemplate {
    static WithSettings(): ViewModel {
        let result = new ViewModel({
            projectId: 1,
            vars: [
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
        return result;
    }


    static ShowEditVar(): ViewModel {
        let result = this.WithSettings();
        result.selectedVar = result.vars[0];
        result.isAddOrEditVarModalVisible = true;
        return result;
    }
}
