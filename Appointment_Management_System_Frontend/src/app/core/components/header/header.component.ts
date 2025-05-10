import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {
  isAdmin: boolean = false;
  isLoggedIn: boolean = false;
  authService = inject(AuthService);

  constructor(public router: Router,private messageService: MessageService) {}

  logout() {
    this.authService.logout().subscribe( // Calls backend + clears tokens
      {
        next: ()=> {
          this.authService.clearTokens();
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Lougout completed!' });
          this.router.navigate(['/login']);
        },
        error: err => this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error.message || 'Error lougout' })
      }
    ); 
  }

  ngOnInit(): void {
    this.authService.isLoggedIn().subscribe(logged => {
      this.isLoggedIn = logged;
    });

    this.authService.getRolesStream().subscribe(roles => {
      this.isAdmin = roles? roles.includes("Admin"): false;
    });
  }

}
