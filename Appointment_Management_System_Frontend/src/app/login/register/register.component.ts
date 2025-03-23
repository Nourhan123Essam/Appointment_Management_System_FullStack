import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { User } from '../../core/Interfaces/User';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm!: FormGroup;
  confirmPassword: string = '';

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.pattern('^01[0-9]{9}$')]], 
      password: ['', [Validators.required, this.passwordValidator]],
      confirmPassword: ['', Validators.required],
    });
  }

  passwordValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value: string = control.value || '';
    console.log(value);
    

    // Correct Regular Expression for Password Validation
    const passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/;

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
        fullName: this.registerForm.value.fullName,
        email: this.registerForm.value.email,
        telephoneNumber: this.registerForm.value.phoneNumber,
        password: this.registerForm.value.password
      };
      
      console.log('Registering user:', userData);
      this.authService.register(userData).subscribe({
        next: (res) => {
          alert('Registration successful! Please login.');
          this.router.navigate(['/login']);
        },
        error: (error) => {
          alert('Registration failed!')
        }
      });
    }
  }
}
