import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Question} from '../Models/Question';
import {Answer} from '../Models/Answer';
import {AppComment} from '../Models/AppComment';

@Injectable()
export class QuestionsService {
  private _serverAddress = 'https://localhost:44376';

  constructor(private http: HttpClient) {
  }

  SearchQuestions(query: string) {
    return this.http.get<Question[]>(this._serverAddress + '/questions/search' + `?query=${query}`);
  }

  GetHotQuestions(count: number) {
    return this.http.get<Question[]>(this._serverAddress + '/questions/hot' + `?count=${count}`);
  }

  GetQuestion(id: number) {
    return this.http.get<Question>(this._serverAddress + `/questions/${id}`);
  }

  GetAnswersForQuestion(id: number) {
    return this.http.get<Answer[]>(this._serverAddress + `/questions/${id}/answers`);
  }

  GetCommentsForQuestion(id: number) {
    return this.http.get<AppComment[]>(this._serverAddress + `/questions/${id}/comments`);
  }

  GetCommentsForAnswer(questionId: number, answerId: number) {
    return this.http.get<AppComment[]>(this._serverAddress + `/questions/${questionId}/answers/${answerId}/comments`);
  }
}
