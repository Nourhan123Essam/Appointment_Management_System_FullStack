import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment.development';
import { Observable } from 'rxjs';
import { LoginResult } from '../Interfaces/LoginResult';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.ApiBaseUrl}/Authentication`;
  private userRoles: string[] | null = null; // Cache roles in memory

  constructor(private http: HttpClient, private router: Router) {}

  login(email: string, password: string) {
    return this.http.post<any>(`${this.apiUrl}/login`, { email, password });
  }

  register(data: any) {
    return this.http.post<any>(`${this.apiUrl}/register`, data);
  }

  verifyCapture(recaptchaToken: any) {
    return this.http.post<any>(`${this.apiUrl}/verify-captcha`, recaptchaToken);
  }

  // Function to call refresh token endpoint
  refreshToken(): Observable<LoginResult> {
    console.log("entered to call the api for refresh token");
  
    const refreshToken = this.getRefreshToken();
  
    // Send it as a raw string in the request body
    return this.http.post<LoginResult>(
      `${this.apiUrl}/refresh-token`,
      JSON.stringify(refreshToken), // Important: convert string to JSON string
      {
        headers: { 'Content-Type': 'application/json' }
      }
    );
  }
  

  ///////////////////////////////////////////////////////////////////////////////////////////////////

  logout() {
    this.clearTokens();
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('access_token');
  }

  getToken(): string | null {
    return localStorage.getItem('access_token');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refresh_token');
  }

  setTokens(access: string, refresh: string) {
    localStorage.setItem('access_token', access);
    localStorage.setItem('refresh_token', refresh);
    this.extractRoles();
    this.startTokenRefreshMonitor();
  }

  clearTokens() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('roles');
  }

  startTokenRefreshMonitor() {
    // setInterval(() => {
    //   const token = this.getToken();
    //   if (token && this.isTokenExpiringSoon(token)) {
    //     this.refreshToken().subscribe({
    //       next: () => console.log("Token refreshed"),
    //       error: () => this.logout()
    //     });
    //   }
    // }, 1 * 60 * 1000); // Every 5 mins
  }
  
  private isTokenExpiringSoon(token: string): boolean {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const exp = payload.exp;
    const currentTime = Math.floor(Date.now() / 1000);
    return exp - currentTime < 300; // Less than 5 minutes left
  }
  

  private extractRoles(): void {
    if (!this.userRoles) {
      const token = this.getToken();
      if (token) {
        try {
          const payloadStr = atob(token.split('.')[1]);
          const payload = JSON.parse(payloadStr);
  
          // Extract role from the long claim key
          const roleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
          const roles = payload[roleClaim];
  
          // Ensure roles are stored as an array
          this.userRoles = Array.isArray(roles) ? roles : [roles];
  
          localStorage.setItem('roles', JSON.stringify(this.userRoles));
          console.log("Extracted user roles:", this.userRoles);
  
        } catch (error) {
          console.error("Failed to parse token roles:", error);
          this.userRoles = [];
        }
      } else {
        this.userRoles = [];
      }
    }
  }
  
  hasRole(role: string): boolean {
    if (!this.userRoles) {
      this.extractRoles(); // Extract roles if not already done
    }
    var res = this.userRoles?.includes(role);
    return res? res: false;
  }
}
