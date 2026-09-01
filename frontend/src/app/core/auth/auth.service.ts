import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { AuthResponse, UserDto} from './auth.models';
import { environment } from '../../../environments/environment';

const TOKEN_KEY = 'rm_access_token';
const USER_KEY = 'rm_user';

@Injectable({
  providedIn: 'root' // one for the all app
})
export class AuthService {
  readonly currentUser = signal<UserDto | null>(null);

  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;

  constructor() {
    this.restoreFromStorage();
  }

  register(body: {
    email: string;
    firstName: string;
    lastName: string;
    phone?: string | null;
  }): Observable<UserDto>{

    return this.http.post<UserDto>(
      this.baseUrl + '/api/auth/register',
      body
    )
  }

  requestCode(email: string): Observable<{ message: string }> {
    const body = {email: email.trim()};

    return this.http.post<{ message: string }>(
      `${this.baseUrl}/api/auth/request-code`,
      body
    );
  }

  verifyCode(email: string, code: string): Observable<AuthResponse> {
    const body = {
      email: email.trim(),
      code: code.trim()
    };

    const url = `${this.baseUrl}/api/auth/verify-code`;

    return this.http.post<AuthResponse>(url, body).pipe(
      tap((auth) => {
          this.setSession(auth);
      }),
    )
  }

  // JWT or null (nor logged in)
  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  // read
  isAuthenticated(): boolean{
    return this.getToken() !== null;
  }

  // write to the LS
  setSession(auth : AuthResponse) {
    localStorage.setItem(TOKEN_KEY, auth.accessToken);
    localStorage.setItem(USER_KEY, JSON.stringify(auth.user));
    this.currentUser.set(auth.user);
  }

  updateCurrentUser(user: UserDto) {
    this.currentUser.set(user);
  }

  // logout and delete all info
  logout(){
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.currentUser.set(null);
  }

  // restoring after F5
  private restoreFromStorage() {
    const token = localStorage.getItem(TOKEN_KEY);
    const rawUser = localStorage.getItem(USER_KEY);

    if (!token || !rawUser) {
      this.logout();
      return;
    }

    try {
      const user = JSON.parse(rawUser) as UserDto;
      this.currentUser.set(user);
    } catch {
      this.logout()
    }
  }
}
