import {Component} from '@angular/core';
import {UsersService} from '../services/users.service';
import {QuestionsService} from '../services/questions.service';
import {Observable, of, throwError} from 'rxjs';
import {FormControl, Validators} from '@angular/forms';
import {catchError, map, startWith} from 'rxjs/operators';
import {HttpErrorResponse} from '@angular/common/http';
import {MatSnackBar} from '@angular/material/snack-bar';
import {MatDialog} from '@angular/material/dialog';
import {ConfirmDialogComponent} from '../confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'tag-editor',
  templateUrl: 'tag-editor.component.html',
  styleUrls: ['tag-editor.component.css'],
  providers: [QuestionsService]
})
export class TagEditorComponent {
  tagSearch = new FormControl('');
  newTagControl = new FormControl('', [Validators.maxLength(10)]);

  allTags: string[] = [];
  tagsForms = [];
  filteredTags: Observable<string[]>;

  constructor(private questionsService: QuestionsService, private snackBar: MatSnackBar, private dialog: MatDialog) {
    questionsService.GetAllTags().subscribe((tags: string[]) => {
      this.allTags = tags;
      for (const tag of tags) {
        this.tagsForms[tag] = {
          isEditing: false,
          editControl: new FormControl(tag, [Validators.maxLength(10)])
        };
      }
      this.filteredTags = this.tagSearch.valueChanges.pipe(
        startWith(null),
        map((tag: string | null) => tag ? this._filter(tag) : this.allTags.slice()));
    });
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.allTags.filter(tag => tag.toLowerCase().startsWith(filterValue));
  }

  deleteTag(tag: string) {
    const dialog = this.dialog.open(ConfirmDialogComponent, {
      data: {
        header: 'Do you want to delete this tag?'
      }
    });
    dialog.afterClosed().subscribe((result) => {
      if (result === true) {
        this.questionsService.DeleteTag(tag).pipe(
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
            this.allTags.splice(this.allTags.indexOf(tag), 1);
            this.filteredTags = this.tagSearch.valueChanges.pipe(
              startWith(null),
              map((valueTag: string | null) => valueTag ? this._filter(valueTag) : this.allTags.slice()));
          }
        });
      }
    });
  }

  createTag(tag: string) {
    this.questionsService.CreateTag(tag).pipe(
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
        this.allTags.push(tag);
        this.tagsForms[tag] = {
          isEditing: false,
          editControl: new FormControl(tag, [Validators.maxLength(10)])
        };
        this.newTagControl.setValue('');
        this.filteredTags = this.tagSearch.valueChanges.pipe(
          startWith(null),
          map((valueTag: string | null) => valueTag ? this._filter(valueTag) : this.allTags.slice()));
      }
    });
  }

  updateTag(oldTag: string, newTag: string) {
    this.questionsService.UpdateTag(oldTag, newTag).pipe(
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
        this.allTags[this.allTags.indexOf(oldTag)] = newTag;
        this.tagsForms[newTag] = this.tagsForms[oldTag];
        this.tagsForms.splice(this.tagsForms.indexOf(oldTag), 1);
        this.tagsForms[newTag].isEditing = false;
        this.filteredTags = this.tagSearch.valueChanges.pipe(
          startWith(null),
          map((valueTag: string | null) => valueTag ? this._filter(valueTag) : this.allTags.slice()));
      }
    });
  }

}
