import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { MessageService } from 'primeng/api';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements OnInit {
 userId!: string;
  token!: string;

  form = this.fb.group({
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  });

  constructor(
    private translate: TranslateService,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private authService: AuthService,
    private messageService: MessageService,
    private router: Router
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.userId = params['userId'];
      this.token = params['token'];
    });
  }

  submit() {
    if (this.form.invalid || this.form.value.newPassword !== this.form.value.confirmPassword) {
      const errorMessage = this.translate.instant('resetPassword.mismatch');
      this.messageService.add({ severity: 'error', summary: 'Error', detail: errorMessage });
      return;
    }

    this.authService.resetPassword(this.token, this.form.value.newPassword!, this.form.value.confirmPassword!).subscribe({
      next: () => {
        const successMessage = this.translate.instant('resetPassword.success');
        this.messageService.add({ severity: 'success', summary: this.translate.instant('common.success'), detail: successMessage });
        this.router.navigate(['/login']);
      },
      error: err => {
        const fallbackError = this.translate.instant('resetPassword.errorFallback');
        const message = err.error?.message ?? fallbackError;
        this.messageService.add({ severity: 'error', summary: this.translate.instant('common.error'), detail: message });
      }
    });

  }
}
