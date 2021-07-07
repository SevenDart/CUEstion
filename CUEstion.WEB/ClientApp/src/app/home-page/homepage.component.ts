import {Component, OnInit} from '@angular/core';
import {QuestionsService} from '../services/questions.service';
import {Observable} from 'rxjs';
import {Question} from '../Models/Question';
import {MatSnackBar} from '@angular/material/snack-bar';
import {ActivatedRoute} from '@angular/router';


@Component({
  selector: 'home-page',
  templateUrl: './homepage.component.html',
  styleUrls: [ './homepage.component.css' ],
  providers: [QuestionsService]
})
export class HomePageComponent implements OnInit {
  popularQuestions: Observable<Question[]>;
  popularQuestionsCount = 10;

  displayedColumns: string[] = ['rate', 'header', 'last-update'];

  constructor(private questionService: QuestionsService) {
  }

  ngOnInit(): void {
    this.popularQuestions = this.questionService.GetHotQuestions(this.popularQuestionsCount);
  }


}
