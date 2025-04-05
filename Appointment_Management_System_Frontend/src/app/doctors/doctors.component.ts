import { Component, inject } from '@angular/core';
import { DoctorStateService } from '../core/services/State/doctor-state.service';
import { AuthService } from '../core/services/auth.service';
import { Router } from '@angular/router';
import { Doctor, WorkplaceType } from '../core/Interfaces/Doctor';
import { DialogService } from 'primeng/dynamicdialog';
import { CommonModule } from '@angular/common';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { AddDoctorComponent } from './add-doctor/add-doctor.component';
import { UpdateDoctorComponent } from './update-doctor/update-doctor.component';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { Table } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { HttpClientModule } from '@angular/common/http';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { InputGroupModule } from 'primeng/inputgroup';  // Add this for inputgroup

@Component({
  selector: 'app-doctors',
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    ConfirmDialogModule,
    InputGroupModule,  // Use InputGroupModule
    TagModule,
    HttpClientModule,
    InputTextModule,  // Use InputTextModule
    MultiSelectModule,
  ],
  standalone: true,
  templateUrl: './doctors.component.html',
  styleUrl: './doctors.component.css',
  providers: [ConfirmationService, DialogService]
})
export class DoctorsComponent {
  private doctorStateService = inject(DoctorStateService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private dialogService = inject(DialogService);
  private confirmationService = inject(ConfirmationService);


  doctors: Doctor[] = [];
  filteredDoctors: Doctor[] = [];
  searchQuery = '';
  selectedWorkplaceType: string | null = null;
  selectedSpecialization: string | null = null;
  isAdmin = false;
  rowsPerPage = 10;
  searchValue: string | undefined;


  ngOnInit() {
    this.isAdmin = this.authService.hasRole('Admin');
    this.doctorStateService.fetchDoctors().subscribe((doctors) => {
      this.doctors = doctors?doctors: [];
      this.filteredDoctors = doctors?doctors: [];
      console.log(doctors);
      
    });
    this.doctorStateService.doctors$.subscribe((doctors) => {
      this.doctors = doctors?doctors: [];
      this.filteredDoctors = doctors?doctors: [];
      console.log(doctors);
      
    });
  }

  sortDoctors(event: any) {
    const field = event.field;
    const order = event.order;
    this.filteredDoctors.sort((a, b) => {
      const valueA = (a as any)[field] ?? 0;
      const valueB = (b as any)[field] ?? 0;
      return order * (valueA - valueB);
    });
  }

  getRating(doctor: Doctor): number {
    return (doctor.TotalRatingsGiven && doctor.TotalRatingScore) ? 
      (doctor.TotalRatingScore / doctor.TotalRatingsGiven) : 0;
  }

  goToDoctorDetails(doctorId: string) {
    console.log('Navigating to doctor details:', doctorId);
    if (doctorId) {
      this.router.navigate(['/doctor-details', doctorId]);
    }
  }
  
  openAddDoctorDialog() {
    this.dialogService.open(AddDoctorComponent, { 
      width: '50%',
      showHeader: false,
      closable: true
     });
  }

  openUpdateDoctorDialog(doctor: Doctor) {
    this.dialogService.open(UpdateDoctorComponent, { data: doctor, width: '50%' });
  }

  confirmDelete(doctorId: string) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete this doctor?',
      header: 'Confirm Deletion',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.doctorStateService.deleteDoctor(doctorId);
      }
    });
  }
  
  clear(table: Table) {
    table.clear();
    this.searchValue = ''
  }

  // Filter method for the global filter
  onGlobalFilter(event: any, dt: Table) {
    const value = (event.target as HTMLInputElement).value;
    dt.filterGlobal(value, 'contains');
  }
}
