import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { DoctorQualification } from '../../core/Interfaces/DoctorQualification';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-add-qualification',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, ButtonModule],
  templateUrl: './add-update-qualification.component.html',
  styleUrl: './add-update-qualification.component.css'
})
export class AddUpdateQualificationComponent {
  form!: FormGroup;
  doctorId!: string;
  title: string = "Add Qualification";

  constructor(
    private fb: FormBuilder,
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig
  ) {
    this.doctorId = this.config.data.doctorId;
    const qualification = config.data.qualification;
    console.log("qualifcation to be updated",  qualification);
    if(qualification != undefined)this.title = "Update Qualification";
    
    this.form = this.fb.group({
      qualificationName: [qualification?.qualificationName??'', Validators.required],
      issuingInstitution: [qualification?.issuingInstitution??''],
      yearEarned: [
        qualification?.yearEarned??null,
        [Validators.required, Validators.min(1900), Validators.max(new Date().getFullYear())]
      ]
    });
  }

  submit(): void {
    if (this.form.valid) {
      const qualification: DoctorQualification = this.form.value;
      console.log("qualification to be added", qualification);
      
      this.ref.close(qualification);
    }
  }

  close(): void {
    this.ref.close();
  }
}
