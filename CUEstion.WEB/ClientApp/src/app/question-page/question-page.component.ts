import {Component, OnInit} from '@angular/core';
import {Question} from '../Models/Question';
import {Observable, of} from 'rxjs';
import {Answer} from '../Models/Answer';
import {QuestionsService} from '../services/questions.service';
import {ActivatedRoute, ParamMap} from '@angular/router';
import {AppComment} from '../Models/AppComment';
import {Comment} from '@angular/compiler';
import {switchMap} from 'rxjs/operators';


@Component({
  selector: 'question-page',
  templateUrl: 'question-page.component.html',
  styleUrls: ['question-page.component.css'],
  providers: [QuestionsService]
})
export class QuestionPageComponent implements OnInit {
  question: Observable<Question>;
  questionComments: Observable<AppComment[]>;
  answers: Observable<Answer[]>;
  answerComments: Observable<AppComment[]>[];

  constructor(private questionService: QuestionsService, private activatedRoute: ActivatedRoute) {
  }

  ngOnInit() {
    this.activatedRoute.paramMap.subscribe((map: ParamMap) => {
      const id: number = Number(map.get('id'));
      this.question = this.questionService.GetQuestion(id);
      this.questionComments = this.questionService.GetCommentsForQuestion(id);
      this.answers = this.questionService.GetAnswersForQuestion(id);
      this.answers.subscribe((answers: Answer[]) => {
        for (const answer of answers) {
          this.answerComments[answer.id] = this.questionService.GetCommentsForAnswer(id, answer.id);
        }
      });
    });
  }
}
