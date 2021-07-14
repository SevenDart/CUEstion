import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { Routes, RouterModule } from '@angular/router';
import {HttpClientModule} from '@angular/common/http';

import {AppComponent } from './app.component';
import {NavigationBarComponent} from './navigation-bar/navigation-bar.component';
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import {SearchFieldComponent} from './search-field/search-field.component';
import {MatInputModule} from '@angular/material/input';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {FooterBarComponent} from './footer-bar/footer-bar.component';
import {HomePageComponent} from './home-page/homepage.component';
import {MatButtonModule} from '@angular/material/button';
import {MatTableModule} from '@angular/material/table';
import {QuestionPageComponent} from './question-page/question-page.component';
import {MatChipsModule} from '@angular/material/chips';
import {MatExpansionModule} from '@angular/material/expansion';
import {MatDividerModule} from '@angular/material/divider';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatDialogModule} from '@angular/material/dialog';
import {SignDialogComponent} from './auth-panel/sign-dialog/sign-dialog.component';
import {AuthPanelComponent} from './auth-panel/auth-panel.component';
import {MatTabsModule} from '@angular/material/tabs';
import {MatIconModule} from '@angular/material/icon';
import {LogInComponent} from './auth-panel/sign-dialog/log-in/log-in.component';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import {SnackBarContentComponent} from './auth-panel/sign-dialog/snack-bar-content/snack-bar-content.component';
import {RegisterFormComponent} from './auth-panel/sign-dialog/register-form/register-form.component';
import {CreateQuestionComponent} from './create-question/create-question.component';
import {TextFieldModule} from '@angular/cdk/text-field';
import {AuthorizationGuard} from './guards/authorization.guard';
import {UpdateQuestionComponent} from './update-question/update-question.component';
import {TagSelectComponent} from './tag-select/tag-select.component';
import {ConfirmDialogComponent} from './confirm-dialog/confirm-dialog.component';
import {MatMenuModule} from '@angular/material/menu';
import {TagEditorComponent} from './tag-editor/tag-editor.component';
import {MatListModule} from '@angular/material/list';


const routes: Routes = [
  {path: 'home', component: HomePageComponent},
  {path: 'question/create', component: CreateQuestionComponent, canActivate: [AuthorizationGuard]},
  {path: 'question/:id', component: QuestionPageComponent},
  {path: 'question/:id/edit', component: UpdateQuestionComponent, canActivate: [AuthorizationGuard]},
  {path: '', redirectTo: 'home'},
  {path: '/', redirectTo: 'home'}
];


@NgModule({
  declarations: [
    AppComponent,
    NavigationBarComponent,
    SearchFieldComponent,
    FooterBarComponent,
    HomePageComponent,
    QuestionPageComponent,
    AuthPanelComponent,
    SignDialogComponent,
    LogInComponent,
    SnackBarContentComponent,
    RegisterFormComponent,
    CreateQuestionComponent,
    UpdateQuestionComponent,
    TagSelectComponent,
    ConfirmDialogComponent,
    TagEditorComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    MatInputModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    BrowserAnimationsModule,
    RouterModule.forRoot(routes),
    MatButtonModule,
    HttpClientModule,
    MatTableModule,
    MatChipsModule,
    MatExpansionModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatTabsModule,
    MatIconModule,
    MatSnackBarModule,
    TextFieldModule,
    MatMenuModule,
    MatListModule
  ],
  providers: [AuthorizationGuard],
  exports: [RouterModule],
  bootstrap: [AppComponent]
})
export class AppModule { }
