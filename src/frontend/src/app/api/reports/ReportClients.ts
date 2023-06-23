import { HttpClient } from '@angular/common/http';
import { AddOrUpdateConfigsRequest, AddOrUpdateConfigVarRequest, AddOrUpdatePlanRequest, AddOrUpdateProjectRequest, AddOrUpdateSuiteRequest, AddOrUpdateTestCaseRequest, AddOrUpdateUserRequest, AddResultsRequest, AddRunRequest, AddTestCaseToSuiteRequest, DeleteTestCaseFromSuiteRequest, GetConfigsForAssignResponse, GetConfigsResponse, GetConfigsVarsResponse, GetPlansResponse, GetProjectsResponse, GetSuitesTreeResponse, GetSuiteTestCasesResponse, GetSuiteTestPointsResponse, GetTestCaseResponse, GetTestPlanResultResponse, GetUsersResponse, IReportClient, LoggedUserInfo, ReorderTestCasesRequest, SetOutcomeRequest, SetTestCaseConfigRequest, SetTesterRequest, SetTestSuiteConfigRequest, UpdateResultRequest, UserIdentity } from './swagger';


export namespace ReportClients 
{   
    export class MockReportClient  implements IReportClient
    {
        _http: HttpClient;
        constructor(private http: HttpClient)  { 
            this._http = http;
        } 
        config_GetConfigs(projectId: number): Promise<GetConfigsResponse> {
            throw new Error('Method not implemented.');
        }
        config_AddConfig(projectId: number, request: AddOrUpdateConfigsRequest): Promise<number> {
            throw new Error('Method not implemented.');
        }
        config_GetConfigsVars(projectId: number): Promise<GetConfigsVarsResponse> {
            throw new Error('Method not implemented.');
        }
        config_AddConfigVariable(projectId: number, request: AddOrUpdateConfigVarRequest): Promise<void> {
            throw new Error('Method not implemented.');
        }
        config_UpdateConfig(configId: number, request: AddOrUpdateConfigsRequest, projectId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        config_DeleteConfig(configId: number, projectId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        config_UpdateConfigVariable(projectId: number, id: number, request: AddOrUpdateConfigVarRequest): Promise<void> {
            throw new Error('Method not implemented.');
        }
        plan_GetSuitesTree(planId: number, projectId: string): Promise<GetSuitesTreeResponse> {
            throw new Error('Method not implemented.');
        }
        runs_GetResult(planId: number, projectId: string): Promise<GetTestPlanResultResponse> {
            throw new Error('Method not implemented.');
        }
        runs_Add(request: AddRunRequest, projectId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        runs_AddResults(request: AddResultsRequest, projectId: string, runId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        runs_UpdateResult(request: UpdateResultRequest, projectId: string, runId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        testPoints_GetSuiteTestPoints(suiteId: number, withCildrenSuites: boolean, projectId: string, planId: string): Promise<GetSuiteTestPointsResponse> {
            throw new Error('Method not implemented.');
        }
        testPoints_SetTester(request: SetTesterRequest, projectId: string, planId: string, suiteId: string, testId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        testPoints_GetUserListForAssign(search: string, projectId: string): Promise<UserIdentity[]> {
            throw new Error('Method not implemented.');
        }
        testPoints_SetOutcome(request: SetOutcomeRequest, projectId: string, planId: string, suiteId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        testSuite_GetSuiteTestCases(suiteId: number, withCildren: boolean, projectId: string, planId: string): Promise<GetSuiteTestCasesResponse> {
            throw new Error('Method not implemented.');
        }
        testSuite_AddTestCaseSuite(suiteId: number, request: AddTestCaseToSuiteRequest, projectId: string, planId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        testSuite_DeleteTestCaseFromSuite(request: DeleteTestCaseFromSuiteRequest, projectId: string, planId: string, suiteId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        testSuite_GetSuiteConfigs(suiteId: number, projectId: string, planId: string): Promise<number[]> {
            throw new Error('Method not implemented.');
        }
        testSuite_SetConfig(suiteId: number, request: SetTestSuiteConfigRequest, projectId: string, planId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        testSuite_ReorderTestCasess(request: ReorderTestCasesRequest, projectId: string, planId: string, suiteId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        testSuite_SetTestCaseConfig(request: SetTestCaseConfigRequest, projectId: string, planId: string, suiteId: string, testId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        testSuite_GetConfigsForAssign(projectId: string): Promise<GetConfigsForAssignResponse> {
            throw new Error('Method not implemented.');
        }
        testSuites_AddSuite(planId: number, request: AddOrUpdateSuiteRequest, projectId: string): Promise<number> {
            throw new Error('Method not implemented.');
        }
        testSuites_GetChildrenSuites(planId: number, suiteId: number, projectId: string): Promise<GetSuitesTreeResponse> {
            throw new Error('Method not implemented.');
        }
        testSuites_UpdateSuite(suiteId: number, request: AddOrUpdateSuiteRequest, projectId: string, planId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        testSuites_DeleteSuite(id: number, projectId: string, planId: string, suiteId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        admin_ClearDb(): Promise<void> {
            throw new Error('Method not implemented.');
        }
        plans_GetPlans(projectId: number): Promise<GetPlansResponse> {
            throw new Error('Method not implemented.');
        }
        plans_AddPlan(projectId: number, request: AddOrUpdatePlanRequest): Promise<number> {
            throw new Error('Method not implemented.');
        }
        plans_UpdatePlan(planId: number, request: AddOrUpdatePlanRequest, projectId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        plans_DeletePlan(planId: number, projectId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        projects_GetProjects(): Promise<GetProjectsResponse> {
            throw new Error('Method not implemented.');
        }
        projects_AddProject(request: AddOrUpdateProjectRequest): Promise<number> {
            throw new Error('Method not implemented.');
        }
        projects_UpdateProject(projectId: number, request: AddOrUpdateProjectRequest): Promise<void> {
            throw new Error('Method not implemented.');
        }
        projects_DeleteProject(projectId: number): Promise<void> {
            throw new Error('Method not implemented.');
        }
        testCases_GetTestCase(testId: number, projectId: string): Promise<GetTestCaseResponse> {
            throw new Error('Method not implemented.');
        }
        testCases_UpdateTest(testId: number, request: AddOrUpdateTestCaseRequest, projectId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        testCases_DeleteTest(testId: number, projectId: string): Promise<void> {
            throw new Error('Method not implemented.');
        }
        testCases_AddTest(projectId: number, request: AddOrUpdateTestCaseRequest): Promise<number> {
            throw new Error('Method not implemented.');
        }
        users_Login(login: string, pass: string): Promise<string> {
            throw new Error('Method not implemented.');
        }
        users_UserInfo(): Promise<LoggedUserInfo> {
            throw new Error('Method not implemented.');
        }
        users_UpdateCurrentUser(request: AddOrUpdateUserRequest): Promise<void> {
            throw new Error('Method not implemented.');
        }
        users_UpdateUser(userId: number, request: AddOrUpdateUserRequest): Promise<void> {
            throw new Error('Method not implemented.');
        }
        users_AddUser(request: AddOrUpdateUserRequest): Promise<number> {
            throw new Error('Method not implemented.');
        }
        users_GetUsers(): Promise<GetUsersResponse> {
            throw new Error('Method not implemented.');
        }
        users_DeleteUser(id: number): Promise<void> {
            throw new Error('Method not implemented.');
        }
        wakeup_Ping(): Promise<void> {
            throw new Error('Method not implemented.');
        }
    }
}
