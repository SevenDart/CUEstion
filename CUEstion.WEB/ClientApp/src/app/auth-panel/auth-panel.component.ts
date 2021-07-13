import { Component } from '@angular/core';
import {User} from '../Models/User';
import {MatDialog} from '@angular/material/dialog';
import {SignDialogComponent} from './sign-dialog/sign-dialog.component';
import {Observable} from 'rxjs';
import {UsersService} from '../services/users.service';

@Component({
  selector: 'auth-panel',
  templateUrl: 'auth-panel.component.html',
  styleUrls: ['auth-panel.component.css'],
  providers: [UsersService]
})
export class AuthPanelComponent {
  user: Observable<User>;

  constructor(public dialog: MatDialog, private userService: UsersService) {
    this.checkId();
  }

  checkId() {
    const id = localStorage.getItem('userId');
    if (id) {
      this.user = this.userService.getUser(Number(id));
      this.user.subscribe((user: User) => {
        console.log(user);
      });
    }
  }

  openWindow() {
    const window = this.dialog.open(SignDialogComponent, {
      width: '500px'
    });
    window.afterClosed().subscribe(() => {
      this.checkId();
    });
  }

  logout() {
    this.user = null;
    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    localStorage.removeItem('role');
    UsersService.userId = null;
  }
}

