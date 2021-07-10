import {Component, HostListener, OnInit} from '@angular/core';
import {Question} from '../Models/Question';
import {Observable, of, throwError} from 'rxjs';
import {Answer} from '../Models/Answer';
import {QuestionsService} from '../services/questions.service';
import {ActivatedRoute, ParamMap, Router} from '@angular/router';
import {AppComment} from '../Models/AppComment';
import {FormControl, Validators} from '@angular/forms';
import {UsersService} from '../services/users.service';
import {catchError} from 'rxjs/operators';
import {HttpErrorResponse} from '@angular/common/http';
import {MatSnackBar} from '@angular/material/snack-bar';
import {ConfirmDialogComponent} from '../confirm-dialog/confirm-dialog.component';
import {MatDialog} from '@angular/material/dialog';


@Component({
  selector: 'question-page',
  templateUrl: 'question-page.component.html',
  styleUrls: ['question-page.component.css'],
  providers: [QuestionsService]
})
export class QuestionPageComponent implements OnInit {

  constructor(private questionService: QuestionsService,
              private activatedRoute: ActivatedRoute,
              private router: Router, private snackBar: MatSnackBar, private dialog: MatDialog) {
  }

  get isAuthed() {
    return UsersService.userId !== null;
  }

  get isOwner() {
    return UsersService.userId === this.questionOwnerId;
  }
  questionOwnerId: number;
  question: Observable<Question>;
  questionComments: Observable<AppComment[]>;
  answers: Observable<Answer[]>;
  answerComments: Observable<AppComment[]>[] = [];
  answersForms = [];
  commentForms = [];

  answerControl: FormControl = new FormControl('',
    [Validators.required, Validators.minLength(15), Validators.maxLength(500)]);
  commentControl: FormControl = new FormControl('',
    [Validators.required, Validators.minLength(10), Validators.maxLength(200)]);

  updateAnswers(questionId: number) {
    this.answers = this.questionService.GetAnswersForQuestion(questionId);
    this.answers.subscribe((answers: Answer[]) => {
      for (const answer of answers) {
        this.answersForms[answer.id] = {
          get isOwner() {
            return UsersService.userId === answer.user.id;
          },
          isEditing: false,
          editFormControl: new FormControl('',
            [Validators.required, Validators.maxLength(500 - answer.text.length)]),
          addCommentFormControl: new FormControl('',
            [Validators.required, Validators.minLength(10), Validators.maxLength(200)])
        };
        this.updateAnswerComments(answer.id);
      }
    });
  }

  updateQuestionComments(questionId: number) {
    this.questionComments = this.questionService.GetCommentsForQuestion(questionId);
    this.questionComments.subscribe((comments: AppComment[]) => {
      for (const comment of comments) {
        this.commentForms[comment.id] = {
          get isOwner() {
            return UsersService.userId === comment.user.id;
          },
          isEditing: false,
          editFormControl: new FormControl('',
            [Validators.required, Validators.maxLength(200 - comment.text.length)]),
        };
      }
    });
  }

  updateAnswerComments(answerId: number) {
    this.answerComments[answerId] = this.questionService.GetCommentsForAnswer(0, answerId);
    this.answerComments[answerId].subscribe((comments: AppComment[]) => {
      for (const comment of comments) {
        this.commentForms[comment.id] = {
          get isOwner() {
            return UsersService.userId === comment.user.id;
          },
          isEditing: false,
          editFormControl: new FormControl('',
            [Validators.required, Validators.maxLength(200 - comment.text.length)]),
        };
      }
    });
  }

  updateComment(questionId: number, answerId: number, commentId: number, originText: string) {
    this.questionService.UpdateComment(questionId, answerId, commentId,
      (originText + (this.commentForms[commentId].editFormControl.value.length > 0
        ? ' [UPD.] ' + this.commentForms[commentId].editFormControl.value
        : '')))
      .pipe(
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
          if (questionId != null) {
            this.updateQuestionComments(questionId);
          } else {
            this.updateAnswerComments(answerId);
          }
        }
      }
    );
  }

  deleteComment(questionId: number, answerId: number, commentId: number) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        header: 'Do you want to delete this comment?'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.questionService.DeleteComment(questionId, answerId, commentId).pipe(
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
            if (questionId != null) {
              this.updateQuestionComments(questionId);
            } else {
              this.updateAnswerComments(answerId);
            }
          }
        });
      }
    });
  }


  updateAnswer(answerId: number, originText: string) {
    this.questionService.UpdateAnswer(answerId,
      (originText + (this.answersForms[answerId].editFormControl.value.length > 0
        ? ' [UPD.] ' + this.answersForms[answerId].editFormControl.value
        : '')))
      .pipe(
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
          this.question.subscribe((question: Question) => {
            this.updateAnswers(question.id);
            this.answersForms[answerId].editFormControl.reset();
          });
        }
      }
    );
  }

  deleteAnswer(answerId: number) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        header: 'Do you want to delete this answer?'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.questionService.DeleteAnswer(answerId).pipe(
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
            this.question.subscribe((question: Question) => {
              this.updateAnswers(question.id);
            });
          }
        });
      }
    });
  }

  addComment(questionId: number, answerId: number, commentControl: FormControl) {
    this.questionService.CreateComment(questionId, answerId, commentControl.value).pipe(
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
        if (questionId != null) {
          this.updateQuestionComments(questionId);
        } else {
          this.updateAnswerComments(answerId);
        }
        commentControl.reset();
      }
    });
  }

  ngOnInit() {
    this.activatedRoute.paramMap.subscribe((map: ParamMap) => {
      const id: number = Number(map.get('id'));
      this.question = this.questionService.GetQuestion(id);
      this.question.subscribe((question: Question) => {
        this.questionOwnerId = question.user.id;
      });
      this.updateQuestionComments(id);
      this.updateAnswers(id);
    });
  }

  addAnswer() {
    this.question.subscribe((question: Question) => {
      this.questionService.AddAnswer(question.id, this.answerControl.value).pipe(
        catchError(catchError((error: HttpErrorResponse) => {
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
      )).subscribe((data) => {
        if (data === null) {
          this.updateAnswers(question.id);
          this.answerControl.reset();
        }
      });
    });
  }
}
