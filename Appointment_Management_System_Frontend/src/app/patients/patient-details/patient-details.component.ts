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
import { PatientApiService, PatientDto } from '../../core/services/Api/patient-api.service';


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
   dialogService = inject(DialogService);
    authService = inject(AuthService);
    patientService = inject(PatientApiService);

    patient: PatientDto | null = null;
  
    doctorId: string = '';
    isAdmin: boolean = false; // load this from your auth service
    constructor(
      private route: ActivatedRoute, 
      private confirmationService: ConfirmationService, 
      private messageService: MessageService,
    ) {}
  
  
    ngOnInit() {
      const id = this.route.snapshot.paramMap.get('id');
      if(id){
        this.patientService.getPatientById(id).subscribe({
          next: (res) => {
            this.patient = res;
          },
          error: (er) => {
            console.log(er);
            
          }
        })
      }
    }
    
  
   
    
  
}
