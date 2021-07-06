import {Component, Inject, Input} from '@angular/core';
import {MAT_SNACK_BAR_DATA} from '@angular/material/snack-bar';

@Component({
  selector: 'snack-bar-content',
  templateUrl: 'snack-bar-content.component.html',
  styleUrls: ['snack-bar-content.component.css'],
})
export class SnackBarContentComponent {
  constructor(@Inject(MAT_SNACK_BAR_DATA) public data) {
  }
}
