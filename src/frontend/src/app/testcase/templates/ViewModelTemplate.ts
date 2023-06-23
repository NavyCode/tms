import { WiState } from "src/app/api/reports/swagger";
import { TestStepVm, ViewModel } from "../viewmodel";

export class ViewModelTemplate {
    static WithProjects(): ViewModel {
        let result = new ViewModel({
            id: 25,
            assignedTo: "Иванов Иван Иванович",
            description: "Описание",
            postconditions: "Авторизоваться и открыть Gmail",
            preconditions: "Выйти из учетки",
            state: WiState.Ready,
            title: "Отправка письма",
            steps: [
                new TestStepVm(
                    {
                        id: 1,
                        number: 1,
                        action: "Отправить письмо на почту.<br>Текст: привет, это тест письма",
                        result: ""
                    }),
                new TestStepVm(
                    {
                        id: 2,
                        number: 2,
                        action: "Проверить отправленные",
                        result: "Появилась запись об отправленном письме"
                    }),
                    new TestStepVm(
                        {
                            id: 3,
                            number: 3,
                            action: 'Проверить получение письма в ящике, откуда письмо отправлено',
                            result: 'Письмо получено'
                        }),
                        new TestStepVm(
                            {
                                id: 4,
                                action: "Разлогиниться",
                                number: 4,
                            })
                ],
                users:[
                    "Иванов Иван Иванович", 
                    "Петров Петр Петрович",
                    "Сидоров Сидор Сидорович",
                ]
        });
        for(let i = 5; i < 20; i++)
        {
            result.steps.push(
                new TestStepVm(
                    {
                        id: i,
                        number: i,
                        action: "Новый шаг" + i,
                        result: ""
                    }));
        }
        return result;
    }
}
