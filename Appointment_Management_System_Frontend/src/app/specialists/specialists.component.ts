import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-specialists',
  standalone: true,
  imports: [],
  templateUrl: './specialists.component.html',
  styleUrl: './specialists.component.css'
})
export class SpecialistsComponent {
  specialistType!: string;

  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.specialistType = params['type'];
      this.fetchDoctorsBySpecialist(this.specialistType);
    });
  }

  fetchDoctorsBySpecialist(type: string) {
    // Call your API to get doctors based on the `type`
  }
}
