import {Component, ElementRef, Input, OnInit, ViewChild} from '@angular/core';
import {FormControl} from '@angular/forms';
import {Observable, of, pipe} from 'rxjs';
import {debounceTime, distinctUntilChanged, map, startWith, switchMap} from 'rxjs/operators';
import {QuestionsService} from '../services/questions.service';
import {Question} from '../Models/Question';
import {MatAutocompleteSelectedEvent} from '@angular/material/autocomplete';
import {Router} from '@angular/router';
import {MatChipInputEvent} from '@angular/material/chips';

@Component({
  selector: 'search-field',
  templateUrl: './search-field.component.html',
  styleUrls: ['./search-field.component.css'],
  providers: [QuestionsService]
})
export class SearchFieldComponent implements OnInit {
  searchControl = new FormControl();
  tagSearch = new FormControl();
  results: Observable<Question[]>;

  @Input() isExpandable = true;

  tagPanelExpanded = false;

  @ViewChild('tagInput') tagInput: ElementRef<HTMLInputElement>;

  filteredTags: Observable<string[]>;
  allTags: string[] = [];
  selectedTags: string[] = [];

  constructor(private questionsService: QuestionsService, private router: Router) {
    this.questionsService.GetAllTags().subscribe((tags: string[]) => {
      this.allTags = tags;
    });
    this.filteredTags = this.tagSearch.valueChanges.pipe(
      startWith(null),
      map((tag: string | null) => tag ? this._filter(tag) : this.allTags.slice()));
  }

  addTag(event: MatChipInputEvent) {
    if (event.value) {
      this.selectedTags.push(event.value);
    }
    event.chipInput.clear();
    this.tagSearch.setValue(null);
  }

  remove(tag: string): void {
    const index = this.selectedTags.indexOf(tag);

    if (index >= 0) {
      this.selectedTags.splice(index, 1);
    }
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    this.selectedTags.push(event.option.viewValue);
    this.tagInput.nativeElement.value = '';
    this.tagSearch.setValue(null);
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.allTags.filter(tag => tag.toLowerCase().includes(filterValue));
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
