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


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, DropdownModule, RouterLink,
    ButtonModule, InputTextModule, RecaptchaModule,SelectButtonModule, DropdownModule ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  username = '';
  password = '';
  selectedRole = signal<string>(''); // Track role selection

  recaptchaToken: string = '';
  siteKey = environment.recaptchaSiteKey;

  roles = [
    { label: 'Doctor', value: 'Doctor' },
    { label: 'Patient', value: 'Patient' },
    { label: 'Admin', value: 'Admin' }
  ];
  

  constructor(private authService: AuthService, private router: Router) {
    // Effect: Run renderRecaptcha when selectedRole changes
    effect(() => {
      console.log(this.selectedRole());
      
      if (this.selectedRole() != 'Admin') {
       // this.renderRecaptcha();
      }
    });
    //this.renderRecaptcha();
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
      this.handleCaptchaResponse(token);
    }
  }

  handleCaptchaResponse(response: string) {
    this.recaptchaToken = response; // Store the token received from Google
  }

  onLogin() {
    if (this.selectedRole() && this.selectedRole() == 'Admin') {
      this.authService.login(this.username, this.password, this.selectedRole()).subscribe({
        next: (response) => {
          localStorage.setItem('token', response.token);
          localStorage.setItem('role', this.selectedRole());
          this.router.navigate(['/appointments']);
        },
        error: (error) => {
          alert('Login failed!')
        }
      });
    }
    else if (!this.recaptchaToken) {
      alert('Please complete the reCAPTCHA');
    }
    else{
      this.authService.verifyCapture({ recaptchaToken: this.recaptchaToken }).subscribe({
        next:(response: any) => {
          console.log('Captcha Verified', response);
          this.authService.login(this.username, this.password, this.selectedRole()).subscribe({
            next: (res) => {
              localStorage.setItem('token', res.token);
              localStorage.setItem('role', this.selectedRole());
              this.router.navigate(['/appointments']);
            },
            error: (error) => {
              alert('Login failed!')
            }
          });
        },
        error: (error) => {
          alert('Login failed2!')
        }
      })
    }
  }
}
