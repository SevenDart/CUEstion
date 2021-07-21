import {Component, OnInit} from '@angular/core';
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

  get isAdmin() {
    return UsersService.IsAdmin;
  }

  get isOwner() {
    return UsersService.userId === this.question.user.id;
  }
  questionVotingExpanded = false;
  question: Question;

  questionComments: Observable<AppComment[]>;
  answersObs: Observable<Answer[]>;
  answerComments: Observable<AppComment[]>[] = [];
  answersForms = [];
  commentForms = [];

  answerControl: FormControl = new FormControl('',
    [Validators.required, Validators.minLength(15), Validators.maxLength(2000)]);
  commentControl: FormControl = new FormControl('',
    [Validators.required, Validators.minLength(10), Validators.maxLength(200)]);

  updateAnswers() {
    this.answersObs = this.questionService.GetAnswersForQuestion(this.question.id);
    this.answersObs.subscribe((answers: Answer[]) => {
      for (const answer of answers) {
        this.answersForms[answer.id] = {
          get isOwner() {
            return UsersService.userId === answer.user.id;
          },
          rate: answer.rate,
          isEditing: false,
          votingExpanded: false,
          editFormControl: new FormControl('',
            [Validators.required, Validators.maxLength(500 - answer.text.length - 8)]),
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
          rate: comment.rate,
          isEditing: false,
          votingExpanded: false,
          editFormControl: new FormControl('',
            [Validators.required, Validators.maxLength(200 - comment.text.length - 8)]),
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
          rate: comment.rate,
          isEditing: false,
          votingExpanded: false,
          editFormControl: new FormControl('',
            [Validators.required, Validators.maxLength(200 - comment.text.length)]),
        };
      }
    });
  }

  updateComment(questionId: number, answerId: number, commentId: number, originText: string) {
    this.questionService.UpdateComment(this.question.id, answerId, commentId,
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
        this.questionService.DeleteComment(this.question.id, answerId, commentId).pipe(
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
    this.questionService.UpdateAnswer(this.question.id, answerId,
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
          this.updateAnswers();
          this.answersForms[answerId].editFormControl.reset();
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
        this.questionService.DeleteAnswer(this.question.id, answerId).pipe(
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
            this.updateAnswers();
          }
        });
      }
    });
  }

  addComment(questionId: number, answerId: number, commentControl: FormControl) {
    this.questionService.CreateComment(this.question.id, answerId, commentControl.value).pipe(
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
      const questionObs = this.questionService.GetQuestion(id);
      questionObs.subscribe((question: Question) => {
        this.question = question;
        this.updateAnswers();
      });
      this.updateQuestionComments(id);
    });
  }

  addAnswer() {
    this.questionService.AddAnswer(this.question.id, this.answerControl.value).pipe(
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
        this.updateAnswers();
        this.answerControl.reset();
      }
    });
  }

  upvoteForElement(questionId: number, answerId: number, commentId: number) {
    const type = (commentId !== null) ? 'comment' : (answerId !== null) ? 'answer' : 'question';
    const result: Observable<any> = this.questionService.UpvoteElement(this.question.id, answerId, commentId);
    result.pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 0) {
          this.snackBar.open('Something is wrong, try, please, later.', '', {
            panelClass: ['mat-toolbar', 'mat-warn']
          });
          return throwError(() => {
            return new Error('something is wrong');
          });
        }
        let message: string;
        switch (error.status) {
          case 401:
            message = `Please, log in to upvote this ${type}.`;
            break;
          case 500:
            message = `You have already upvoted this ${type}.`;
            break;
        }
        const bar = this.snackBar.open(message, 'Close', {
          panelClass: ['mat-toolbar', 'mat-warn'],
        });
        bar._dismissAfter(3 * 1000);
        return of([]);
      })
    ).subscribe((data) => {
      if (data === null) {
        switch (type) {
          case 'question':
            this.question.rate++;
            break;
          case 'answer':
            this.answersForms[answerId].rate++;
            break;
          case 'comment':
            this.commentForms[commentId].rate++;
            break;
        }
      }
    });
  }

  downvoteForElement(questionId: number, answerId: number, commentId: number) {
    const type = (commentId !== null) ? 'comment' : (answerId !== null) ? 'answer' : 'question';
    const result = this.questionService.DownvoteElement(questionId, answerId, commentId);
    result.pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 0) {
          this.snackBar.open('Something is wrong, try, please, later.', '', {
            panelClass: ['mat-toolbar', 'mat-warn']
          });
          return throwError(() => {
            return new Error('something is wrong');
          });
        }
        let message: string;
        switch (error.status) {
          case 401:
            message = `Please, log in to downvote this ${type}.`;
            break;
          case 500:
            message = `You have already downvoted this ${type}.`;
            break;
        }
        const bar = this.snackBar.open(message, 'Close', {
          panelClass: ['mat-toolbar', 'mat-warn'],
        });
        bar._dismissAfter(3 * 1000);
        return of([]);
      })
    ).subscribe((data) => {
      if (data === null) {
        switch (type) {
          case 'question':
            this.question.rate--;
            break;
          case 'answer':
            this.answersForms[answerId].rate--;
            break;
          case 'comment':
            this.commentForms[commentId].rate--;
            break;
        }
      }
    });
  }

}
