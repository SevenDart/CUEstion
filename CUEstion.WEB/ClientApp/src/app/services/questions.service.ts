import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
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

  GetAllTags() {
    return this.http.get<string[]>(this._serverAddress + '/questions/tags');
  }

  CreateQuestion(header: string, description: string, tags: string[]) {
    const question = {
      header: header,
      text: description,
      tags: tags,
      user: {
        id: Number(localStorage.getItem('userId'))
      }
    };

    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    return this.http.put(this._serverAddress + '/questions', question, options);
  }

  UpdateQuestion(id: number, header: string, description: string, tags: string[]) {
    const question = {
      id: id,
      header: header,
      text: description,
      tags: tags,
      user: {
        id: Number(localStorage.getItem('userId'))
      }
    };

    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    return this.http.post(this._serverAddress + `/questions/${id}`, question, options);
  }
}
