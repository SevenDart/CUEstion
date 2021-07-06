import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {User} from '../Models/User';

@Injectable()
export class UsersService {
  private _serverAddress = 'https://localhost:44376';

  constructor(private http: HttpClient) {
  }

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
}
