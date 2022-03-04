import {Component, ElementRef, Input, OnInit, ViewChild} from '@angular/core';
import {FormControl} from '@angular/forms';
import {Observable, of, pipe} from 'rxjs';
import {debounceTime, distinctUntilChanged, map, startWith, switchMap} from 'rxjs/operators';
import {QuestionsService} from '../services/questions.service';
import {Question} from '../Models/Question';
import {MatAutocompleteSelectedEvent} from '@angular/material/autocomplete';
import {ActivatedRoute, Params, Router} from '@angular/router';
import {MatChipInputEvent} from '@angular/material/chips';

@Component({
  selector: 'search-field',
  templateUrl: './search-field.component.html',
  styleUrls: ['./search-field.component.css'],
  providers: [QuestionsService]
})
export class SearchFieldComponent implements OnInit {
  searchControl = new FormControl();
  results: Observable<Question[]>;

  @Input() isExpandable = true;

  tagPanelExpanded = false;

  selectedTags: string[] = [];

  constructor(private questionsService: QuestionsService, private router: Router, activatedRoute: ActivatedRoute) {
      activatedRoute.queryParams.subscribe((params: Params) => {
        if (this.isExpandable) {
          if (params.tags !== undefined) {
            if (params.tags instanceof Array) {
              this.selectedTags = params.tags;
            } else {
              this.selectedTags.push(params.tags);
            }
          }
        }
      });
  }

  ngOnInit() {
    this.results = this.searchControl.valueChanges.pipe(
      debounceTime(1000),
      distinctUntilChanged(),
      startWith(''),
      switchMap(value => this.Search(value))
    );
  }

  private Search(query: string) {
    if (query === '' && this.selectedTags.length === 0) {
      return of([]);
    }
    return this.questionsService.SearchQuestions(query, this.selectedTags);
  }

  Display(question: Question) {
    return question ? question.header : '';
  }

  RedirectToPage(event: MatAutocompleteSelectedEvent) {
    const question: Question = event.option.value;
    this.router.navigate([`/question/`, question.id]);
    this.searchControl.setValue('');
  }

}
