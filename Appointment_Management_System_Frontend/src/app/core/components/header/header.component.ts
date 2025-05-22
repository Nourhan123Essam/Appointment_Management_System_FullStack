import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { MessageService } from 'primeng/api';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule } from '@angular/forms';
import { AppStateService } from '../../services/State/app-state.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, 
    DropdownModule, FormsModule, TranslateModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {
  isAdmin: boolean = false;
  isLoggedIn: boolean = false;
  authService = inject(AuthService);
  dropdownOpen = false;
  activeSection: 'home' | 'specialists' | 'doctors' = 'home';
  isProfileDropdownOpen = false;

  // Moq data until handle this from API
  userName = 'Nourhan Essam';
  userRole = 'Admin'; 
  userImageUrl = 'https://cdn-icons-png.flaticon.com/512/2922/2922561.png'; 


  // Example list until fetch from API
  specialists = [
    { id: 1, name: 'Cardiology' },
    { id: 2, name: 'Neurology' },
    { id: 3, name: 'Orthopedics' }
  ];
  languages = [
    { code: 'en', label: 'English' },
    { code: 'ar', label: 'العربية' }
  ];
  
  selectedLang = this.translate.getDefaultLang();
  selectedLangObj = {};

  constructor(
    public router: Router,
    private messageService: MessageService, 
    private translate: TranslateService,
    private appState: AppStateService
  ) {
    const browserLang = translate.getBrowserLang();
    this.selectedLang = browserLang?.match(/en|ar/) ? browserLang : 'en';

    if(this.selectedLang == 'en'){
      this.selectedLangObj = { code: 'en', label: 'English' };
    }
    else this.selectedLangObj = { code: 'ar', label: 'العربية' };

    // direction
    document.documentElement.dir = this.selectedLang === 'ar' ? 'rtl' : 'ltr';

    // set language in localstorage
    this.appState.setLanguage(this.selectedLang);

    this.translate.use(this.selectedLang);
  }

  toggleProfileDropdown() {
    this.isProfileDropdownOpen = !this.isProfileDropdownOpen;
  }

  closeProfileDropdown() {
    setTimeout(() => {
      this.isProfileDropdownOpen = false;
    }, 200);
  }
   
  toggleDropdown() {
    this.dropdownOpen = !this.dropdownOpen;
    console.log(this.dropdownOpen);
    if(this.dropdownOpen){
      this.activeSection = 'specialists';
    }
  }

  goToSpecialist(specialist: any) {
    this.dropdownOpen = false;
    this.activeSection = 'specialists';
    this.router.navigate(['/specialists'], { queryParams: { type: specialist.name } });
  }

  ngOnInit(): void {
    this.authService.isLoggedIn().subscribe(logged => {
      this.isLoggedIn = logged;
    });

    this.authService.getRolesStream().subscribe(roles => {
      this.isAdmin = roles? roles.includes("Admin"): false;
    });
    console.log("is admin from header", this.isAdmin);
    
  }

  switchLang(langCode: any) {
    this.translate.use(langCode.code);
    this.selectedLang = langCode.code;

    // set language in localstorage
    this.appState.setLanguage(this.selectedLang);
    
    // Change text direction
    document.documentElement.dir = langCode.code === 'ar' ? 'rtl' : 'ltr';
  }

  logout() {
    this.authService.logout().subscribe( // Calls backend + clears tokens
      {
        next: (res: any)=> {
          this.authService.clearTokens();
          this.messageService.add({ severity: 'success', summary: this.translate.instant('common.success'), detail: res.message });
          this.router.navigate(['/login']);
        },
        error: err => this.messageService.add({ severity: 'error', summary: this.translate.instant('common.error'), detail: err.error.message || 'Error lougout' })
      }
    ); 
  }

}
