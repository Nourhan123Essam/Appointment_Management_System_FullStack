import { Component } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {
  emailForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private messageService: MessageService
  ) {}

  submit() {
    if (this.emailForm.invalid) return;

    this.authService.requestPasswordReset(this.emailForm.value.email!).subscribe({
      next: () => this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Check your email for the reset link.' }),
      error: err => this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error.message || 'Error sending reset link' })
    });
  }
}
