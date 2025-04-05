import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RecaptchaModule } from "ng-recaptcha";
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RecaptchaModule, ToastModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'my-angular17-app';
  resolved(captchaResponse: string|null) {
    console.log(`Resolved captcha with response: ${captchaResponse}`);
  }
}
