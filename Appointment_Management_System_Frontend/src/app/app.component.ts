import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RecaptchaModule } from "ng-recaptcha";
import { ToastModule } from 'primeng/toast';
import { HeaderComponent } from "./core/components/header/header.component";
import { AuthService } from './core/services/auth.service';
import { SecondaryHeaderComponent } from './core/components/secondary-header/secondary-header.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RecaptchaModule, ToastModule, HeaderComponent, SecondaryHeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'my-angular17-app';

 isLoggedIn: boolean = false;
  constructor(private auth: AuthService){
    this.auth.isLoggedIn().subscribe(logged => {
      this.isLoggedIn = logged;
    });
  }
  resolved(captchaResponse: string|null) {
    console.log(`Resolved captcha with response: ${captchaResponse}`);
  }
}
