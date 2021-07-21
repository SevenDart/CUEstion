import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {User} from '../Models/User';
import {Question} from '../Models/Question';

@Injectable()
export class UsersService {

  constructor(private http: HttpClient) {
  }

  public static get isExpired() {
    return Date.parse(localStorage.getItem('expiration-time')) < Date.now();
  }

  public static get IsAdmin() {
    return localStorage.getItem('role') === 'admin';
  }

  public static userId: number = localStorage.getItem('userId') ? Number(localStorage.getItem('userId')) : null;
  private _serverAddress = 'https://localhost:44376';

  login(email: string, password: string) {
    const authData = {
      email: email,
      password: password
    };
    return this.http.put(this._serverAddress + '/users/login', authData);
  }

  getUser(userId: number) {
    return this.http.get<User>(this._serverAddress + '/users/' + userId);
  }

  register(email: string, password: string) {
    const authData = {
      email: email,
      password: password
    };
    return this.http.post(this._serverAddress + '/users/register', authData);
  }

  getCreatedQuestions(userId: number) {
    return this.http.get<Question[]>(this._serverAddress + `/users/${userId}/questions`);
  }

  getSubscribedQuestions() {
    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    return this.http.get<Question[]>(this._serverAddress + `/users/subscribed`, options);
  }
}
