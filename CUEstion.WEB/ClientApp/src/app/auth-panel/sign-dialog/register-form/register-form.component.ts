import {Component, Output, EventEmitter} from '@angular/core';
import {AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators} from '@angular/forms';
import {UsersService} from '../../../services/users.service';
import {catchError} from 'rxjs/operators';
import {HttpErrorResponse} from '@angular/common/http';
import {of, throwError} from 'rxjs';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'register-form',
  templateUrl: 'register-form.component.html',
  styleUrls: ['register-form.component.css'],
  providers: [UsersService]
})
export class RegisterFormComponent {
  form: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)]),
    confirmPassword: new FormControl('')
  });

  constructor(private usersService: UsersService, private snackBar: MatSnackBar) {
    this.form.get('confirmPassword').setValidators([Validators.required, confirmValidator(this.form.get('password'))]);
  }

  @Output() closeRequest = new EventEmitter();


  Register() {
    this.usersService.register(this.form.get('email').value, this.form.get('password').value)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          if (error.status === 500) {
            const bar = this.snackBar.open('Such user already exists, try other e-mail.', 'Close', {
              panelClass: ['mat-toolbar', 'mat-warn'],
            });
            bar._dismissAfter(3 * 1000);
            bar.afterDismissed().subscribe(() => {
              this.form.get('email').setValue('');
              this.form.get('password').setValue('');
              this.form.get('confirmPassword').setValue('');
            });
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
      .subscribe((data: any) => {
        if (data) {
          UsersService.userId = data.id;
          localStorage.setItem('token', data.token);
          localStorage.setItem('role', data.role);
          localStorage.setItem('userId', data.id);
          this.closeRequest.emit();
        }
      });
  }
}

export function confirmValidator(matchingForm: AbstractControl): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const isEqual = matchingForm.value === control.value;
    return isEqual ? null : {incorrectConfirmField: control.value};
  };
}
