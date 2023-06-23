// ng build --base-href '/coverage/' --configuration production --aot

import { registerLocaleData } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import localeRu from '@angular/common/locales/ru';
import { LOCALE_ID, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';

import {ContextMenuModule} from 'primeng/contextmenu';
import { CookieModule, CookieService } from 'ngx-cookie';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { IndexComponent } from './index/projects.component';
import { LoginComponent } from './login/login.component';
import { NavigationComponent } from './navigation/navigation.component';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { TestPlansComponent } from './testplans/testplans.component';
import {TableModule} from 'primeng/table';
import {DropdownModule} from 'primeng/dropdown';
import { TestPlanComponent } from './testplan/testplan.component';
import {TreeModule} from 'primeng/tree';
import {SplitterModule} from 'primeng/splitter';
import {TabViewModule} from 'primeng/tabview';
import { TestCaseComponent } from './testcase/testcase.component';
import {InputTextModule} from 'primeng/inputtext';
import {EditorModule} from 'primeng/editor';
import { QuillModule } from 'ngx-quill';
import {AccordionModule} from 'primeng/accordion';
import {CheckboxModule} from 'primeng/checkbox';
import { LeftToolbarComponent } from './toolbar/lefttoolbar.component';
import { SettingsComponent } from './settings/settings.component'; 
import { NgChartsModule } from 'ng2-charts';
import {ListboxModule} from 'primeng/listbox';
import {CardModule} from 'primeng/card';
import { UsersComponent } from './users/users.component';
import { MessagesModule } from 'primeng/messages';
import {MenuModule} from 'primeng/menu'; 
import {MenuItem} from 'primeng/api';
import { SettingsConfigVarsComponent as SettingConfigVarsComponent } from './settings_configvars/settings_configvars.component';
import { SettingConfigsComponent } from './settings_configs/settings_configs.component';
import { ProgressReportComponent } from './progressreport/progressreport.component';





const appRoutes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'projects/:projectId/users',
    component: UsersComponent
  },
  {
    path: '',
    component: IndexComponent
  },
  {
    path: 'projects/:projectId/testplans',
    component: TestPlansComponent
  },
  {
    path: 'projects/:projectId/testplans/:{testPlanId}',
    component: TestPlanComponent
  },
  {
    path: 'projects/:projectId/testcases/:{testCaseId}',
    component: TestCaseComponent
  },
  {
    path: 'projects/:projectId/settings',
    component: SettingsComponent
  },
  {
    path: 'projects/:projectId/analytics/progressreport',
    component: ProgressReportComponent 
  },

  
  {
    path: 'projects/:projectId/setting_vars',
    component: SettingConfigVarsComponent
  },
  {
    path: 'projects/:projectId/setting_configs',
    component: SettingConfigsComponent
  },
];



registerLocaleData(localeRu);

@NgModule({
  declarations: [
    IndexComponent,
    AppComponent,
    NavigationComponent,
    LoginComponent,
    TestPlansComponent,
    TestPlanComponent,
    TestCaseComponent,
    LeftToolbarComponent,
    SettingsComponent,
    ProgressReportComponent,
    UsersComponent,
    SettingConfigVarsComponent,
    SettingConfigsComponent
  ],
  imports: [
    RouterModule.forRoot(appRoutes, {
      enableTracing: false
    }),
    CookieModule.forRoot(),
    AppRoutingModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    HttpClientModule,
    BrowserModule,
    DialogModule,
    ButtonModule,
    TableModule,
    DropdownModule,
    TreeModule,
    SplitterModule,
    TabViewModule,
    InputTextModule,
    EditorModule,
    AccordionModule,
		ContextMenuModule,
    CheckboxModule,
    MessagesModule,
    MenuModule,
    
    QuillModule.forRoot({
      customOptions: [{
        import: 'formats/font',
        whitelist: ['mirza', 'roboto', 'aref', 'serif', 'sansserif', 'monospace']
      }]
    }),
    NgChartsModule,
    ListboxModule,
    CardModule
  ],
  exports: [],
  providers: [
    CookieService,
    { provide: LOCALE_ID, useValue: "ru" },
  ],
  bootstrap: [AppComponent]
})

export class AppModule {
  constructor() {
  }
}
