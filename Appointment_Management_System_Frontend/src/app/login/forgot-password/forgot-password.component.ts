import { Component } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { MessageService } from 'primeng/api';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule,
    TranslateModule, RouterLink, CommonModule
  ],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {
  emailForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  constructor(
    private translate: TranslateService,
    private fb: FormBuilder,
    private authService: AuthService,
    private messageService: MessageService
  ) {}

  submit() {
  if (this.emailForm.invalid) return;

  this.authService.requestPasswordReset(this.emailForm.value.email!).subscribe({
    next: () => this.messageService.add({
      severity: 'success',
      summary: this.translate.instant('common.success'),
      detail: this.translate.instant('forgotPassword.success')
    }),
    error: err => this.messageService.add({
      severity: 'error',
      summary: this.translate.instant('common.error'),
      detail: err.error.message || this.translate.instant('forgotPassword.failure')
    })
  });
}

}
