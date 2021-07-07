import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Observable} from 'rxjs';
import * as Url from 'url';
import {Injectable} from '@angular/core';
import {MatSnackBar} from '@angular/material/snack-bar';

@Injectable()
export class AuthorizationGuard implements CanActivate {

  constructor(private snackBar: MatSnackBar) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):
    Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      if (localStorage.getItem('token')) {
        return true;
      } else {
        const bar = this.snackBar.open('Please log in.', 'Close', {
          panelClass: ['mat-toolbar', 'mat-warn']
        });
        bar._dismissAfter(3 * 1000);
        return false;
      }
  }

}
