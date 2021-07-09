import {Component, Output, EventEmitter} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {UsersService} from '../../../services/users.service';
import {catchError} from 'rxjs/operators';
import {of, throwError} from 'rxjs';
import {MatSnackBar} from '@angular/material/snack-bar';


@Component({
  selector: 'log-in',
  templateUrl: 'log-in.component.html',
  styleUrls: ['log-in.component.css'],
  providers: [UsersService]
})
export class LogInComponent {
  form: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)])
  });

  constructor(private userService: UsersService, private snackBar: MatSnackBar) {
  }

  @Output() closeRequest = new EventEmitter<number>();


  Login() {
    this.userService.login(this.form.get('email').value, this.form.get('password').value)
      .pipe(
        catchError(error => {
          if (error.status === 401) {
            const bar = this.snackBar.open('Incorrect e-mail or password.', 'Close', {
              panelClass: ['mat-toolbar', 'mat-warn'],
            });
            bar._dismissAfter(3 * 1000);
            bar.afterDismissed().subscribe(() => {
              this.form.get('password').setValue('');
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
          this.closeRequest.emit(data.id);
        }
      });
  }

}
