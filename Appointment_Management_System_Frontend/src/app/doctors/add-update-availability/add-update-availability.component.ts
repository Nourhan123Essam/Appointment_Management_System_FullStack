import { Component, Inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { DoctorQualification } from '../../core/Interfaces/DoctorQualification';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { DayOfWeek, DoctorAvailability } from '../../core/Interfaces/DoctorAvailability';
 
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
  selector: 'app-add-availability',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, ButtonModule],
  templateUrl: './add-update-availability.component.html',
  styleUrl: './add-update-availability.component.css'
})
export class AddUpdateAvailabilityComponent {
   form!: FormGroup;
    doctorId!: string;
    title: string = "Add Availability";
    daysOfWeek = DayOfWeek;
    daysOfWeekOptions = Object.keys(this.daysOfWeek).filter(key => isNaN(Number(key)));

  
    constructor(
      private fb: FormBuilder,
      private ref: DynamicDialogRef,
      private config: DynamicDialogConfig
    ) {
      this.doctorId = this.config.data.doctorId;
      const passedAvailability = this.config.data?.availability;
      if(passedAvailability != undefined) this.title = "Update Availability";

      console.log("passed availabiltiy", passedAvailability);
      
      this.form = this.fb.group({
        dayOfWeek: [passedAvailability?.dayOfWeek ?? DayOfWeek.Friday, Validators.required],
          startTime: [passedAvailability?.startTime ?? '', Validators.required],
          endTime: [passedAvailability?.endTime ?? '', Validators.required]
        }, { validators: timeRangeValidator }
      );
    }
  
    getDayOfWeekValue(day: string): number {
      return this.daysOfWeek[day as keyof typeof DayOfWeek];
    }

    formatTime = (time: string): string => {
      return time.length === 5 ? `${time}:00` : time; // e.g. "03:43" -> "03:43:00"
    };

    submit(): void {
      if (this.form.valid) {
        const Availability: DoctorAvailability = this.form.value;
        Availability.dayOfWeek = Number(Availability.dayOfWeek);
        Availability.startTime = this.formatTime(Availability.startTime);
        Availability.endTime = this.formatTime(Availability.endTime);
        console.log("Availability to be added", Availability);
        
        this.ref.close(Availability);
      }
    }
  
    close(): void {
      this.ref.close();
    }
}
