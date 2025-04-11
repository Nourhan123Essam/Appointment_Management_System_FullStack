// Angular 17 Standalone Component with PrimeNG (Table, Inputs, Dropdown, Slider)
// Component name: PatientListComponent
// This component displays patients with filtering, sorting, pagination, and delete functionality

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { SliderModule } from 'primeng/slider';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { HttpClientModule } from '@angular/common/http';
import { PagedResult, PatientApiService, PatientDto, PatientQueryParams } from '../core/services/Api/patient-api.service';
import { CardModule } from 'primeng/card';


@Component({
  selector: 'app-patients',
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    HttpClientModule,
    TableModule,
    InputTextModule,
    DropdownModule,
    ConfirmDialogModule,
    ButtonModule,
    SliderModule,
    ToastModule,
    CardModule    
  ],
  providers: [ConfirmationService],
  standalone: true,
  templateUrl: './patients.component.html',
  styleUrl: './patients.component.css'
})
export class PatientsComponent {
  patients: PatientDto[] = [];
  totalCount = 0;
  loading = true;

  // Filtering and sorting state
  nameFilter = '';
  emailFilter = '';
  sortBy = '';
  isDescending = false;
  pageNumber = 1;
  pageSize = 2;

  sortOptions = [
    { label: 'Name', value: 'fullName' },
    { label: 'Email', value: 'email' }
  ];

  constructor(
    private patientService: PatientApiService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {
    this.loadPatients();
  }

  loadPatients(): void {
    this.loading = true;
    const queryParams: PatientQueryParams = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      search: this.nameFilter || this.emailFilter,
      sortBy: this.sortBy,
      isDescending: this.isDescending
    };
    // debugger;
    console.log("query", queryParams);
    
    this.patientService.getPatients(queryParams).subscribe({
      next: (result: PagedResult<PatientDto>) => {
        console.log("resurlt patients coming", result);
        
        this.patients = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load patients.' });
      }
    });
  }

  onPageChange(event: any) {
    console.log("when page change", event);
  
    this.pageNumber = Math.floor(event.first / event.rows) + 1;
    this.pageSize = event.rows;
    console.log(this.pageNumber, this.pageSize);
    
    this.loadPatients();
  }

  confirmDelete(patient: PatientDto): void {
    this.confirmationService.confirm({
      message: `Are you sure you want to delete ${patient.fullName}?`,
      header: 'Confirm Delete',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.patientService.deletePatient(patient.id).subscribe(() => {
          this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Patient deleted successfully.' });
          this.loadPatients();
        });
      }
    });
  }

  goToDetails(patient: PatientDto): void {
    // Navigate to patient details
    window.location.href = `/patient-details/${patient.id}`;
  }

  onFilterChange(): void {
    this.pageNumber = 1;
    this.loadPatients();
  }

  onSortChange(): void {
    this.loadPatients();
  }

  onDirectionToggle(): void {
    this.isDescending = !this.isDescending;
    this.loadPatients();
  }
}
