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


const routes: Routes = [
  {path: 'home', component: HomePageComponent},
  {path: 'question/:id', component: QuestionPageComponent},
  {path: '', redirectTo: 'home'}
];


@NgModule({
  declarations: [
    AppComponent,
    NavigationBarComponent,
    SearchFieldComponent,
    FooterBarComponent,
    HomePageComponent,
    QuestionPageComponent
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
    MatDividerModule
  ],
  exports: [RouterModule],
  bootstrap: [AppComponent]
})
export class AppModule { }
