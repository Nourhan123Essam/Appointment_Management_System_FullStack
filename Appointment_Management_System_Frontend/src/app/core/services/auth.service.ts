import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.ApiBaseUrl}/Authentication`;

  constructor(private http: HttpClient, private router: Router) {}

  login(email: string, password: string, role: string) {
    return this.http.post<any>(`${this.apiUrl}/login`, { email, password });
  }

  register(data: any) {
    return this.http.post<any>(`${this.apiUrl}/register`, data);
  }
  verifyCapture( recaptchaToken: any){
    return this.http.post<any>(`${this.apiUrl}/verify-captcha`, recaptchaToken);
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUserRole(): string | null {
    return localStorage.getItem('role');
  }

  hasRole(role: string): boolean {
    return this.getUserRole() === role;
  }
}
