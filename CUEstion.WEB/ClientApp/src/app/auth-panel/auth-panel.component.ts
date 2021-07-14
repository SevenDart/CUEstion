import { Component } from '@angular/core';
import {User} from '../Models/User';
import {MatDialog} from '@angular/material/dialog';
import {SignDialogComponent} from './sign-dialog/sign-dialog.component';
import {UsersService} from '../services/users.service';
import {TagEditorComponent} from '../tag-editor/tag-editor.component';

@Component({
  selector: 'auth-panel',
  templateUrl: 'auth-panel.component.html',
  styleUrls: ['auth-panel.component.css'],
  providers: [UsersService]
})
export class AuthPanelComponent {
  user: User;

  constructor(public dialog: MatDialog, private userService: UsersService) {
    this.checkId();
  }

  get isAdmin() {
    return UsersService.IsAdmin;
  }

  refreshUser() {
    const id = localStorage.getItem('userId');
    const userObs = this.userService.getUser(Number(id));
    userObs.subscribe((user: User) => {
      this.user.rate = user.rate;
    });
  }

  checkId() {
    const id = localStorage.getItem('userId');
    if (id) {
      const userObs = this.userService.getUser(Number(id));
      userObs.subscribe((user: User) => {
        this.user = user;
      });
    }
  }

  openLoginWindow() {
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

  openTagsWindow() {
    this.dialog.open(TagEditorComponent, {
      width: '40%'
    });
  }
}

