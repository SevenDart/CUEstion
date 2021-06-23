import { Component, OnInit } from '@angular/core';
import {FormControl} from '@angular/forms';
import {Observable} from 'rxjs';
import {debounceTime, map, startWith} from 'rxjs/operators';

@Component({
  selector: 'search-field',
  templateUrl: './search-field.component.html',
  styleUrls: ['./search-field.component.css']
})
export class SearchFieldComponent implements OnInit {
  searchControl = new FormControl();
  results: Observable<String[]>;

  constructor() {
  }

  ngOnInit() {
    this.results = this.searchControl.valueChanges.pipe(
      debounceTime(1000),
      startWith(''),
      map(value => this.Search(value))
    );
  }

  private Search(query: string): string[] {
    const example: string[] = ['abcd', 'dbce', 'addd'];

    return example.filter(str => str.includes(query));
  }

}
