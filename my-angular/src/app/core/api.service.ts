import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../model/user.model';
import { Observable } from 'rxjs';
import { Constants } from '../constants';

@Injectable()
export class ApiService {
  constructor(private http: HttpClient) { }

  baseUrl = `${Constants.HOME_URL}/api/users`;

  getUsers() {
    return this.http.get<User[]>(this.baseUrl);
  }

  getUser(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/edit/${id}`);
  }

  updateUser(value: any): Observable<Object> {
    return this.http.put(`${this.baseUrl}/save`, value);
  }
}
