import { Component } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SpecializationTranslation } from '../../core/Interfaces/SpecializationTranslation';
import { SpecializationStateService } from '../../core/services/State/specialization-state.service';

@Component({
  selector: 'app-update-specialist-translation',
  standalone: true,
   imports: [
      CommonModule,
      FormsModule,
      ReactiveFormsModule,
      ButtonModule,
      InputTextModule
    ],
  templateUrl: './update-specialist-translation.component.html',
  styleUrl: './update-specialist-translation.component.css'
})
export class UpdateSpecialistTranslationComponent {
   form: FormGroup;
    loading = false;
    translation: SpecializationTranslation;
  
    constructor(
      public ref: DynamicDialogRef,
      public config: DynamicDialogConfig,
      private fb: FormBuilder,
      private state: SpecializationStateService,
    ) {
      this.translation = this.config.data;
      this.form = this.fb.group({
        name: [this.translation.name, Validators.required],
       });
    }
  
    update() {
    if (this.form.invalid) return;
  
    this.loading = true;
  
    const updated: SpecializationTranslation = {
      ...this.translation,
      ...this.form.value
    };
    console.log("translation before update", this.translation);
    
    console.log("updated specialization", updated);
    
    this.state.updateTranslation(this.translation.specializationId, updated);
    this.cancel();
  }
  
  cancel() {
    this.ref.close(false);
  }
}
