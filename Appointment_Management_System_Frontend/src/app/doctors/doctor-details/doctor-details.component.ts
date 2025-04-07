import { Component, inject } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { UpdateDoctorComponent } from '../update-doctor/update-doctor.component';
import { Doctor, WorkplaceType } from '../../core/Interfaces/Doctor';
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
import { AddUpdateQualificationComponent } from '../add-update-qualification/add-update-qualification.component';
import { AddUpdateAvailabilityComponent } from '../add-update-availability/add-update-availability.component';

@Component({
  selector: 'app-doctor-details',
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
  templateUrl: './doctor-details.component.html',
  styleUrl: './doctor-details.component.css',
  providers: [DialogService, ConfirmationService]
})
export class DoctorDetailsComponent implements OnInit{
  dialogService = inject(DialogService);
  authService = inject(AuthService);
  doctorService = inject(DoctorStateService);

  doctorAvailabilities$!: Observable<DoctorAvailability[]>;
  doctorQualifications$!: Observable<DoctorQualification[]>;

  doctorId: string = '';
  doctor$!: Observable<Doctor | undefined>;
  isAdmin: boolean = false; // load this from your auth service
  constructor(
    private route: ActivatedRoute, 
    private confirmationService: ConfirmationService, 
    private messageService: MessageService,
  ) {}


  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.doctorId = id;
  
      // Reactive doctor observable using current cached doctors$
      this.doctor$ = this.doctorService.doctors$.pipe(
        map(doctors => (doctors || []).find(d => d.id === this.doctorId))
      );
  
      // Availabilities
      this.doctorAvailabilities$ = this.doctorService.getDoctorAvailabilities(this.doctorId).pipe(
        switchMap(() => this.doctorService.doctorAvailabilities$.pipe(
          map(data => data[this.doctorId] || [])
        ))
      );
  
      // Qualifications
      this.doctorQualifications$ = this.doctorService.getDoctorQualifications(this.doctorId).pipe(
        switchMap(() => this.doctorService.doctorQualifications$.pipe(
          map(data => data[this.doctorId] || [])
        ))
      );
    }
  
    this.isAdmin = this.authService.hasRole("Admin");
  }
  

  getWorkplaceLabel(type: WorkplaceType | number | undefined): string {
    switch (type) {
      case WorkplaceType.Online:
        return 'Online';
      case WorkplaceType.Hospital:
        return 'Hospital';
      case WorkplaceType.Clinic:
        return 'Clinic';
      default:
        return 'Unknown';
    }
  }  

  getWorkplaceBadgeClass(type: WorkplaceType | number | undefined): string {
    switch (type) {
      case WorkplaceType.Online:
        return 'bg-primary text-white'; // blue
      case WorkplaceType.Hospital:
        return 'bg-secondary text-white'; // gray
      case WorkplaceType.Clinic:
        return 'bg-purple text-white'; // light purple (custom class or use 'bg-info' as fallback)
      default:
        return 'bg-dark text-white';
    }
  }

  getDayOfWeekLabel(day: number | undefined): string {
    if(day == undefined)return "";
    return DayOfWeek[day];
  }

  async openUpdateDoctorDialog() {
    const doctor = await firstValueFrom(this.doctor$);
    this.dialogService.open(UpdateDoctorComponent, { 
      data: doctor, 
      width: '50%', 
      showHeader: false,
      closable: true 
    });
  }

  async deleteQualification(qualificationId: number) {
    const doctor = await firstValueFrom(this.doctor$);
    
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete this qualification?',
      header: 'Confirm Deletion',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        if (!doctor?.id) return;
  
        this.doctorService.deleteDoctorQualification(doctor.id, qualificationId).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Qualification deleted successfully' });
          },
          error: () => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete qualification' });
          }
        });
      }
    });
  }  

  async deleteAvailability(availabilityId: number) {
    console.log("let's trace the delete of availability");
    const doctor = await firstValueFrom(this.doctor$);
    
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete this availability?',
      header: 'Confirm Deletion',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        console.log("is the problem in confirm!!!");
        if (!doctor?.id) return;
        
        this.doctorService.deleteDoctorAvailability(this.doctorId, availabilityId).subscribe({
          next: () => {
            console.log("availability deleted");
            
            this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Availability deleted successfully' });
          },
          error: () => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete availability' });
          }
        });
      }
    });
  }

   editAvailability(availability: any) {
    console.log("aviability to be edit", availability);
    
    const ref = this.dialogService.open(AddUpdateAvailabilityComponent, {
      width: '35%',
      data: {
        doctorId: this.doctorId,
        availability: availability // Pass the current availability
      },
      closable: true,
      showHeader: false,
      styleClass: 'p-dialog-custom'
    });

    ref.onClose.subscribe((updatedAvailability: DoctorAvailability) => {
      if (updatedAvailability) {
        updatedAvailability.doctorId = this.doctorId;
        updatedAvailability.id = availability?.id?? 0;
        this.doctorService
          .updateDoctorAvailability(this.doctorId, updatedAvailability)
          .subscribe({
            next: () => {              
              this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'Availability updated successfully' });
            },
            error: () => {
              this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update availability' });
            }
         }); 
      }
    });
   
  }

  openAddAvailabilityDialog(){
    const ref = this.dialogService.open(AddUpdateAvailabilityComponent, {
      width: '35%',
      data: { doctorId: this.doctorId },
      closable: true,
      showHeader: false,
      styleClass: 'p-dialog-custom'
    });
  
    ref.onClose.subscribe((Availability: DoctorAvailability) => {
      if (Availability) {
        Availability.doctorId = this.doctorId;
        this.doctorService
          .addDoctorAvailability(this.doctorId, Availability)
          .subscribe({
            next: () => {              
              this.messageService.add({ severity: 'success', summary: 'Added', detail: 'Availability added successfully' });
            },
            error: () => {
              this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to add availability' });
            } 
          }); 
      }
    });
  }

  editQualification(qualification: any) {
    const ref = this.dialogService.open(AddUpdateQualificationComponent, {
      width: '35%',
      data: { doctorId: this.doctorId, qualification: qualification },
      closable: true,
      showHeader: false,
      styleClass: 'p-dialog-custom'
    });
  
    ref.onClose.subscribe((Qualification: DoctorQualification) => {
      if (Qualification) {
        Qualification.doctorId = this.doctorId;
        Qualification.id = qualification.id;
        console.log("qualificatoin after update", Qualification);
        
        this.doctorService
          .updateDoctorQualifications(this.doctorId, Qualification)
          .subscribe(
            {
              next: () => {                
                this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'Qualificatoin updated successfully' });
              },
              error: () => {
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update qualificatoin' });
              }
            }
          ); 
      }
    });
  }

  openAddQualificationDialog(): void {
    const ref = this.dialogService.open(AddUpdateQualificationComponent, {
      width: '35%',
      data: { doctorId: this.doctorId },
      closable: true,
      showHeader: false,
      styleClass: 'p-dialog-custom'
    });
  
    ref.onClose.subscribe((qualification: DoctorQualification) => {
      if (qualification) {
        qualification.doctorId = this.doctorId;
        this.doctorService
          .addDoctorQualification(this.doctorId, qualification)
          .subscribe( {
            next: () => {              
              this.messageService.add({ severity: 'success', summary: 'Added', detail: 'Qualificatoin added successfully' });
            },
            error: () => {
              this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to added qualificatoin' });
            }
          }); 
      }
    });
  }
}
