import {Component} from '@angular/core';
import {MatDialogRef} from '@angular/material/dialog';

@Component({
  selector: 'sign-dialog',
  templateUrl: 'sign-dialog.component.html',
  styleUrls: ['sign-dialog.component.css']
})
export class SignDialogComponent {

  constructor(
    public dialogRef: MatDialogRef<SignDialogComponent>) {
  }

  closeRequest() {
    this.dialogRef.close();
  }

}
