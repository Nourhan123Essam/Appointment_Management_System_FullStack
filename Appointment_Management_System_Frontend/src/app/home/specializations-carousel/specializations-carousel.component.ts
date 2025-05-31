import { Component } from '@angular/core';
import { SpecializationWithDoctorCount } from '../../core/Interfaces/SpecializationWithDoctorCount';
import { SpecializationStateService } from '../../core/services/State/specialization-state.service';
import { CommonModule } from '@angular/common';
import { CarouselModule } from 'primeng/carousel';
import { AppStateService } from '../../core/services/State/app-state.service';


@Component({
  selector: 'app-specializations-carousel',
  standalone: true,
  imports: [CommonModule, CarouselModule],
  templateUrl: './specializations-carousel.component.html',
  styleUrl: './specializations-carousel.component.css'
})
export class SpecializationsCarouselComponent {
  specializations: SpecializationWithDoctorCount[] = [];
  isRtl = false;
  constructor(private state: SpecializationStateService, private app_state: AppStateService) {}

  ngOnInit(): void {
    this.state.loadSpecializationsWithDoctorCount();
    this.state.specializationsWithDoctorCount$
      .subscribe(data =>{
        
        this.specializations = data || [];
        console.log("specialists in home", this.specializations);
      });   


      this.isRtl = this.app_state.getSavedLanguage() === 'ar'; // or your RTL language code

      this.app_state.currentLang$.subscribe((lang) => {
        this.isRtl = lang === 'ar';
      });

  }

  trackBySpecialization(index: number, item: SpecializationWithDoctorCount) {
    return item.specializationId;
  }

}
