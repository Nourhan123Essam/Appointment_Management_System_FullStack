import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, Validators} from '@angular/forms';
import { Doctor, WorkplaceType } from '../../core/Interfaces/Doctor';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { DoctorStateService } from '../../core/services/State/doctor-state.service';
import { DialogModule } from 'primeng/dialog';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-update-doctor',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, TableModule, DialogModule, ToastModule],
  standalone: true,
  templateUrl: './update-doctor.component.html',
  styleUrl: './update-doctor.component.css'
})
export class UpdateDoctorComponent implements OnInit{
    currentDoctor: Doctor | null = null;
    doctorForm!: FormGroup;
    workplaceTypes = WorkplaceType;
    workplaceTypeOptions = Object.keys(WorkplaceType).filter(key => isNaN(Number(key))); // removes '0', '1', '2';
  
    messageService = inject(MessageService);
  
    constructor(private fb: FormBuilder, private doctorStateService: DoctorStateService, public ref: DynamicDialogRef, public config: DynamicDialogConfig) {}
  
    ngOnInit(): void {
      this.currentDoctor = this.config.data;
      console.log("doctor to be updated,: ", this.currentDoctor);
      
      this.initForm();
    }
  
    closeDialog() {
      this.ref.close();
    }
  
    getWorkplaceValue(type: string): number {
      return this.workplaceTypes[type as keyof typeof WorkplaceType];
    }
  
    showSuccessToast() {
      this.messageService.add({
        severity: 'success',
        summary: 'Success',
        detail: 'Doctor added successfully!',
        life: 3000
      });
    }
  
    private initForm(): void {
      this.doctorForm = this.fb.group({
        fullName: [this.currentDoctor?.fullName, Validators.required],
        email: [this.currentDoctor?.email, Validators.compose([Validators.required, Validators.email])],
        password: [this.currentDoctor?.password, [Validators.required, this.passwordValidator]],
        specialization: [this.currentDoctor?.specialization],
        licenseNumber: [this.currentDoctor?.licenseNumber],
        yearsOfExperience: [this.currentDoctor?.yearsOfExperience, [Validators.min(0), Validators.max(70), Validators.required]],
        consultationFee: [this.currentDoctor?.consultationFee, [Validators.min(0), Validators.max(5000), Validators.required]],
        workplaceType: [this.currentDoctor?.workplaceType, Validators.required],
      });
    }
  
    passwordValidator(control: AbstractControl): { [key: string]: boolean } | null {
      const value: string = control.value || '';
      console.log(value);
      
      // Correct Regular Expression for Password Validation
      const passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$#!%*?&]{6,}$/;
  
      if (!passwordPattern.test(value)) {
        return { passwordStrength: true }; // If the password does NOT match, return an error
      }
      return null; // If the password is valid, return null
    }
  
    onSubmit() {
      if (this.doctorForm.invalid) return;
    
      const updatedDoctor = {
        ...this.currentDoctor,
        ...this.doctorForm.value,
        password: this.doctorForm.value.password || undefined // don't send empty password
      };
    
      this.doctorStateService.updateDoctor(updatedDoctor).subscribe(() => {
        this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'Doctor updated successfully.' });
        this.ref.close(true); // or return updated data if needed
      });
    }
    
}
