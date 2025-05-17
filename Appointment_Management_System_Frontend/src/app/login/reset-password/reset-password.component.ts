import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { MessageService } from 'primeng/api';
import { TranslateModule } from '@ngx-translate/core';

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
      this.messageService.add({ severity: 'error', summary: 'Error', detail: "Passwords do not match or are invalid."})
      return;
    }

    this.authService.resetPassword(this.token, this.form.value.newPassword!, this.form.value.confirmPassword!).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Password reset successfully' });
        this.router.navigate(['/login']);
      }, 
      error: err => this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error.message || 'Error resetting password'})
    });
  }
}
