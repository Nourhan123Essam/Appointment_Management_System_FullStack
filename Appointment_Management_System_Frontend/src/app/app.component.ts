import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RecaptchaModule } from "ng-recaptcha";
import { ToastModule } from 'primeng/toast';
import { HeaderComponent } from "./core/components/header/header.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RecaptchaModule, ToastModule, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'my-angular17-app';
  resolved(captchaResponse: string|null) {
    console.log(`Resolved captcha with response: ${captchaResponse}`);
  }
}
