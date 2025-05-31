import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-home-hero-stats',
  standalone: true,
  imports: [],
  templateUrl: './home-hero-stats.component.html',
  styleUrl: './home-hero-stats.component.css'
})
export class HomeHeroStatsComponent {
   @Input() stats = {
    doctors: null,
    offices: null,
    appointments: null,
    specializations: null
  };
}
