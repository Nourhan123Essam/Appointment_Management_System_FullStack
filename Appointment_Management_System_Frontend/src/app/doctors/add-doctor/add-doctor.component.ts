import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, FormsModule, Validators, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Doctor, WorkplaceType } from '../../core/Interfaces/Doctor';
import { DayOfWeek } from '../../core/Interfaces/DoctorAvailability';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { DoctorStateService } from '../../core/services/State/doctor-state.service';
import { DialogModule } from 'primeng/dialog';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

export const timeRangeValidator: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
  const start = group.get('startTime')?.value;
  const end = group.get('endTime')?.value;

  if (!start || !end) return null;

  // Convert to Date objects to compare
  const startDate = new Date(`2000-01-01T${start}`);
  const endDate = new Date(`2000-01-01T${end}`);

  return endDate > startDate ? null : { invalidTimeRange: true };
};


@Component({
  selector: 'app-add-doctor',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, TableModule, DialogModule, ToastModule],
  standalone: true,
  templateUrl: './add-doctor.component.html',
  styleUrl: './add-doctor.component.css'
})
export class AddDoctorComponent implements OnInit {
  doctorForm!: FormGroup;
  daysOfWeek = DayOfWeek;
  showQualifications = false;
  showAvailabilities = false;
  keys = Object.keys;
  workplaceTypes = WorkplaceType;
  daysOfWeekOptions = this.keys(this.daysOfWeek).filter(key => isNaN(Number(key)));
  workplaceTypeOptions = this.keys(WorkplaceType).filter(key => isNaN(Number(key))); // removes '0', '1', '2';

  messageService = inject(MessageService);

  constructor(private fb: FormBuilder, private doctorStateService: DoctorStateService, public ref: DynamicDialogRef) {}

  ngOnInit(): void {
    console.log('work place types', this.workplaceTypes);
    console.log("keys", this.keys(this.workplaceTypes));
    
    console.log('days', this.daysOfWeek);
    
    
    this.initForm();
  }

  closeDialog() {
    this.ref.close();
  }

  getWorkplaceValue(type: string): number {
    return this.workplaceTypes[type as keyof typeof WorkplaceType];
  }

  getDayOfWeekValue(day: string): number {
    return this.daysOfWeek[day as keyof typeof DayOfWeek];
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
      fullName: ['', Validators.required],
      email: ['', Validators.compose([Validators.required, Validators.email])],
      password: ['', [Validators.required, this.passwordValidator]],
      specialization: [''],
      licenseNumber: [''],
      yearsOfExperience: [null, [Validators.min(0), Validators.max(70), Validators.required]],
      consultationFee: [null, [Validators.min(0), Validators.max(5000), Validators.required]],
      workplaceType: [WorkplaceType.Clinic, Validators.required],
      qualifications: this.fb.array([]),
      availabilities: this.fb.array([])
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

  toggleQualifications() {
    this.showQualifications = !this.showQualifications;
  }

  toggleAvailabilities() {
    this.showAvailabilities = !this.showAvailabilities;
  }

  get qualifications(): FormArray {
  return this.doctorForm.get('qualifications') as FormArray;
  }

  get availabilities(): FormArray {
    return this.doctorForm.get('availabilities') as FormArray;
  }


  addQualification(): void {
    this.qualifications.push(this.fb.group({
      qualificationName: ['', Validators.required],
      issuingInstitution: [''],
      yearEarned: [null, [Validators.required, Validators.min(1900), Validators.max(new Date().getFullYear())]]
    }));
  }

  removeQualification(index: number): void {
    this.qualifications.removeAt(index);
  }

  addAvailability(): void {
    this.availabilities.push(this.fb.group({
      dayOfWeek: [DayOfWeek.Friday, Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required]
    }, { validators: timeRangeValidator }));
  }

  removeAvailability(index: number): void {
    this.availabilities.removeAt(index);
  }

  formatTime = (time: string): string => {
    return time.length === 5 ? `${time}:00` : time; // e.g. "03:43" -> "03:43:00"
  };

  
  addDoctor(): Doctor {
    const formValue = this.doctorForm.value;
  
    const newDoctor: Doctor = {
      id: '', // Assuming this will be generated by the backend
      fullName: formValue.fullName,
      email: formValue.email,
      password: formValue.password,
      yearsOfExperience: formValue.yearsOfExperience,
      specialization: formValue.specialization,
      licenseNumber: formValue.licenseNumber,
      consultationFee: formValue.consultationFee,
      workplaceType: Number(formValue.workplaceType),
      TotalRatingScore: 0, // Default value
      TotalRatingsGiven: 0, // Default value
      qualifications: formValue.qualifications.map((q: any) => ({
        id: 0, // Backend should assign a real ID
        qualificationName: q.qualificationName,
        issuingInstitution: q.issuingInstitution,
        yearEarned: q.yearEarned,
        doctorId: '' // Will be assigned by backend
      })),
      availabilities: formValue.availabilities.map((a: any) => ({
        id: 0, // Backend should assign a real ID
        dayOfWeek: Number(a.dayOfWeek),
        startTime: this.formatTime(a.startTime),
        endTime: this.formatTime(a.endTime),
        doctorId: '' // Will be assigned by backend
      }))
    };

    return newDoctor;
  }

  submitForm(): void {
    if (this.doctorForm.valid) {
      debugger;
      var newDoctor = this.addDoctor();

      // newDoctor.workplaceType = WorkplaceType.Online;
      
      console.log("created doctor", newDoctor);
      
      this.doctorStateService.addDoctor(newDoctor).subscribe({
        next: (savedDoctor) => {
          console.log('Doctor added successfully:', savedDoctor);
          this.closeDialog();
          this.showSuccessToast();
        },
        error: (err) => {
          console.error('Error adding doctor:', err);
        }
      });
    } else {
      console.log('Form is invalid');
    }
  }
  
}
