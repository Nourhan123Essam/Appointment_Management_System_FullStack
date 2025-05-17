import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppStateService {

  private currentLangSubject = new BehaviorSubject<string>('en');
  currentLang$ = this.currentLangSubject.asObservable();

  setLanguage(lang: string) {
    this.currentLangSubject.next(lang);
    localStorage.setItem('lang', lang); // Persist user preference
  }

  getSavedLanguage() {
    return localStorage.getItem('lang') ?? 'en';
  }
}
