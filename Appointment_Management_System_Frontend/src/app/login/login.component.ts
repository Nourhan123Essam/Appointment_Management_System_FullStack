import { AfterViewInit, Component, effect, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { environment } from '../../environments/environment.development';
import { RecaptchaModule } from "ng-recaptcha";
import { SelectButtonModule } from 'primeng/selectbutton';
import { AuthService } from '../core/services/auth.service';
import { MessageService } from 'primeng/api';
import { TranslateModule } from '@ngx-translate/core';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, DropdownModule, RouterLink,
    ButtonModule, InputTextModule, RecaptchaModule,SelectButtonModule, DropdownModule, TranslateModule ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  username = '';
  password = '';

  recaptchaToken: string = '';
  siteKey = environment.recaptchaSiteKey;

  roles = [
    { label: 'Doctor', value: 'Doctor' },
    { label: 'Patient', value: 'Patient' },
    { label: 'Admin', value: 'Admin' }
  ];
  

  constructor(
    private authService: AuthService, 
    private router: Router,
    private messageService: MessageService
  ) {
    this.renderRecaptcha();
  }

  renderRecaptcha() {
    if (typeof grecaptcha === 'undefined') {
      console.warn("reCAPTCHA script not loaded yet.");
      setTimeout(() => this.renderRecaptcha(), 500); // Retry after delay
      return;
    }
  
    const captchaElement = document.querySelector('.g-recaptcha');
    if (captchaElement) {
      grecaptcha.render(captchaElement as HTMLElement, {
        sitekey: this.siteKey,
        callback: this.onCaptchaResolved.bind(this)
      });
    } else {
      console.warn("reCAPTCHA element not found.");
    }
  }
  

  // Called when CAPTCHA is solved
  onCaptchaResolved(token: string|null) {
    console.log('Captcha resolved with token:', token);
    if(token){
      this.recaptchaToken = token; // Store the token received from Google
    }
  }

  onLogin() {   
    this.authService.verifyCapture({ recaptchaToken: this.recaptchaToken }).subscribe({
      next:(response: any) => {
        console.log('Captcha Verified', response);
        this.authService.login(this.username, this.password).subscribe({
          next: (res) => {
            console.log("message after login" , res);
            
            this.authService.setTokens(res.data.accessToken, res.data.refreshToken);
            this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Login completed successfully!' });
            this.router.navigate(['/appointments']);
          },
          error: (error) => {
            console.log("error when login", error);
            
            this.messageService.add({ severity: 'error', summary: 'Error', detail: error.error.message || 'Login failed!' });
          }
        });
      },
      error: (error) => {
        console.log("error 2222 when login", error);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: error.message || 'Login failed!' });
      }
    });
  }
}
