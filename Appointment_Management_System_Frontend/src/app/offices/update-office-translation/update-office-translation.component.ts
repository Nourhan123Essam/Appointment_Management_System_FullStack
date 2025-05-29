import { Component } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { OfficeTranslation } from '../../core/Interfaces/OfficeTranslation';
import { OfficeStateService } from '../../core/services/State/office-state.service';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-update-office-translation',
  standalone: true,
  imports: [
      CommonModule,
      FormsModule,
      ReactiveFormsModule,
      ButtonModule,
      InputTextModule
    ],
  templateUrl: './update-office-translation.component.html',
  styleUrl: './update-office-translation.component.css'
})
export class UpdateOfficeTranslationComponent {
  form: FormGroup;
  loading = false;
  translation: OfficeTranslation;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private fb: FormBuilder,
    private officeService: OfficeStateService,
    private messageService: MessageService
  ) {
    this.translation = this.config.data;
    this.form = this.fb.group({
      name: [this.translation.name, Validators.required],
      city: [this.translation.city, Validators.required],
      streetName: [this.translation.streetName, Validators.required],
      state: [this.translation.state, Validators.required],
      country: [this.translation.country, Validators.required],
    });
  }

  update() {
  if (this.form.invalid) return;

  this.loading = true;

  const updated: OfficeTranslation = {
    ...this.translation,
    ...this.form.value
  };
  console.log("translation before update", this.translation);
  
  console.log("updated office", updated);
  

  this.officeService.updateTranslation(updated).subscribe((success) => {
    if (success) {
      this.ref.close(true); // triggers success toast
    } else {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to update translation'
      });
      this.loading = false;
    }
  });
}


  cancel() {
    this.ref.close(false);
  }
}
