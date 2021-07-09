import {Component, ElementRef, Input, ViewChild} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {Observable, of, throwError} from 'rxjs';
import {QuestionsService} from '../services/questions.service';
import {catchError, map, startWith} from 'rxjs/operators';
import {MatChipInputEvent} from '@angular/material/chips';
import {MatAutocompleteSelectedEvent} from '@angular/material/autocomplete';
import {MatSnackBar} from '@angular/material/snack-bar';
import {HttpErrorResponse} from '@angular/common/http';
import {ActivatedRoute, ParamMap, Router} from '@angular/router';
import {Question} from '../Models/Question';
import {Answer} from '../Models/Answer';

@Component({
  selector: 'update-question',
  templateUrl: 'update-question.component.html',
  styleUrls: ['update-question.component.css'],
  providers: [QuestionsService]
})
export class UpdateQuestionComponent {
  questionForm: FormGroup = new FormGroup({
    header: new FormControl(''),
    description: new FormControl(''),
    tagSearch: new FormControl()
  });

  question: Observable<Question>;
  filteredTags: Observable<string[]>;
  allTags: string[] = [];
  selectedTags: string[] = [];

  @ViewChild('tagInput') tagInput: ElementRef<HTMLInputElement>;

  constructor(private questionService: QuestionsService, private snackBar: MatSnackBar, private router: Router,
              private activatedRoute: ActivatedRoute) {
    this.activatedRoute.paramMap.subscribe((paramMap: ParamMap) => {
      const id: number = Number(paramMap.get('id'));
      this.question = this.questionService.GetQuestion(id);
      this.question.subscribe((question: Question) => {
        this.selectedTags = question.tags;
        this.questionForm.get('description').setValidators([Validators.maxLength(500 - question.text.length)]);
      });
    });

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

  updateQuestion() {
    this.question.subscribe((question: Question) => {
      this.questionService.UpdateQuestion(question.id,
        question.text + (this.questionForm.get('description').value.length > 0
          ? ' [UPD.] ' + this.questionForm.get('description').value
          : ''),
        this.selectedTags).pipe(
          catchError((error: HttpErrorResponse) => {
            if (error.status === 500) {
              const bar = this.snackBar.open('Something is wrong, please, try again later.', 'Close', {
                panelClass: ['mat-toolbar', 'mat-warn'],
              });
              bar._dismissAfter(3 * 1000);
              return of([]);
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
      ).subscribe((data) => {
        if (data === null) {
          this.router.navigate(['/question/', question.id]);
        }
      });
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
