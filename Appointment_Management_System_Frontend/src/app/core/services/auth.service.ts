import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment.development';

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

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('roles');
    this.userRoles = null; // Clear cached roles
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  private extractRoles(): void {
    if (!this.userRoles) {
      const token = this.getToken();
      if (token) {
        const payload = JSON.parse(atob(token.split('.')[1])); // Decode JWT payload
        
        // Handle multiple roles
        if (payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]) {
          const roles = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
          
          // Ensure roles are stored as an array
          this.userRoles = Array.isArray(roles) ? roles : [roles];
  
          localStorage.setItem('roles', JSON.stringify(this.userRoles)); // Store in local storage
        } else {
          this.userRoles = [];
        }
      } else {
        this.userRoles = [];
      }
    }
    console.log("Extracted user roles", this.userRoles);
    
  }
  
  hasRole(role: string): boolean {
    if (!this.userRoles) {
      this.extractRoles(); // Extract roles if not already done
    }
    var res = this.userRoles?.includes(role);
    return res? res: false;
  }
}
