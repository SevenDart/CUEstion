import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Question} from '../Models/Question';

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
}
