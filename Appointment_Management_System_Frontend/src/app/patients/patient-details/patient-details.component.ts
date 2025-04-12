import { Component, inject } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { BadgeModule } from 'primeng/badge';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { MessageModule } from 'primeng/message';
import { ActivatedRoute } from '@angular/router';
import { OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { DoctorStateService } from '../../core/services/State/doctor-state.service';
import { DayOfWeek, DoctorAvailability } from '../../core/Interfaces/DoctorAvailability';
import { DoctorQualification } from '../../core/Interfaces/DoctorQualification';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { firstValueFrom, map, Observable, switchMap, take } from 'rxjs';
import { AppointmentDto, PatientApiService, PatientDto } from '../../core/services/Api/patient-api.service';


@Component({
  selector: 'app-patient-details',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    BadgeModule,
    TableModule,
    TabViewModule,
    MessageModule,
    ConfirmDialogModule
  ],
  providers: [DialogService, ConfirmationService],
  templateUrl: './patient-details.component.html',
  styleUrl: './patient-details.component.css'
})
export class PatientDetailsComponent {
    patientService = inject(PatientApiService);

    patient: PatientDto | null = null;
    appointments: AppointmentDto[] = [];

    noAppointments = false;
  
    constructor(private route: ActivatedRoute) {}
 
    ngOnInit() {
      const id = this.route.snapshot.paramMap.get('id');
      console.log("recieved id", id);
      
      if(id){
        // get patient details
        this.patientService.getPatientById(id).subscribe({
          next: (res) => {
            this.patient = res;
          },
          error: (er) => console.log(er)
        });

        // get patient appointments
        this.patientService.getPatientAppointments(id).subscribe({
          next: (res) =>this.appointments = res,
          error: (er) => console.log(er)
        });
        this.noAppointments = this.appointments.length == 0;
        console.log("appointments", this.appointments);
        
      }
    }
    
  
   
    
  
}
