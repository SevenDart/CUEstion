import {Component, ElementRef, EventEmitter, Input, Output, ViewChild} from '@angular/core';
import {MatChipInputEvent} from '@angular/material/chips';
import {MatAutocompleteSelectedEvent} from '@angular/material/autocomplete';
import {Observable} from 'rxjs';
import {FormControl} from '@angular/forms';
import {map, startWith} from 'rxjs/operators';
import {QuestionsService} from '../services/questions.service';

@Component({
  selector: 'tag-select',
  templateUrl: 'tag-select.component.html',
  styleUrls: ['tag-select.component.css'],
  providers: [QuestionsService]
})
export class TagSelectComponent {
  tagSearch = new FormControl();

  filteredTags: Observable<string[]>;
  allTags: string[] = [];
  @Input() selectedTags: string[] = [];

  @Output() focus = new EventEmitter<boolean>();

  constructor(private questionsService: QuestionsService) {
    this.questionsService.GetAllTags().subscribe((tags: string[]) => {
      this.allTags = tags;
    });
    this.filteredTags = this.tagSearch.valueChanges.pipe(
      startWith(null),
      map((tag: string | null) => tag ? this._filter(tag) : this.allTags.slice()));
  }

  @ViewChild('tagInput') tagInput: ElementRef<HTMLInputElement>;

  addTag(event: MatChipInputEvent) {
    if (!this.selectedTags.includes(event.value)) {
      if (event.value) {
        this.selectedTags.push(event.value);
      }
      event.chipInput.clear();
      this.tagSearch.setValue(null);
    }
  }

  remove(tag: string): void {
    const index = this.selectedTags.indexOf(tag);

    if (index >= 0) {
      this.selectedTags.splice(index, 1);
    }
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    if (!this.selectedTags.includes(event.option.value)) {
      this.selectedTags.push(event.option.viewValue);
    }
    this.tagInput.nativeElement.value = '';
    this.tagSearch.setValue(null);
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.allTags.filter(tag => tag.toLowerCase().startsWith(filterValue));
  }
}
