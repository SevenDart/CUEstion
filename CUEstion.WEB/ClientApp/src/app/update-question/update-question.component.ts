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
import {MatDialog} from '@angular/material/dialog';
import {ConfirmDialogComponent} from '../confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'update-question',
  templateUrl: 'update-question.component.html',
  styleUrls: ['update-question.component.css'],
  providers: [QuestionsService]
})
export class UpdateQuestionComponent {
  questionForm: FormGroup = new FormGroup({
    header: new FormControl(''),
    description: new FormControl('')
  });

  question: Observable<Question>;
  selectedTags: string[] = [];

  constructor(private questionService: QuestionsService, private snackBar: MatSnackBar, private router: Router,
              private activatedRoute: ActivatedRoute, private dialog: MatDialog) {
    this.activatedRoute.paramMap.subscribe((paramMap: ParamMap) => {
      const id: number = Number(paramMap.get('id'));
      this.question = this.questionService.GetQuestion(id);
      this.question.subscribe((question: Question) => {
        this.selectedTags = question.tags;
        this.questionForm.get('description').setValidators([Validators.maxLength(500 - question.text.length - 8)]);
      });
    });
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

  deleteQuestion(questionId: number) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        header: 'Do you want to delete this question?'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.questionService.DeleteQuestion(questionId).pipe(
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
        ).subscribe( (data) => {
          if (data === null) {
            this.router.navigate(['home']);
          }
        });
      }
    });
  }
}
