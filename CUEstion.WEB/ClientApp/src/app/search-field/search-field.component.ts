import { Component, OnInit } from '@angular/core';
import {FormControl} from '@angular/forms';
import {Observable, of, pipe} from 'rxjs';
import {debounceTime, distinctUntilChanged, map, startWith, switchMap} from 'rxjs/operators';
import {QuestionsService} from '../services/questions.service';
import {Question} from '../Models/Question';
import {MatAutocompleteSelectedEvent} from '@angular/material/autocomplete';
import {Router} from '@angular/router';

@Component({
  selector: 'search-field',
  templateUrl: './search-field.component.html',
  styleUrls: ['./search-field.component.css'],
  providers: [QuestionsService]
})
export class SearchFieldComponent implements OnInit {
  searchControl = new FormControl();
  results: Observable<Question[]>;

  constructor(private questionsService: QuestionsService, private router: Router) {
  }

  ngOnInit() {
    this.results = this.searchControl.valueChanges.pipe(
      debounceTime(2000),
      distinctUntilChanged(),
      startWith(''),
      switchMap(value => this.Search(value))
    );
  }

  private Search(query: string) {
    if (query === '') {
      return of([]);
    }
    return this.questionsService.SearchQuestions(query);
  }

  Display(question: Question) {
    return question ? question.header : '';
  }

  RedirectToPage(event: MatAutocompleteSelectedEvent) {
    const question: Question = event.option.value;
    this.router.navigate([`/questions/`, question.id]);
  }

}
