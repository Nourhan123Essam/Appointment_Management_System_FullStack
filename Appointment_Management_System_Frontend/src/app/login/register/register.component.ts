import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { User } from '../../core/Interfaces/User';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { environment } from '../../../environments/environment';
import { RecaptchaModule } from "ng-recaptcha";
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MessageService } from 'primeng/api';


@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    FormsModule, 
    CommonModule, 
    ReactiveFormsModule,
    CalendarModule,
    DropdownModule,
    RecaptchaModule,
    RouterLink,
    TranslateModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm!: FormGroup;
  confirmPassword: string = '';

   recaptchaToken: string = '';
   siteKey = environment.recaptchaSiteKey;

  genders = [
    { label: 'Male', value: 'Male' },
    { label: 'Female', value: 'Female' }
  ];
  
  maxDate = new Date(); // prevent future dates

  constructor(
    private translate: TranslateService,
    private messageService: MessageService,
    private fb: FormBuilder, 
    private authService: AuthService, 
    private router: Router) {
      this.renderRecaptcha();
    }

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(3)]],
      lastName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern('^01[0-9]{9}$')]], 
      password: ['', [Validators.required, this.passwordValidator]],
      confirmPassword: ['', Validators.required],
      gender: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      address: [''],
    });
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

  passwordValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value: string = control.value || '';
    console.log(value);
    

    // Correct Regular Expression for Password Validation
    const passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?#&]{6,}$/;

    if (!passwordPattern.test(value)) {
      return { passwordStrength: true }; // If the password does NOT match, return an error
    }
    return null; // If the password is valid, return null
  }
    // Check if passwords match
    passwordsMatch(): boolean {
      return this.registerForm.get('password')?.value === this.registerForm.get('confirmPassword')?.value;
    }

  onSubmit(): void {
    if (this.registerForm.valid && this.passwordsMatch()) {
      const userData: User = {
        Id: '',
        firstName: this.registerForm.value.firstName,
        email: this.registerForm.value.email,
        phone: this.registerForm.value.phone,
        password: this.registerForm.value.password,
        lastName: this.registerForm.value.lastName,
        address: this.registerForm.value.address,
        gender: this.registerForm.value.gender.value,
        dateOfBirth: this.registerForm.value.dateOfBirth
      };
      
      console.log('Registering user:', userData);

      this.authService.verifyCapture({ recaptchaToken: this.recaptchaToken }).subscribe({
        next:(response: any) => {
          console.log('Captcha Verified', response);
          this.authService.register(userData).subscribe({
          next: (res) => {
          alert('Registration successful! Please login.');
          this.router.navigate(['/login']);
          },
          error: (error) => {
          alert('Registration failed!')
          }
          });
        },
        error: (error) => {
          alert('Login failed2!')
        }
      });

      this.authService.verifyCapture({ recaptchaToken: this.recaptchaToken }).subscribe({
      next: (response: any) => {
        console.log('Captcha Verified', response);

        this.authService.register(userData).subscribe({
          next: (res) => {
            this.messageService.add({
              severity: 'success',
              summary: this.translate.instant('common.success'),
              detail: res.message
            });
            this.router.navigate(['/login']);
          },
          error: (error) => {
            this.messageService.add({
              severity: 'error',
              summary: this.translate.instant('common.error'),
              detail: error.error.message
            });
          }
        });
      },
      error: (error) => {
        const captchaError = this.translate.instant('register.captchaError');
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('common.error'),
          detail: captchaError
        });
      }
    });

    }
  }
}
