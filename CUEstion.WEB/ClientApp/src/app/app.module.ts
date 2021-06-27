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


const routes: Routes = [
  {path: 'Home', component: HomePageComponent},
  {path: '', redirectTo: 'Home'}
];


@NgModule({
  declarations: [
    AppComponent,
    NavigationBarComponent,
    SearchFieldComponent,
    FooterBarComponent,
    HomePageComponent
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
    MatTableModule
  ],
  exports: [RouterModule],
  bootstrap: [AppComponent]
})
export class AppModule { }
