import {Component, ElementRef, ViewChild} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {Observable, of, throwError} from 'rxjs';
import {QuestionsService} from '../services/questions.service';
import {catchError, map, startWith} from 'rxjs/operators';
import {MatChipInputEvent} from '@angular/material/chips';
import {MatAutocompleteSelectedEvent} from '@angular/material/autocomplete';
import {MatSnackBar} from '@angular/material/snack-bar';
import {HttpErrorResponse} from '@angular/common/http';
import {Router} from '@angular/router';

@Component({
  selector: 'create-question',
  templateUrl: 'create-question.component.html',
  styleUrls: ['create-question.component.css'],
  providers: [QuestionsService]
})
export class CreateQuestionComponent {
  questionForm: FormGroup = new FormGroup({
    header: new FormControl('', [Validators.required, Validators.minLength(15), Validators.maxLength(50)]),
    description: new FormControl('', [Validators.required, Validators.minLength(30), Validators.maxLength(500)]),
    tagSearch: new FormControl()
  });

  filteredTags: Observable<string[]>;
  allTags: string[] = [];
  selectedTags: string[] = [];

  @ViewChild('tagInput') tagInput: ElementRef<HTMLInputElement>;

  constructor(private questionService: QuestionsService, private snackBar: MatSnackBar, private router: Router) {
    this.questionService.GetAllTags().subscribe((tags: string[]) => {
      this.allTags = tags;
    });
    this.filteredTags = this.questionForm.get('tagSearch').valueChanges.pipe(
      startWith(null),
      map((tag: string | null) => tag ? this._filter(tag) : this.allTags.slice()));
  }

  addTag(event: MatChipInputEvent) {
    if (event.value) {
      this.selectedTags.push(event.value);
    }
    event.chipInput.clear();
    this.questionForm.get('tagSearch').setValue(null);
  }

  remove(tag: string): void {
    const index = this.selectedTags.indexOf(tag);

    if (index >= 0) {
      this.selectedTags.splice(index, 1);
    }
  }

  createQuestion() {
    this.questionService.CreateQuestion(this.questionForm.get('header').value,
                                        this.questionForm.get('description').value,
                                        this.selectedTags).pipe(
      catchError(catchError((error: HttpErrorResponse) => {
        if (error.status === 500) {
          const bar = this.snackBar.open('Something is wrong, please, try again later.', 'Close', {
            panelClass: ['mat-toolbar', 'mat-warn'],
          });
          bar._dismissAfter(3 * 1000);
          return of(null);
        }
        if (error.status === 0) {
          this.snackBar.open('Something is wrong, try, please, later.', '', {
            panelClass: ['mat-toolbar', 'mat-warn']
          });
          return throwError(() => {
            return new Error('something is wrong');
          });
        }
      })
    )
    ).subscribe((data: any) => {
      if (data) {
        this.router.navigate(['/question/', data]);
      }
    });
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    this.selectedTags.push(event.option.viewValue);
    this.tagInput.nativeElement.value = '';
    this.questionForm.get('tagSearch').setValue(null);
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.allTags.filter(tag => tag.toLowerCase().includes(filterValue));
  }
}
