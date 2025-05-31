import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-doctors-with-specialist',
  standalone: true,
  imports: [],
  templateUrl: './doctors-with-specialist.component.html',
  styleUrl: './doctors-with-specialist.component.css'
})
export class DoctorsWithSpecialistComponent {
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
